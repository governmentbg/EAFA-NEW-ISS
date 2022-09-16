using IARA.Common;
using IARA.Common.ConfigModels;
using IARA.Logging.Abstractions;
using IARA.Logging.Abstractions.Interfaces;
using Microsoft.Extensions.Logging;

namespace IARA.Logging
{
    public class DatabaseLogger<T> : DatabaseLogger, ILogger<T>, IExtendedLogger<T>
    {
        public DatabaseLogger(ScopedServiceProviderFactory scopedServiceProviderFactory, LoggingSettings settings, ITeamsLogger teamsLogger)
            : base(scopedServiceProviderFactory, settings, teamsLogger)
        {
            this.categoryName = typeof(T).Name;
        }
    }
}
