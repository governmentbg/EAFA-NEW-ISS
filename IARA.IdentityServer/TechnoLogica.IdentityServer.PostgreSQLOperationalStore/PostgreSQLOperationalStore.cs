using IdentityServer4.KeyManagement.EntityFramework;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TechnoLogica.Authentication.Common;

namespace TechnoLogica.IdentityServer.PostgreSQLOperationalStore
{
    public static class PostgreSQLOperationalStore
    {
        public static IIdentityServerBuilder AddPosgreOperationalStore(this IIdentityServerBuilder builder, OperationalStore storeSettings, string connectionString)
        {
            return builder.AddOperationalStore(options =>
            {

                options.ConfigureDbContext = builder =>
                {
                    builder.UseNpgsql(connectionString);
                };

                options.EnableTokenCleanup = storeSettings.EnableCleanup;

                if (storeSettings.CleanupInterval != 0)
                {
                    options.TokenCleanupInterval = storeSettings.CleanupInterval;
                }
            });
        }

        public static IDataProtectionBuilder AddPostgrePersistance(this IDataProtectionBuilder builder, string dataProtectionConnectionString, ILoggerFactory loggerFactory)
        {
            return builder.PersistKeysToDatabase(new DatabaseKeyManagementOptions
            {
                ConfigureDbContext = b => b.UseNpgsql(dataProtectionConnectionString),
                LoggerFactory = loggerFactory,
            });
        }

        public static void AddPostgreDistributedCache(IServiceCollection services, IConfiguration configuration)
        {

        }
    }
}
