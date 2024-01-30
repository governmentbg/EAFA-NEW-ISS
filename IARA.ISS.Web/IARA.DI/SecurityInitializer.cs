using IARA.DataAccess;
using IARA.EntityModels.Entities;
using IARA.Infrastructure.Services;
using IARA.Security.Interfaces;
using IARA.Security.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TL.AspNet.Security;
using TL.AspNet.Security.eAuth;

namespace SUVR.DI
{
    public static class SecurityInitializer
    {
        public static IServiceCollection AddSecurity(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddJwtAuthentication<int>(configuration, config =>
            {
                config.HubPaths = new string[] { "/notifications" };
                config.AllowedCookiePaths = new string[] { "/ReportServer" };
            })
            .AddRequestContext()
            .AddDefaultAuthServices<SecurityUserEntity, IARADbContext>()
            .AddUserPasswordManager<SecurityUserEntity, IARADbContext>()
            .AddEGovManager<int, SecurityUserEntity, IARADbContext>()
            .AddAuthorization<PermissionsService, string>()
            .AddSaltFormatter(salt => salt?.ToUpper().Substring(0, 5));

            services.AddScoped<IARA.Security.Permissions.IPermissionsService, PermissionsService>();
            services.AddScoped<ISecurityService, SecurityService>();
            services.AddScoped<ISecurityEmailSender, SecurityEmailSender>();

            return services;
        }
    }
}
