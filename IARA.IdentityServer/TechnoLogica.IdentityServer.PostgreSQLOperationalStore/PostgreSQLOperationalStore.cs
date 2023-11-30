using IdentityServer4.KeyManagement.EntityFramework;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel.Composition;
using TechnoLogica.Authentication.Common;
using TechnoLogica.Common;

namespace TechnoLogica.IdentityServer.PostgreOperationalStore
{
    [Export(typeof(IOperationalStore))]
    [Export(typeof(IDataProtectionKeyStoreProvider))]
    //[Export(typeof(IDistributionCacheProvider))]
    public class PostgreSQLOperationalStore : IOperationalStore, IDataProtectionKeyStoreProvider//, IDistributionCacheProvider
    {

        public void AddOperationalStore(IIdentityServerBuilder builder, IConfiguration configuration)
        {
            builder.AddOperationalStore(
                    options =>
                {
                    var storeSettings = configuration.GetSettings<OperationalStore>();
                    options.ConfigureDbContext = builder =>
                        builder.UseNpgsql(configuration.GetConnectionString(storeSettings.ConnectionStringName));
                    options.EnableTokenCleanup = storeSettings.EnableCleanup;
                    if (storeSettings.CleanupInterval != 0)
                    {
                        options.TokenCleanupInterval = storeSettings.CleanupInterval;
                    }
                }
            );
        }

        public IDataProtectionBuilder AddPersistance(IDataProtectionBuilder builder, IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            var storeSettings = configuration.GetSettings<OperationalStore>();
            string dataProtectionConnectionString = configuration.GetConnectionString(storeSettings.DataProtectionConnectionStringName ?? "DataProtection");
            return builder.PersistKeysToDatabase(new DatabaseKeyManagementOptions
            {
                ConfigureDbContext = b => b.UseNpgsql(dataProtectionConnectionString),
                LoggerFactory = loggerFactory,
            });
        }

        public void AddDistributedCache(IServiceCollection services, IConfiguration configuration)
        {

        }
    }
}
