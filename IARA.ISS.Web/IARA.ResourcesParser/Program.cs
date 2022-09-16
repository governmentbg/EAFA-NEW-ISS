using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using IARA.Common;
using IARA.DataAccess;
using IARA.DI;
using IARA.EntityModels.Entities;
using IARA.Logging.Abstractions.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using TLTTS.Common.ConfigModels;

namespace IARA.ResourcesParser
{
    public static class Program
    {
        private static string path = @"IARA.Web\src\app\i18n";
        public static readonly DateTime MAX_VALID_DATE = new DateTime(9999, 01, 01, 0, 0, 0);
        public static readonly DateTime MIN_VALID_DATE = new DateTime(1990, 01, 01, 0, 0, 0);

        public static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                args = new string[] { "172.23.163.13" };
            }

            IServiceProvider serviceProvider = InitializeDI(args[0]);

            Console.Write("Language BG|EN default(BG): ");
            string language = Console.ReadLine();

            if (string.IsNullOrEmpty(language))
            {
                language = "BG";
            }

            DirectoryInfo dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            dir = dir?.Parent?.Parent?.Parent?.Parent;

            string fileFullPath = null;
            string filePathLabel = "Resources file path: ";

            if (dir != null)
            {
                FileInfo file = dir.GetDirectories(path).First().GetFiles($"{language.ToLower()}.ts").First();
                fileFullPath = file.FullName;
            }

            Console.WriteLine(filePathLabel);
            Console.Write("default: " + fileFullPath);
            var position = Console.GetCursorPosition();
            Console.SetCursorPosition(filePathLabel.Length + 1, position.Top - 1);
            string filePath = Console.ReadLine();

            if (string.IsNullOrEmpty(filePath))
            {
                filePath = fileFullPath;
            }

            string json = System.IO.File.ReadAllText(filePath);
            StringBuilder builder = new StringBuilder(json);
            builder.Replace("export const locale =", "");
            builder.Replace("\"", "\\\"");
            builder.Replace("'", "\"");
            json = builder.ToString().Trim().Trim(';');
            json = Regex.Replace(json, "(\r\n){2,}", "\r\n");

            JsonSerializerOptions options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            options.AllowTrailingCommas = true;
            options.ReadCommentHandling = JsonCommentHandling.Skip;

            var categories = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(json, options);

            using (var db = serviceProvider.GetService<IARADbContext>())
            {
                using (var transaction = db.BeginTransaction())
                {
                    var labelGategories = categories.Where(x => !x.Key.EndsWith("helpers")).ToList();
                    var helpers = categories.Where(x => x.Key.EndsWith("helpers")).ToList();

                    foreach (var category in labelGategories)
                    {
                        NtranslationGroup translationGroup = db.NtranslationGroups.Where(x => x.LanguageCode == language && x.Code == category.Key).FirstOrDefault();
                        var categoryHelpers = helpers.Where(x => x.Key == $"{category.Key}-helpers").Select(x => x.Value).FirstOrDefault();

                        if (translationGroup == null)
                        {
                            translationGroup = db.NtranslationGroups.Add(new NtranslationGroup
                            {
                                Code = category.Key,
                                Name = category.Key,
                                LanguageCode = language,
                                TranslationType = "WEB",
                                ValidFrom = MIN_VALID_DATE,
                                ValidTo = MAX_VALID_DATE
                            }).Entity;

                            db.SaveChanges();
                        }

                        List<NtranslationResource> transactionResources = db.NtranslationResources.Where(x => x.TranslationGroupId == translationGroup.Id).ToList();

                        foreach (var resource in category.Value)
                        {
                            AddOrUpdateResource(db, transactionResources, translationGroup.Id, resource.Key, resource.Value, ResourceTypes.Label);
                        }

                        db.SaveChanges();

                        if (categoryHelpers != null)
                        {
                            foreach (var helper in categoryHelpers)
                            {
                                AddOrUpdateResource(db, transactionResources, translationGroup.Id, helper.Key, helper.Value, ResourceTypes.Help);
                            }
                        }

                        db.SaveChanges();
                    }

                    transaction.Commit();
                }
            }

            Console.WriteLine("Resources finished updating to database");
            Console.ReadLine();
        }

        private static void AddOrUpdateResource(IARADbContext db, List<NtranslationResource> transactionResources, int transactionGroupId, string key, string value, ResourceTypes resourceType)
        {
            NtranslationResource dbResource = transactionResources.Where(x => x.Code == key).FirstOrDefault();

            if (dbResource == null)
            {
                db.NtranslationResources.Add(new NtranslationResource
                {
                    Code = key,
                    ResourceType = resourceType.ToString(),
                    TranslationValue = value,
                    TranslationGroupId = transactionGroupId,
                    ValidFrom = MIN_VALID_DATE,
                    ValidTo = MAX_VALID_DATE
                });
            }
            else
            {
                dbResource.TranslationValue = value;
            }
        }

        private static IServiceProvider InitializeDI(string ipAddress)
        {
            IServiceCollection serviceCollection = new ServiceCollection();

            DatabaseInitializer.AddDbContext(serviceCollection, new ConnectionStrings
            {
                Connection = $"Host={ipAddress};Port=5432;Database=iss;Username=iara;Password=zFy5Ugps;SslMode=Prefer;Trust Server Certificate=true;Include Error Detail=true;"
            });

            serviceCollection.AddSingleton<IUserActionsAuditLogger>((serviceProvider) =>
            {
                return null;
            });

            serviceCollection.AddSingleton<ScopedServiceProviderFactory>();

            return serviceCollection.BuildServiceProvider();
        }
    }
}
