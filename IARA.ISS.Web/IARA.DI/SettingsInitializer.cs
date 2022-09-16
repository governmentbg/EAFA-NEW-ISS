using IARA.Common.ConfigModels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TLTTS.Common.ConfigModels;

namespace IARA.DI
{
    public static class SettingsInitializer
    {
        public static IServiceCollection AddSettingsModels(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(ConnectionStrings.ReadSettings(configuration));
            services.AddSingleton(CorsSettings.ReadSettings(configuration));
            services.AddSingleton(LoggingSettings.ReadSettings(configuration));
            services.AddSingleton(EmailClientSettings.ReadSettings(configuration));
            services.AddSingleton(JwtBearerSettings.ReadSettings(configuration));
            services.AddSingleton(StartupSettings.ReadSettings(configuration));
            services.AddSingleton(ExternalSystemSettings.ReadSettings(configuration));

            return services;
        }
    }
}
