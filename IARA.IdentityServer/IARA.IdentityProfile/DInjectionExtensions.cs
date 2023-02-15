using IARA.EntityModels.Entities;
using IARA.Security.AuthContext;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TL.Logging;

namespace IARA.IdentityProfile
{
    internal static class DInjectionExtensions
    {
        public static void AddLocalLogging(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCustomErrorLogging<ErrorLog, AuthDbContext>(configuration);
            services.AddComplexAuditLogging<AuditLog, NauditLogTable, string, AuthDbContext>();
        }
    }
}
