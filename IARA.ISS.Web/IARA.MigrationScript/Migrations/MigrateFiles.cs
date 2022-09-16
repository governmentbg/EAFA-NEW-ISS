using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using IARA.MigrationScript.Entities;
using IARA.MigrationScript.Models;
using MySql.Data.MySqlClient;
using Npgsql;

namespace IARA.MigrationScript.Migrations
{
    public class MigrateFiles : BaseMigrate
    {
        private string selectMaxIdQuery;
        private string selectQuery;
        private string duplicateQuery;
        private string insertFileQuery;
        private string updateFileQuery;

        public MigrateFiles(int startId, int? endId)
            : base(startId, endId) { }

        public override void Init()
        {
            this.selectMaxIdQuery = this.ReadScript("SelectMaxFileId");
            this.selectQuery = this.ReadScript("SelectFileRange");
            this.duplicateQuery = this.ReadScript("FindFileDuplicate");
            this.insertFileQuery = this.ReadScript("InsertFile");
            this.updateFileQuery = this.ReadScript("UpdateFile");

            if (this.LastId == 0)
            {
                using (IDbConnection oldDb = new MySqlConnection(OldDbCS))
                {
                    this.LastId = oldDb.QueryFirst<int>(this.selectMaxIdQuery);
                }
            }
        }

        protected override void ThreadLoop(int loopIndex)
        {
            using IDbConnection oldDb = new MySqlConnection(OldDbCS);
            using IDbConnection newDb = new NpgsqlConnection(NewDbCS);

            this.UtilizedThreads++;

            while (this.CurrentId < this.LastId + ChunkSize && !this.CancelToken.IsCancellationRequested)
            {
                ChunkModel obj;

                lock (this.LockObj)
                {
                    obj = new ChunkModel
                    {
                        Start = this.CurrentId,
                        End = this.CurrentId + ChunkSize - 1
                    };

                    this.CurrentId += ChunkSize;
                }

                DateTime currentTimestamp = DateTime.Now;

                long currentFileId = obj.Start;

                try
                {
                    List<FileEntity> query = oldDb.Query<FileEntity>(this.selectQuery, obj).AsList();

                    this.TimePerQuery.Add(DateTime.Now - currentTimestamp);

                    foreach (FileEntity file in query)
                    {
                        currentFileId = file.Id;

                        string hash = this.ComputeFileContentHash(file.Content);

                        List<FileIARAEntity> duplicateCount = newDb.Query<FileIARAEntity>(this.duplicateQuery, new
                        {
                            ContentLength = file.File_Size,
                            ContentHash = hash,
                        }).AsList();

                        string strId = file.Id.ToString();

                        if (duplicateCount.Count == 0)
                        {
                            newDb.Execute(this.insertFileQuery, new
                            {
                                Name = file.File_Name,
                                MimeType = file.File_Type,
                                ContentHash = hash,
                                ContentLength = file.File_Size,
                                Content = file.Content,
                                Comments = file.Ctrl_Code,
                                UploadedOn = file.Create_Date,
                                UpdatedBy = strId,
                            });
                        }
                        else if (!duplicateCount[0].UpdatedBy.Split(',').Contains(strId))
                        {
                            int fileId = duplicateCount[0].Id;

                            newDb.Execute(this.updateFileQuery, new
                            {
                                Id = strId,
                                CtrlCode = file.Ctrl_Code,
                                FileId = fileId
                            });
                        }

                        this.LastReachedIds[loopIndex] = file.Id;
                    }
                }
                catch (Exception ex)
                {
                    this.WriteException(ex, obj, currentFileId);
                }

                this.TimePerLoop.Add(DateTime.Now - currentTimestamp);
            }

            this.UtilizedThreads--;
        }
    }
}
