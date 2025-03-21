using System;
using System.Reflection;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Pub.Application.Interfaces.Utilities;
using IARA.Mobile.Pub.Utilities;
using IARA.Mobile.Shared.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace IARA.Mobile.Pub
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMain(this IServiceCollection services)
        {
            services.AddTransient<IOfflineFiles, FilesUtility>();
            services.AddTransient<ISettings, SettingsUtility>();
            services.AddTransient<ICommonLogout, CommonLogoutUtility>();
            services.AddSingleton<IPopUp, PopUpUtility>();
            services.AddTransient<IAuthenticationProvider, AuthenticationProvider>();
            services.AddTransient<ICurrentUser, CurrentUserUtility>();
            services.AddTransient<ICommonNavigator, CommonNavigatorUtility>();
            services.AddTransient<INomenclatureDates, NomenclatureDatesUtility>();
            services.AddTransient<INomenclatureDatesClear, NomenclatureDatesUtility>();
            services.AddTransient<IFishingTicketsSettings, FishingTicketsUtility>();
            services.AddSingleton<IConnectivity, ConnectivityUtility>();
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
