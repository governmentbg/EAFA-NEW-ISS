using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using IARA.MigrationScript.Models;
using IARA.MigrationScript.TicketEntities;
using MySql.Data.MySqlClient;
using Npgsql;

namespace IARA.MigrationScript.Migrations
{
    public class MigrateTicketFiles : BaseMigrate
    {
        private string selectMaxIdQuery;
        private string selectFileTypesQuery;
        private string selectQuery;
        private string selectTicketIds;
        private string selectDuplicateFishingTicketFile;
        private string duplicateQuery;
        private string insertFileQuery;
        private string insertTicketFileQuery;
        private string updateFileQuery;

        private int PERSONALPHOTO, PAYEDFEE, OTHER, MEMBERSHIPCARD, IDCARD, BIRTHCERTIFICATE, D;

        public MigrateTicketFiles(int startId, int? endId)
            : base(startId, endId) { }

        public override void Init()
        {
            this.selectMaxIdQuery = this.ReadScript("SelectMaxTicketFileId");
            this.selectFileTypesQuery = this.ReadScript("SelectFileTypes");
            this.selectQuery = this.ReadScript("SelectTicketFileRange");
            this.selectTicketIds = this.ReadScript("SelectTicketIds");
            this.selectDuplicateFishingTicketFile = this.ReadScript("SelectDuplicateFishingTicketFile");
            this.duplicateQuery = this.ReadScript("FindFileDuplicate");
            this.insertFileQuery = this.ReadScript("InsertTicketFile");
            this.insertTicketFileQuery = this.ReadScript("InsertIntoFishingTicketFiles");
            this.updateFileQuery = this.ReadScript("UpdateFileReference");

            if (this.LastId == 0)
            {
                using (IDbConnection oldDb = new MySqlConnection(OldDbCS))
                {
                    this.LastId = oldDb.QueryFirst<int>(this.selectMaxIdQuery);
                }
            }

            List<FileTypeEntity> fileTypes;

            using (IDbConnection newDb = new NpgsqlConnection(NewDbCS))
            {
                fileTypes = newDb.Query<FileTypeEntity>(this.selectFileTypesQuery).ToList();
            }

            this.PERSONALPHOTO = fileTypes.First(f => f.Code == "PERSONALPHOTO").Id;
            this.PAYEDFEE = fileTypes.First(f => f.Code == "PAYEDFEE").Id;
            this.OTHER = fileTypes.First(f => f.Code == "OTHER").Id;
            this.MEMBERSHIPCARD = fileTypes.First(f => f.Code == "MEMBERSHIPCARD").Id;
            this.IDCARD = fileTypes.First(f => f.Code == "IDCARD").Id;
            this.BIRTHCERTIFICATE = fileTypes.First(f => f.Code == "BIRTHCERTIFICATE").Id;
            this.D = fileTypes.First(f => f.Code == "D").Id;
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

                        List<TicketFileEntity> ticketIds = newDb.Query<TicketFileEntity>(this.selectTicketIds, new
                        {
                            TicketId = file.Ticket_Id.ToString(),
                        }).ToList();

                        if (ticketIds.Count == 0)
                        {
                            continue;
                        }

                        string hash = this.ComputeFileContentHash(file.Content);

                        List<FileIARAEntity> duplicateCount = newDb.Query<FileIARAEntity>(this.duplicateQuery, new
                        {
                            ContentLength = file.File_Size,
                            ContentHash = hash,
                        }).AsList();

                        int fileTypeId = file.File_Tip switch
                        {
                            1 => this.PERSONALPHOTO,
                            2 => this.PAYEDFEE,
                            3 => this.OTHER,
                            4 => this.MEMBERSHIPCARD,
                            5 => this.IDCARD,
                            6 => this.BIRTHCERTIFICATE,
                            7 => this.D,
                            _ => throw new NotImplementedException("The given file type id was not present in the switch statement.")
                        };

                        foreach (TicketFileEntity ticketFile in ticketIds)
                        {
                            if (duplicateCount.Count == 0)
                            {
                                int lastFileId;
                                try
                                {
                                    lastFileId = newDb.QueryFirst<int>(this.insertFileQuery, new
                                    {
                                        Name = file.File_Name,
                                        MimeType = file.File_Type,
                                        ContentHash = hash,
                                        ContentLength = file.File_Size,
                                        Content = file.Content,
                                        UploadedOn = file.Create_Date,
                                        UpdatedBy = file.Id,
                                    });
                                }
                                catch (Exception ex) when (ex.Message == "23505: duplicate key value violates unique constraint \u0022UK_ISS_Files\u0022")
                                {
                                    // Sometimes another thread will insert the same file at the same time causing a race condition
                                    FileIARAEntity findInsertedFile = newDb.QueryFirst<FileIARAEntity>(this.duplicateQuery, new
                                    {
                                        ContentLength = file.File_Size,
                                        ContentHash = hash,
                                    });

                                    lastFileId = findInsertedFile.Id;
                                }

                                newDb.Execute(this.insertTicketFileQuery, new
                                {
                                    Id = file.Id,
                                    FileTypeId = fileTypeId,
                                    TicketId = ticketFile.Id,
                                    FileId = lastFileId,
                                    OldTicketId = file.Ticket_Id,
                                });

                                // Add the file so the second run won't try to insert the file again.
                                duplicateCount.Add(new FileIARAEntity
                                {
                                    Id = lastFileId,
                                    UpdatedBy = file.Id.ToString()
                                });
                            }
                            else
                            {
                                FileIARAEntity duplicate = duplicateCount[0];
                                int fileId = duplicate.Id;

                                List<int> ticketFileDuplicates = newDb.Query<int>(this.selectDuplicateFishingTicketFile, new
                                {
                                    FileId = fileId,
                                    TicketId = ticketFile.Id,
                                }).AsList();

                                if (ticketFileDuplicates.Count == 0)
                                {
                                    newDb.Execute(this.insertTicketFileQuery, new
                                    {
                                        Id = file.Id,
                                        FileTypeId = fileTypeId,
                                        TicketId = ticketFile.Id,
                                        FileId = fileId,
                                        OldTicketId = file.Ticket_Id,
                                    });

                                    if (!duplicate.UpdatedBy.Split(',').Contains(file.Id.ToString()))
                                    {
                                        newDb.Execute(this.updateFileQuery, new { Id = fileId });
                                    }
                                }
                            }
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
