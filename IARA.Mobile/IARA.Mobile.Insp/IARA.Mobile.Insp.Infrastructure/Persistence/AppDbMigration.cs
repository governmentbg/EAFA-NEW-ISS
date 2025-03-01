﻿using IARA.Mobile.Application;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Insp.Infrastructure.Persistence.Migrations;
using IARA.Mobile.Insp.Infrastructure.Persistence.Migrations.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

/*
 * IMPORTANT! If you want to add new tables/columns (or remove some) and such in production here is what you need to do:
 * 1. Update the CURRENT_VERSION (add 1 to it).
 * 2. Create a Version{X} file inside of the Migrations folder.
 * 3. Make the Version{X} file inherit the IVersion interface.
 * 4. You can now implement the changes that this version will bring inside of the Version{X} class Migrate method.
 * 5. Implement those same changes in the Migrations folder / the ModelSnapshot class.
 * 6. Go to line 56 in this file and add the new Version{X} file at the end of the array with IVersion-s.
 * NOTE: Make sure the versions in the array are ordered in ascending order using {X}.
 * ALSO NOTE: The ModelSnapshot class only gets called when creating the database for the first time, so you should only create tables in it.
 */

namespace IARA.Mobile.Insp.Infrastructure.Persistence
{
    public class AppDbMigration : IAppDbMigration
    {
        /// <summary>
        /// Represents the current version of the migrations
        /// </summary>
        private const int CURRENT_VERSION = 14;

        private readonly INomenclatureDatesClear nomenclatureDatesClear;
        private readonly IExceptionHandler exceptionHandler;
        private readonly IDbSettings dbSettings;
        private readonly IAuthenticationProvider authenticationProvider;

        // Needs to be a service provider because of the circular dependency
        private readonly IServiceProvider serviceProvider;

        public AppDbMigration(INomenclatureDatesClear nomenclatureDatesClear, IExceptionHandler exceptionHandler, IDbSettings dbSettings, IAuthenticationProvider authenticationProvider, IServiceProvider serviceProvider)
        {
            this.nomenclatureDatesClear = nomenclatureDatesClear ?? throw new ArgumentNullException(nameof(nomenclatureDatesClear));
            this.exceptionHandler = exceptionHandler ?? throw new ArgumentNullException(nameof(exceptionHandler));
            this.dbSettings = dbSettings ?? throw new ArgumentNullException(nameof(dbSettings));
            this.authenticationProvider = authenticationProvider ?? throw new ArgumentNullException(nameof(authenticationProvider));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public static bool DatabaseExists { get; private set; }

        public void CheckForMigrations(bool exceptionCaught = false)
        {
            AppDbContextBuilder builder = new AppDbContextBuilder();
            try
            {
                using (AppDbContext context = builder.CreateContext())
                {
                    int lastVersion = dbSettings.LastVersion;

                    // If lastVersion is the default then that means this is the first time we are creating the database
                    if (lastVersion == default)
                    {
                        ModelSnapshot modelSnapshot = new ModelSnapshot(context);
                        modelSnapshot.CreateDatabase();
                        dbSettings.LastVersion = CURRENT_VERSION;
                    }
                    else if (lastVersion != CURRENT_VERSION)
                    {
                        ICommonLogout commonLogout = serviceProvider.GetService<ICommonLogout>();
                        IVersion[] versions = new IVersion[]
                        {
                            new Version2(),
                            new Version3(nomenclatureDatesClear),
                            new Version4(authenticationProvider, commonLogout),
                            new Version5(authenticationProvider, commonLogout),
                            new Version6(),
                            new Version7(authenticationProvider, commonLogout),
                            new Version8(authenticationProvider, commonLogout),
                            new Version9(authenticationProvider, commonLogout),
                            new Version10(authenticationProvider, commonLogout),
                            new Version11(authenticationProvider, commonLogout),
                            new Version12(authenticationProvider, commonLogout),
                            new Version13(authenticationProvider, commonLogout),
                            new Version14(authenticationProvider, commonLogout),
                        };

                        for (int i = lastVersion; i < CURRENT_VERSION; i++)
                        {
                            versions[i - 1].Migrate(context);
                        }

                        dbSettings.LastVersion = CURRENT_VERSION;
                    }

                    DatabaseExists = true;
                }
            }
            catch (Exception ex) when (!exceptionCaught)
            {
                exceptionHandler.HandleException(ex);
                DropDatabase();

                CheckForMigrations(true);
            }
        }

        public void DropDatabase()
        {
            try
            {
                File.Delete(CommonGlobalVariables.DatabasePath);
                dbSettings.Clear();
                nomenclatureDatesClear.Clear();
                DatabaseExists = false;
            }
            catch
            {
                // Fail silently
            }
        }
    }
}
