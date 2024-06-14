using IARA.Mobile.Application;
using IARA.Mobile.Application.Interfaces.Factories;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Infrastructure.Builders;
using IARA.Mobile.Insp.Application.Interfaces.Factories;
using IARA.Mobile.Insp.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace IARA.Mobile.Insp.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            AppDbContextBuilder builder = new AppDbContextBuilder();
            services.AddSingleton<IAppDbContextBuilder, AppDbContextBuilder>((_) => builder);
            services.AddSingleton<IDbContextBuilder, AppDbContextBuilder>((_) => builder);

            services.AddTransient<IAppDbMigration, AppDbMigration>();

            services.AddServerUrlConfigs();

            return services;
        }

        private static IServiceCollection AddServerUrlConfigs(this IServiceCollection services)
        {
            ServerUrlFactoryBuilder builder = new ServerUrlFactoryBuilder();

            builder.AddUrl(Environments.PRODUCTION, "https://iara-iss.egov.bg/api/");
            builder.AddExternalUrl(Environments.PRODUCTION, "IARA_IDENTITY", "https://iara-iss.egov.bg/identity");

            builder.AddUrl(Environments.STAGING, "https://iara-iss-staging-internal.egov.bg/api/");
            builder.AddExternalUrl(Environments.STAGING, "IARA_IDENTITY", "https://iara-staging.egov.bg/identity");

            builder.AddUrl(Environments.DEVELOPMENT_PUBLIC, "https://iara.technologica.com/public-api/");
            builder.AddExternalUrl(Environments.DEVELOPMENT_PUBLIC, "IARA_IDENTITY", "https://iara.technologica.com/identity");

            builder.AddUrl(Environments.DEVELOPMENT_INTERNAL, "https://172.31.12.168/api/");
            builder.AddExternalUrl(Environments.DEVELOPMENT_INTERNAL, "IARA_IDENTITY", "https://172.31.12.168/identity");

            builder.AddUrl(Environments.DEVELOPMENT_LOCAL, "https://4njgdpd4-5000.euw.devtunnels.ms/api/");
            builder.AddExternalUrl(Environments.DEVELOPMENT_LOCAL, "IARA_IDENTITY", "https://iara-iss.egov.bg/identity");

            builder.AddExtension("Services", "Mobile/Administrative/");
            builder.AddExtension("Common", "Common/");

            builder.Build(services);

            return services;
        }
    }
}

