using System;
using System.Collections.Generic;
using System.IO;
using IARA.Common.Enums;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TL.JasperReports.Integration;
using TL.JasperReports.Integration.Enums;

namespace IARA.JasperReports
{

    public static class Program
    {
        public static void Main(string[] args)
        {
            IServiceProvider serviceProvider = GetServiceProvider();
            JasperReportsClient httpClient = serviceProvider.GetRequiredService<JasperReportsClient>();

            //var dictionary = new Dictionary<string, string>
            //{
            //    { "application_id", "20" }
            //};

            //byte[] fileArray = httpClient.RunReportBuffered("/reports/Scientific_fishing_register", OutputFormats.pdf, dictionary).Result;

            Console.WriteLine("Press any key to continue");
            Console.ReadLine();

            Dictionary<string, string> inputs = new Dictionary<string, string> { { "ticket_id", "535" } };
            byte[] fileArray = httpClient.RunReportBuffered(BuildReportUrl(JasperReportsEnum.Ticket_application), OutputFormats.pdf, inputs).Result;

            Console.WriteLine("File downloaded successfully");
            Console.ReadLine();
        }

        private static string BuildReportUrl(JasperReportsEnum reportType)
        {
            return $"reports/{reportType}";
        }

        private static IServiceProvider GetServiceProvider()
        {
            IServiceCollection services = new ServiceCollection();

            IConfiguration configuration = new ConfigurationBuilder()
                  .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                  .AddJsonFile("appsettings.json", false)
                  .Build();

            services.AddSingleton<Microsoft.Extensions.Configuration.IConfiguration>(configuration);
            services.AddSingleton<JasperReportsClient>();

            IServiceProvider serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }

    }
}
