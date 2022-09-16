using IARA.Mobile.Application.Interfaces.Factories;
using IARA.Mobile.Application.Interfaces.Repositories;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Infrastructure.Factories;
using IARA.Mobile.Infrastructure.Repositories;
using IARA.Mobile.Infrastructure.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace IARA.Mobile.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCommonInfrastructure(this IServiceCollection services)
        {
            services.AddTransient<IDateTime, DateTimeUtility>();
            services.AddSingleton<IRestClient, RestClientUtility>();
            services.AddSingleton<IServerUrl, ServerUrlUtility>();
            services.AddSingleton<IFormDataFactory, FormDataFactory>();

            services.AddSingleton<ITLRepository, TLRepository>();

            return services;
        }
    }
}
