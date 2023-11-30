// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using TechnoLogica.Common.DataProtection;

namespace TechnoLogica.IdentityServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Press enter to start identity server...");
            Console.ReadLine();
            Start<BaseStartup>("TechnoLogica Identity Server", args);
        }

        public static void Start<ST>(string title, string[] args)
            where ST : BaseStartup
        {
            Console.Title = title;
            Title = title;
            if (IsConfigure(args))
            {
                var webHost = CreateWebHostBuilder<ST>(args).Build();
                var authenticationProvider = webHost.Services.GetService<IProtectedSettingsProvider>();

                authenticationProvider.ConfigureProtectedSettings();

                Console.WriteLine("Configuration finished!");
            }
            else
            {
                CreateWebHostBuilder<ST>(args).Build().Run();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder<ST>(string[] args)
            where ST : BaseStartup
        {
            var result = WebHost.CreateDefaultBuilder(args)
                    .UseStartup<ST>()
                    .UseContentRoot(Directory.GetCurrentDirectory())
                    .UseIISIntegration()
                    .UseSerilog();

            if (IsConfigure(args))
            {
                result = result.ConfigureAppConfiguration(
                     config => config.Add<WritableJsonConfigurationSource>(
                           s =>
                           {
                               s.FileProvider = null;
                               s.Path = "appsettings.json";
                               s.Optional = true;
                               s.ReloadOnChange = true;
                               s.ResolveFileProvider();
                           }
                     )
                 );
            }

            return result;
        }

        private static bool IsConfigure(string[] args)
        {
            return args != null && args.Length == 1 && args[0].ToLowerInvariant() == "-config";
        }

        public static string Title { get; set; }
    }
}
