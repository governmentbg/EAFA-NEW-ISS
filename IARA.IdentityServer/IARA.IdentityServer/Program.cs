using System;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace IARA.IdentityServer
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "IARA IdentityServer";
            TechnoLogica.IdentityServer.Program.CreateWebHostBuilder<Startup>(args)
                    .UseContentRoot(Environment.CurrentDirectory)
                     .ConfigureAppConfiguration((context, builder) =>
                     {
                         builder.SetBasePath(Directory.GetCurrentDirectory());

                         builder.LoadAppSettings(context.HostingEnvironment.EnvironmentName, AppDomain.CurrentDomain.BaseDirectory);
                         builder.LoadAppSettings(context.HostingEnvironment.EnvironmentName, Environment.CurrentDirectory);
                         var fileFullName = new FileInfo(Process.GetCurrentProcess().MainModule.FileName);
                         builder.LoadAppSettings(context.HostingEnvironment.EnvironmentName, fileFullName.Directory.FullName);
                     }).UseStartup<Startup>().Build().Run();
        }

        private static void LoadAppSettings(this IConfigurationBuilder builder, string environment, string location)
        {
            builder.AddJsonFile(Path.Combine(location, "appsettings.json"), true);
            builder.AddJsonFile(Path.Combine(location, $"appsettings.{environment}.json"), true);
        }
    }
}
