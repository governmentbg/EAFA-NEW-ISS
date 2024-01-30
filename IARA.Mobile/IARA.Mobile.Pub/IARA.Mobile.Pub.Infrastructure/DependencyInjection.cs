using IARA.Mobile.Application;
using IARA.Mobile.Application.Interfaces.Factories;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Infrastructure.Builders;
using IARA.Mobile.Pub.Application.Interfaces.Factories;
using IARA.Mobile.Pub.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace IARA.Mobile.Pub.Infrastructure
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
            builder.AddExternalUrl(Environments.PRODUCTION, "PAY_EGOV", "https://pay.egov.bg/Home/AccessByCode");
            builder.AddExternalUrl(Environments.PRODUCTION, "IARA_IDENTITY", "https://iara-iss.egov.bg/identity");
            //https://iara-iss-staging-internal.egov.bg/api/ 
            //https://n67xzpc0-5000.euw.devtunnels.ms/api/
            builder.AddUrl(Environments.STAGING, "https://n67xzpc0-5000.euw.devtunnels.ms/api/");
            builder.AddExternalUrl(Environments.STAGING, "PAY_EGOV", "https://pay-test.egov.bg/Home/AccessByCode");
            builder.AddExternalUrl(Environments.STAGING, "IARA_IDENTITY", "https://iara-iss-staging-internal.egov.bg/identity");

            builder.AddUrl(Environments.DEVELOPMENT_PUBLIC, "https://iara.technologica.com/api/");
            builder.AddExternalUrl(Environments.DEVELOPMENT_PUBLIC, "PAY_EGOV", "https://pay-test.egov.bg/Home/AccessByCode");
            builder.AddExternalUrl(Environments.DEVELOPMENT_PUBLIC, "IARA_IDENTITY", "https://iara.technologica.com/identity");

            builder.AddUrl(Environments.DEVELOPMENT_INTERNAL, "https://iara-iss-staging-internal.egov.bg/api/");
            builder.AddExternalUrl(Environments.DEVELOPMENT_INTERNAL, "PAY_EGOV", "https://pay-test.egov.bg/Home/AccessByCode");
            builder.AddExternalUrl(Environments.DEVELOPMENT_INTERNAL, "IARA_IDENTITY", "https://iara-iss-staging-internal.egov.bg/identity");

            builder.AddUrl(Environments.DEVELOPMENT_LOCAL, "https://iara-iss-staging-internal.egov.bg/api/");
            builder.AddExternalUrl(Environments.DEVELOPMENT_LOCAL, "PAY_EGOV", "https://pay-test.egov.bg/Home/AccessByCode");
            builder.AddExternalUrl(Environments.DEVELOPMENT_LOCAL, "IARA_IDENTITY", "https://iara-iss-staging-internal.egov.bg/identity");

            builder.AddUrl("PAYMENT_OK", "xamarinformsclients://okcallback");
            builder.AddUrl("PAYMENT_CANCELED", "xamarinformsclients://cancelcallback");
            builder.AddExtension("Services", "Mobile/Public/");
            builder.AddExtension("Common", "Common/");
            builder.AddExtension("Integration", "Integration/");
            builder.AddExtension("Public", "Public/");
            builder.Build(services);

            return services;
        }
    }
}
