using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using IARA.Common.Utils;
using IARA.MigrationScript.TicketEntities;
using Npgsql;

namespace IARA.MigrationScript.Migrations
{
    public class MigrateTicketUsers : BaseMigrate
    {
        private string selectUserQuery;
        private string updateTicketUser;

        public MigrateTicketUsers(int startId, int? endId)
            : base(startId, endId) { }

        public override void Init()
        {
            this.selectUserQuery = this.ReadScript("SelectTicketUser");
            this.updateTicketUser = this.ReadScript("UpdateTickerUser");

            this.LastId = ChunkSize;
        }

        public override Task Run()
        {
            this.Timer.Start();

            using IDbConnection newDb = new NpgsqlConnection(NewDbCS);

            this.AllThreadsUsed = true;
            this.UtilizedThreads = 1;

            DateTime currentTimestamp = DateTime.Now;

            try
            {
                IEnumerable<TicketUserEntity> query = newDb.Query<TicketUserEntity>(this.selectUserQuery, buffered: true);

                this.TimePerQuery.Add(DateTime.Now - currentTimestamp);

                foreach (TicketUserEntity user in query)
                {
                    string hash = CommonUtils.GetPasswordHash(user.Password, user.Username);

                    newDb.Execute(this.updateTicketUser, new
                    {
                        Id = user.Id,
                        Password = hash,
                    });
                }
            }
            catch (Exception ex)
            {
                this.WriteException(ex, new Models.ChunkModel(), 0);
            }

            this.TimePerLoop.Add(DateTime.Now - currentTimestamp);

            this.CurrentId = ChunkSize;
            this.UtilizedThreads = 0;

            this.Timer.Stop();

            this.OnFinish();

            return Task.CompletedTask;
        }

        protected override void ThreadLoop(int loopIndex)
        {
        }
    }
}
