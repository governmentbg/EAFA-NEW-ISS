using IdentityServer4.Configuration.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace TechnoLogica.Authentication.Common
{
    public interface IOperationalStore
    {
        void AddOperationalStore(IIdentityServerBuilder builder, IConfiguration configuration);
    }
}
