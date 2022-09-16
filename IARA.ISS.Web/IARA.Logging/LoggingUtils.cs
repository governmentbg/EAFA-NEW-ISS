using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace IARA.Logging
{
    internal static class LoggingUtils
    {
        public const string BEGIN_MESSAGE = "---BEGIN MESSAGE---";
        public const string END_MESSAGE = "---END MESSAGE---";
        public const string BEGIN_STACKTRACE = "---BEGIN STACKTRACE---";
        public const string END_STACKTRACE = "---END STACKTRACE---";

        public const string DefaultNamespacePrefix = "IARA";

        public static readonly LogLevel[] AllowedLogLevels = new LogLevel[] {
            LogLevel.Debug,
            LogLevel.Information,
            LogLevel.Warning,
            LogLevel.Error
        };

        public static bool IsEnabled(LogLevel messageLogLevel, LogLevel systemLogLevel)
        {
            return AllowedLogLevels.Contains(messageLogLevel)
                 && systemLogLevel != LogLevel.None
                 && messageLogLevel >= systemLogLevel;
        }

        public static string FilterStackTrace(string stackTrace)
        {
            if (string.IsNullOrEmpty(stackTrace))
                return null;

            List<string> stackSplit = new List<string>(stackTrace.Split(new[] { "at " }, StringSplitOptions.None));

            stackSplit.RemoveAll(x => !x.StartsWith(DefaultNamespacePrefix) && !x.StartsWith("System"));

            if (stackSplit.Count > 0)
            {
                string result = " at " + string.Join(" at ", stackSplit.ToArray());
                result = result.Replace(Environment.NewLine, " ").Replace("\\", "\\\\").Replace("_", "\\_");
                result = result.Replace(BEGIN_STACKTRACE, "").Replace(END_STACKTRACE, "");

                return result;
            }
            else
            {
                return null;
            }
        }
    }
}
