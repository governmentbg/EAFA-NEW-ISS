using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace IARA.Common.ConfigModels
{
    public class LoggingLevels
    {
        public LogLevel Database { get; set; }
        public LogLevel Teams { get; set; }
    }

    public class LoggingSettings
    {
        public static LoggingSettings Default { get; private set; }

        public LoggingLevels LogLevel { get; set; }
        public bool ActivityLoggingEnabled { get; set; }
        public string TeamsWebHookUrl { get; set; }

        public static LoggingSettings ReadSettings(IConfiguration configuration)
        {
            var logLevelSection = configuration.GetSection("Logging:LogLevel");
            var loggingSection = configuration.GetSection("Logging");
            Default = new LoggingSettings
            {
                LogLevel = new LoggingLevels
                {
                    Database = logLevelSection.GetValue(nameof(LoggingLevels.Database), Microsoft.Extensions.Logging.LogLevel.Error),
                    Teams = logLevelSection.GetValue(nameof(LoggingLevels.Teams), Microsoft.Extensions.Logging.LogLevel.Error)
                },
                TeamsWebHookUrl = loggingSection.GetValue<string>(nameof(TeamsWebHookUrl))
            };

            return Default;
        }
    }
}
