using Microsoft.Extensions.DependencyInjection;

namespace IARA.Mobile.Application.Interfaces.Utilities
{
    public interface IPlatformSpecificServices
    {
        /// <summary>
        /// Platform specific things that are added to the service collection
        /// </summary>
        void AddPlatformSpecificServices(IServiceCollection services);
    }
}
