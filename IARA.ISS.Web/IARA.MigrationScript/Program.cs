using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Dapper;
using IARA.MigrationScript.Migrations;
using IARA.MigrationScript.Models;

namespace IARA.MigrationScript
{
    public static class Program
    {
        private static async Task Main()
        {
            SqlMapper.Settings.CommandTimeout = 0;

            Console.CursorVisible = false;

            (int startId, int? endId) = ApplySettings();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("Press 1 to migrate files from schema prod_iara");
                Console.WriteLine("Press 2 to migrate files from schema prod_iara_tickets");
                Console.WriteLine("Press 3 to hash the passwords of the specified users");
                char input = Console.ReadKey(false).KeyChar;

                if (input == '1')
                {
                    await RunMigration(new MigrateFiles(startId, endId));
                }
                else if (input == '2')
                {
                    await RunMigration(new MigrateTicketFiles(startId, endId));
                }
                else if (input == '3')
                {
                    await RunMigration(new MigrateTicketUsers(startId, endId));
                }

                GC.Collect();
            }
        }

        private static async Task RunMigration(BaseMigrate migration)
        {
            try
            {
                migration.BeforeInit();
                migration.Init();
                migration.AfterInit();

                await migration.Run();

                migration.Dispose();
            }
            catch (Exception ex)
            {
                migration.Dispose();

                Console.WriteLine(ex.Message + '\n');
                Console.WriteLine("Press any key to return to selection mode.");
                Console.ReadKey(false);
            }
        }

        private static (int, int?) ApplySettings()
        {
            const string path = "settings.json";

            if (File.Exists(path))
            {
                string text = File.ReadAllText(path);

                SettingsModel settings = JsonSerializer.Deserialize<SettingsModel>(text);

                BaseMigrate.OldDbCS = settings.ConnectionStrings.Source;
                BaseMigrate.NewDbCS = settings.ConnectionStrings.Destination;

                BaseMigrate.ThreadCount = settings.ThreadCount;
                BaseMigrate.TimeToNextThread = settings.TimeToNextThread * 1000;
                BaseMigrate.ChunkSize = settings.ChunkSize;

                return (settings.StartId, settings.EndId);
            }
            else
            {
                Console.WriteLine("Could not locate " + path);
                Environment.Exit(0);
                return (0, null);
            }
        }
    }
}
