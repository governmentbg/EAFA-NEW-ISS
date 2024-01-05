using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using IARA.MigrationScript.Models;

namespace IARA.MigrationScript.Migrations
{
    public class MigrateTicketExceptionFiles : BaseMigrate
    {
        private readonly List<string> files;
        private readonly MigrateTicketFiles migrateTickets;

        public MigrateTicketExceptionFiles(int startId, int? endId, string folder) : base(startId, endId)
        {
            this.migrateTickets = new MigrateTicketFiles(startId, endId);

            foreach (string file in Directory.GetFiles(folder))
            {
                if (file.StartsWith("Exception"))
                {
                    this.files.Add(file);
                }
            }
        }

        public override void Init()
        {
        }

        public override Task Run()
        {
            this.Timer.Start();

            this.AllThreadsUsed = true;
            this.UtilizedThreads = 1;

            DateTime currentTimestamp = DateTime.Now;

            foreach (string file in this.files)
            {
                ExceptionModel exception = JsonSerializer.Deserialize<ExceptionModel>(File.ReadAllText(file));

                this.migrateTickets.CurrentId = exception.Chunk.Start;
                this.migrateTickets.ThreadLoop(0);
            }

            this.TimePerLoop.Add(DateTime.Now - currentTimestamp);

            this.CurrentId = ChunkSize;
            this.UtilizedThreads = 0;

            this.Timer.Stop();

            this.OnFinish();

            return Task.CompletedTask;
        }

        public override void ThreadLoop(int loopIndex)
        {
        }
    }
}
