using System;
using System.Linq;
using IARA.DataAccess;
using IARA.DI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TLTTS.Common.ConfigModels;

namespace IARA.DbTest
{
    internal static class Program
    {
        public static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();

            DatabaseInitializer.AddDbContext(services, new ConnectionStrings
            {
                Connection = "Host=172.31.12.168;Port=5432;Database=iss;Username=iara;Password=qaz123WSX;SslMode=Prefer;Trust Server Certificate=true;Include Error Detail=true;"
            });

            CommonInitializer.AddDomainServices(services);
            CommonInitializer.AddManualDomainServices(services);
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            IARADbContext db = serviceProvider.GetService<IARADbContext>();

            var poundnet = db.PoundNetRegisters
                .Include(x => x.PoundNetCoordinates)
                .Where(x => x.Id == 2)
                .First();

            Console.WriteLine("Hello World!");
        }
    }
}
