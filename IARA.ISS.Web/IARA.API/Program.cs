using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace IARA.Web
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            using (IHost host = CreateHostBuilder(args).Build())
            {
                host.Run();
            }

            Console.WriteLine("Web server stopped!");
        }

        private static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                            .ConfigureWebHostDefaults(webBuilder =>
                            {
                                webBuilder.BuildWebHost();
                            });
        }

        private static IWebHostBuilder BuildWebHost(this IWebHostBuilder webHost)
        {
            return webHost.UseContentRoot(Environment.CurrentDirectory)
                 .ConfigureAppConfiguration((context, builder) =>
                 {

                     builder.SetBasePath(Directory.GetCurrentDirectory());
#if RELEASE
                     bool encrypted = true;
#else
                     bool encrypted = false;
#endif

                     HashSet<string> paths = new HashSet<string>();
                     paths.Add(AppDomain.CurrentDomain.BaseDirectory.TrimEnd('\\'));
                     //paths.Add(Environment.CurrentDirectory.TrimEnd('\\'));
                     var fileFullName = new FileInfo(Process.GetCurrentProcess().MainModule.FileName);
                     paths.Add(fileFullName.Directory.FullName.TrimEnd('\\'));

                     foreach (var path in paths)
                     {
                         builder.LoadAppSettings(context.HostingEnvironment.EnvironmentName, path, encrypted);
                     }

                 }).UseStartup<Startup>();
        }


        private static void LoadAppSettings(this IConfigurationBuilder builder, string environment, string location, bool encrypted)
        {
            string baseSettingsFile = "appsettings.json";

            if (encrypted)
            {
                CryptoUtils.DeleteNotUsedSettingFiles(location, environment, baseSettingsFile);
                builder.AddEncryptedJsonFile(Path.Combine(location, baseSettingsFile), true);
                builder.AddEncryptedJsonFile(Path.Combine(location, $"appsettings.{environment}.json"), true);
            }
            else
            {
                builder.AddJsonFile(Path.Combine(location, baseSettingsFile), true);
                builder.AddJsonFile(Path.Combine(location, $"appsettings.{environment}.json"), true);
            }
        }
    }
}
