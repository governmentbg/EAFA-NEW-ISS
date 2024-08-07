﻿using IARA.Logging.Abstractions.Interfaces;
using IARA.Logging.TeamsLogging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace IARA.Logging
{
    public static class LoggingInitializer
    {
        public static void AddCustomLogging(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerProvider, DatabaseLoggingProvider>();
            services.AddSingleton<IUserActionsAuditLogger, UserActionsAuditLogger>();
            services.AddSingleton<IDatabaseLoggerProvider, DatabaseLoggingProvider>();
            services.AddSingleton<ITeamsLogger, TeamsLogger>();
            services.AddSingleton<IExtendedLogger, DatabaseLogger>();
            services.AddSingleton<ILogger, DatabaseLogger>();
            services.AddSingleton(typeof(ILogger<>), typeof(DatabaseLogger<>));
            services.AddSingleton(typeof(IExtendedLogger<>), typeof(DatabaseLogger<>));
            services.AddSingleton(typeof(IExtendedLogger<>), typeof(DatabaseLogger<>));
        }
    }
}
