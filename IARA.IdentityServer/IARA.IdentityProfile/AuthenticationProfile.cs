using System.ComponentModel.Composition;
using IARA.Common.ConfigModels;
using IARA.Common.Utils;
using IARA.DataAccess.Abstractions;
using IARA.Security.AuthContext;
using IARA.Security.Interfaces;
using IARA.Security.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TechnoLogica.Authentication.Common;
using TL.Dependency.Injection;
using TL.Logging.Abstractions.Interfaces;

namespace IARA.IdentityProfile
{
    [Export(typeof(IAuthenticationProfile))]
    public class AuthenticationProfile : IAuthenticationProfile
    {
        public void Configure(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AuthDbContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("connectionString"));
            });

            services.AddScoped<IUsersDbContext, AuthDbContext>();

            services.AddSingleton(EmailClientSettings.ReadSettings(configuration));

            services.AddScoped<IEmailClient, EmailClient>(serviceProvider =>
            {
                EmailClientSettings settings = serviceProvider.GetRequiredService<EmailClientSettings>();
                IExtendedLogger logger = serviceProvider.GetRequiredService<IExtendedLogger>();
                return new EmailClient(settings.Host, settings.Port, logger);
            });

            services.AddScoped<IProfileClientService, InspectorsMobileProfileClientService>();
            services.AddScoped<IProfileClientService, FishersMobileProfileClientService>();
            services.AddScoped<IProfileClientService, PublicWebProfileClientService>();
            services.AddScoped<IProfileClientService, InternalWebProfileClientService>();

            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<PasswordHasher>();
            services.AddScopedServiceFactory();

            services.AddHttpContextAccessor();

            services.AddSingleton<SecurityEmailSender>();
            services.AddSingleton(LoggingSettings.ReadSettings(configuration));
            services.AddLocalLogging();
            //services.AddEmailQueueSender();
        }
    }
}
