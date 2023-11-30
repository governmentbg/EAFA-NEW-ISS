

using IARA.IdentityProfile;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TechnoLogica.Authentication.Common;
using TechnoLogica.Authentication.Common.Models;
using TechnoLogica.Authentication.EAuth;
using TechnoLogica.IdentityServer;
using TechnoLogica.IdentityServer.PostgreSQLOperationalStore;

namespace IARA.IdentityServer
{

    public class Startup : BaseStartup
    {
        public Startup(IWebHostEnvironment environment, IConfiguration config, ILoggerFactory loggerFactory)
                        : base(environment, config, loggerFactory)
        { }

        public void ConfigureServices(IServiceCollection services)
        {
            base.BaseConfigureServices(services, this.Configuration.GetConnectionString("connectionString"));
        }

        public override void Configure(IApplicationBuilder app)
        {
            base.Configure(app);
        }

        protected override IIdentityServerBuilder AddAdditionalConfiguration(IIdentityServerBuilder builder,
                                                                             OperationalStore operationalStore,
                                                                             string connectionString)
        {
            builder.AddPosgreOperationalStore(operationalStore, connectionString);

            return builder;
        }

        protected override void AddAuthenticationProfiles(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthenticationProfiles(configuration);
        }

        protected override void AddAuthenticationProviders(IServiceCollection services)
        {
            services.AddAuthenticationProviders(Configuration).AddProvider<EAuthAuthenticationProvider>();
        }

        protected override void AddPersistance(IDataProtectionBuilder dataProtectionBuilder, string connectionString, ILoggerFactory loggerFactory)
        {
            dataProtectionBuilder.AddPostgrePersistance(connectionString, loggerFactory);
        }
    }
}
