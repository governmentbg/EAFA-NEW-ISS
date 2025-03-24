using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Shared.Utilities;
using IARA.Mobile.Shared.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace IARA.Mobile.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddShared(this IServiceCollection services)
        {
            services.AddTransient<IAuthTokenProvider, AuthTokenProviderUtility>();
            services.AddTransient<IExceptionHandler, ExceptionHandlerUtility>();
            services.AddTransient<IMobileInfo, MobileInfoUtility>();
            services.AddSingleton<IBackButton, BackButtonUtility>();
            services.AddSingleton<IDbSettings, DbSettingsUtility>();
            services.AddTransient<IPageVersion, PageVersionUtility>();
            services.AddTransient<IApplicationInstance, ApplicationInstanceUtility>();
            services.AddTransient(typeof(SystemInformationViewModel));
            services.AddTransient(typeof(ReportsViewModel));
            services.AddTransient(typeof(ReportViewModel));
            services.AddTransient<IAnalytics, AnalyticsUtility>();
            services.AddTransient(typeof(ChangePasswordViewModel));

            return services;
        }
    }
}
