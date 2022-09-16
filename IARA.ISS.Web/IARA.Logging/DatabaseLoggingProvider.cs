using System;
using System.Collections.Concurrent;
using IARA.Common;
using IARA.Common.ConfigModels;
using IARA.Logging.Abstractions.Interfaces;
using Microsoft.Extensions.Logging;

namespace IARA.Logging
{
    public class DatabaseLoggingProvider : IDatabaseLoggerProvider
    {
        private ScopedServiceProviderFactory serviceProviderFactory;
        private IScopedServiceProvider scopedServiceProvider;
        private readonly ConcurrentDictionary<string, DatabaseLogger> loggers;
        public DatabaseLoggingProvider(ScopedServiceProviderFactory serviceProviderFactory)
        {
            this.serviceProviderFactory = serviceProviderFactory;
            this.loggers = new ConcurrentDictionary<string, DatabaseLogger>();
            this.scopedServiceProvider = serviceProviderFactory.GetServiceProvider();
        }

        ILogger ILoggerProvider.CreateLogger(string categoryName)
        {

            return loggers.GetOrAdd(categoryName, GetLogger);
        }

        private DatabaseLogger GetLogger(string categoryName)
        {
            ITeamsLogger teamsLogger = scopedServiceProvider.GetService<ITeamsLogger>();
            LoggingSettings settings = scopedServiceProvider.GetService<LoggingSettings>();
            return new DatabaseLogger(serviceProviderFactory, settings, teamsLogger, categoryName);
        }

        void IDisposable.Dispose()
        {
            serviceProviderFactory = null;
        }
    }
}
