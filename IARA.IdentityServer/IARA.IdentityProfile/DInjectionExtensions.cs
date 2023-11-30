using IARA.EntityModels.Entities;
using IARA.Security.AuthContext;
using Microsoft.Extensions.DependencyInjection;
using TL.Logging;

namespace IARA.IdentityProfile
{
    internal static class DInjectionExtensions
    {
        public static void AddLocalLogging(this IServiceCollection services)
        {
            services.AddComplexAuditLogging<AuditLog, string, AuthDbContext>();
        }
    }
}
