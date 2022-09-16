using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IARA.IdentityServer
{

    public class Startup : TechnoLogica.IdentityServer.Startup
    {
        public Startup(IWebHostEnvironment environment, IConfiguration config, ILoggerFactory loggerFactory)
                        : base(environment, config, loggerFactory)
        {

        }

        public override void ConfigureServices(IServiceCollection services)
        {
            base.ConfigureServices(services);
        }
    }
}
