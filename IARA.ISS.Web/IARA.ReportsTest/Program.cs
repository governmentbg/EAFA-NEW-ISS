using System;
using Microsoft.Extensions.DependencyInjection;
using IARA.DI;
using TLTTS.Common.ConfigModels;
using IARA.Interfaces.Reports;
using System.Linq;
using IARA.DomainModels.DTOModels.Reports;

namespace IARA.ReportsTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceProvider = GetServiceProvider();

            var reportsService = serviceProvider.GetRequiredService<IReportService>();
            var reports = reportsService.GetReport(28);

            var paramerters = reports.Parameters.Select(x => new ExecutionParamDTO
            {
                Name = x.ParameterName,
                Value = "28"
            }).ToList();

            var result = reportsService.ExecuteRawSql(reports.ReportSQL, paramerters, 2);



            Console.WriteLine("Hello World!");
        }

        private static IServiceProvider GetServiceProvider()
        {
            IServiceCollection services = new ServiceCollection();

            DatabaseInitializer.AddDbContext(services, new ConnectionStrings
            {
                Connection = "Host=172.31.12.168;Port=5432;Database=iss;Username=iara;Password=qaz123WSX;SslMode=Prefer;Trust Server Certificate=true;Include Error Detail=true;"
            });

            CommonInitializer.AddDomainServices(services);
            CommonInitializer.AddManualDomainServices(services);
            IServiceProvider serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }
    }
}
