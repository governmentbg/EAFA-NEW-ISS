using IARA.Mobile.Application.Interfaces.Factories;
using IARA.Mobile.Application.Interfaces.Utilities;
using IARA.Mobile.Insp.UWP;
using IARA.Mobile.Insp.UWP.Extensions;
using IdentityModel.OidcClient.Browser;
using Microsoft.Extensions.DependencyInjection;
using Xamarin.Forms;

[assembly: Dependency(typeof(PlatformSpecificServices))]

namespace IARA.Mobile.Insp.UWP
{
    public class PlatformSpecificServices : IPlatformSpecificServices
    {
        public void AddPlatformSpecificServices(IServiceCollection services)
        {
            services.AddTransient<IRestClientHandlerBuilder, RestClientHandlerCreation>();
            services.AddTransient<IDownloader, WindowsDownloader>();
            services.AddTransient<IBrowser, IdentityServerWebBrowser>();
        }
    }
}
