using IARA.Mobile.Application.Interfaces.Factories;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Insp.Droid;
using IARA.Mobile.Insp.Droid.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;

[assembly: Dependency(typeof(PlatformSpecificServices))]

namespace IARA.Mobile.Insp.Droid
{
    public class PlatformSpecificServices : IPlatformSpecificServices
    {
        public void AddPlatformSpecificServices(IServiceCollection services)
        {
            services.AddTransient<IRestClientHandlerBuilder, RestClientHandlerCreation>();
            services.AddTransient<IDownloader, AndroidDownloader>();
        }
    }
}
