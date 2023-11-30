using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TechnoLogica.Authentication.Common
{
    public interface IAuthenticationProfile
    {
        void Configure(IServiceCollection services, IConfiguration configuration);
    }
}
