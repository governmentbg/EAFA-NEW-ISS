using System;
using System.IO;
using System.Linq;
using IARA.Common.Utils;
using IARA.DataAccess;
using IARA.DI;
using IARA.Tests;
using IARA.Tests.Tests;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TLTTS.Common.ConfigModels;
using static IARA.Tests.TestExtensions;

namespace IARA.StateMachine.Test
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            IServiceProvider serviceProvider = CreateServicesCollection("appsettings.Development.json").BuildServiceProvider();

            MigratePlainPasswords(serviceProvider);

            TestFluxFishingActivities(serviceProvider);

            TestStateMachine(serviceProvider);

            TestCrossChecks(serviceProvider);

            Console.ReadLine();

            Console.WriteLine("Hello World!");
        }

        private static void MigratePlainPasswords(IServiceProvider serviceProvider)
        {
            var Db = serviceProvider.GetService<IARADbContext>();
            using (Db.BeginTransaction())
            {
                var users = Db.Users.Where(x => x.Password != null && x.Password.Length != 64).ToList();
                foreach (var user in users)
                {
                    user.Password = CommonUtils.GetPasswordHash(user.Password, user.Email);
                    Db.SaveChanges();
                }

                Db.CommitTransaction();
            }

        }

        private static void TestFluxFishingActivities(IServiceProvider serviceProvider)
        {
            var test = serviceProvider.CreateTestService<FluxFATests>();
            test.TestReporting();
        }

        private static void TestStateMachine(IServiceProvider serviceProvider)
        {
            var test = serviceProvider.CreateTestService<StateMachineTests>();
            test.Test();
        }

        private static void TestCrossChecks(IServiceProvider serviceProvider)
        {
            var crossChecksTests = serviceProvider.CreateTestService<CrossChecksTests>();
            crossChecksTests.Test();
        }

        private static IServiceCollection CreateServicesCollection(string jsonFile)
        {
            IServiceCollection services = new ServiceCollection();

            IConfiguration configuration = new ConfigurationBuilder()
                  .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                  .AddJsonFile(jsonFile, false)
                  .Build();

            services.AddSingleton<IConfiguration>(configuration);
            services.AddSettingsModels(configuration);
            services.AddDbContext(ConnectionStrings.Default);
            services.AddHttpContextAccessor();
            services.AddDomainServices();
            services.AddManualDomainServices();

            return services;
        }
    }
}
