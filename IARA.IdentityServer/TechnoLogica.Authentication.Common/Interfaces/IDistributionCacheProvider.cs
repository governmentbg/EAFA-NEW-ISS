using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TechnoLogica.Authentication.Common
{
    public interface IDistributionCacheProvider
    {
        void AddDistributedCache(IServiceCollection services, IConfiguration configuration);
    }
}
