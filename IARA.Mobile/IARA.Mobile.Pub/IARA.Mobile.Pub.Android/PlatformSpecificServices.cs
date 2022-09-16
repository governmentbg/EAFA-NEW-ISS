using IARA.Mobile.Application.Interfaces.Factories;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Pub.Application.Interfaces.Utilities;
using IARA.Mobile.Pub.Droid;
using IARA.Mobile.Pub.Droid.Extensions;
using IARA.Mobile.Pub.Droid.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;

[assembly: Dependency(typeof(PlatformSpecificServices))]

namespace IARA.Mobile.Pub.Droid
{
    public class PlatformSpecificServices : IPlatformSpecificServices
    {
        public void AddPlatformSpecificServices(IServiceCollection services)
        {
            services.AddTransient<IRestClientHandlerBuilder, RestClientHandlerCreation>();
            services.AddTransient<INotificationManager, AndroidNotification>();
            ////services.AddTransient<IBrowser, ChromeCustomTabsBrowser>();
            //services.AddTransient<IBrowser, WebAuthenticatorBrowser>();

            services.AddTransient<IDownloader, AndroidDownloader>();
        }
    }
}
