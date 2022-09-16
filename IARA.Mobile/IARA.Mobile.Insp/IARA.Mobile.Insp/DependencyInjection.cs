using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Insp.Application.Interfaces.Utilities;
using IARA.Mobile.Insp.Utilities;
using IARA.Mobile.Shared.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;

namespace IARA.Mobile.Insp
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMain(this IServiceCollection services)
        {
            services.AddSingleton<IMessagingCenter, MessagingCenterUtility>();
            services.AddTransient<ICommonLogout, CommonLogoutUtility>();
            services.AddTransient<IOfflineFiles, OfflineFilesUtility>();
            services.AddTransient<IAuthenticationProvider, AuthenticationProvider>();
            services.AddTransient<ISettings, SettingsUtility>();
            services.AddTransient<ICurrentUser, CurrentUserUtility>();
            services.AddTransient<ICommonNavigator, CommonNavigatorUtility>();
            services.AddTransient<INomenclatureDates, NomenclatureDatesUtility>();
            services.AddTransient<INomenclatureDatesClear, NomenclatureDatesUtility>();
            services.AddSingleton<IPopUp, PopUpUtility>();
            services.AddSingleton<IConnectivity, ConnectivityUtility>();
            services.AddTransient<IIdentityServerConfiguration, IdentityServerConfiguration>
                ((_) => new IdentityServerConfiguration("inspectors-mobile-client", "xamarinformsclients://inspectors-callback"));
            services.AddSingleton<ITranslator, TranslatorUtility>();
            services.AddTransient<ISystemInformationProvider, SystemInformationUtility>();

            foreach (Type type in Assembly.GetExecutingAssembly().GetExportedTypes())
            {
                if (type.Name.EndsWith("ViewModel") && type.IsClass && !type.IsAbstract)
                {
                    services.AddTransient(type);
                }
            }

            return services;
        }
    }
}
