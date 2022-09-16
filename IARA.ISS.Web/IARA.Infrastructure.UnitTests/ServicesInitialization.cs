using IARA.Common.Logging;
using IARA.Fakes.InfrastructureStubs;
using Microsoft.Extensions.DependencyInjection;

namespace IARA.Infrastructure.UnitTests
{
    public static class ServicesInitialization
    {
        public static void Initialize(IServiceCollection services)
        {
            services.AddScoped<IExtendedLogger, MockupLogger>();
            services.AddSingleton<IUserActionsAuditLogger, MockupUserActionsLogger>();
        }
    }
}
