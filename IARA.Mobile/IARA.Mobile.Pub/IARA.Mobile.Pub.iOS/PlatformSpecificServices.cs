using IARA.Mobile.Application.Interfaces.Factories;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Pub.Application.Interfaces.Utilities;
using IARA.Mobile.Pub.iOS;
using IARA.Mobile.Pub.iOS.Extensions;
using IARA.Mobile.Pub.iOS.Utilities;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;

[assembly: Dependency(typeof(PlatformSpecificServices))]
namespace IARA.Mobile.Pub.iOS
{
    public class PlatformSpecificServices : IPlatformSpecificServices
    {
        public void AddPlatformSpecificServices(IServiceCollection services)
        {
            services.AddTransient<IRestClientHandlerBuilder, RestClientHandlerCreation>();
            services.AddTransient<IDownloader, IosDownloader>();
            services.AddTransient<INotificationManager, IosNotification>();
        }
    }
}
