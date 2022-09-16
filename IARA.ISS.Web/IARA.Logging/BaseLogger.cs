using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using IARA.Common.ConfigModels;
using IARA.Common.Constants;
using IARA.Logging.Abstractions.Models;
using Microsoft.Extensions.Logging;

namespace IARA.Logging
{
    public abstract class BaseLogger
    {
        protected BaseLogger(LoggingSettings loggingSettings)
        {
            this.loggingSettings = loggingSettings;
        }

        private LoggingSettings loggingSettings;
        private ConcurrentDictionary<string, object> filePadlocks = new ConcurrentDictionary<string, object>();
        protected string categoryName;

        protected internal const ushort EXCEPTION_LAST_SEEN_HOURS = 24;

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return LoggingUtils.IsEnabled(logLevel, loggingSettings.LogLevel.Database);
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            Log(new LogRecord
            {
                Level = logLevel,
                Message = formatter(state, exception),
                CallerName = eventId.Name,
                CallerFileName = eventId.Name,
                StackTrace = exception?.StackTrace,
                ExceptionSource = FormatSource(exception?.Source),
                Username = Thread.CurrentPrincipal?.Identity?.Name ?? DefaultConstants.SYSTEM_USER
            });
        }

        public int Log(LogRecord record)
        {
            if ((categoryName == null || !categoryName.StartsWith("Microsoft.EntityFrameworkCore")) && this.IsEnabled(record.Level))
            {
                try
                {
                    return LogToDataBase(record);
                }
                catch (Exception)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append(record.ToString());
                    LogInFile(builder);
                }
            }

            return -1;
        }

        protected abstract int LogToDataBase(LogRecord record);
        protected abstract List<int> LogToDataBase(List<LogRecord> records);

        public List<int> Log(List<LogRecord> records)
        {
            if (records != null)
            {
                try
                {
                    return LogToDataBase(records);
                }
                catch (Exception)
                {
                    StringBuilder builder = new StringBuilder();

                    foreach (var record in records)
                    {
                        builder.AppendLine("-----BEGIN LOG------");
                        builder.Append(record.ToString());
                        builder.AppendLine("-----END LOG------");
                    }

                    LogInFile(builder);
                }
            }

            return new List<int>();
        }

        public int LogException(Exception ex, [CallerFilePath] string callerFileName = null, [CallerMemberName] string callerName = null)
        {
            Stack<string> exceptionMessages = new Stack<string>();
            Stack<string> stackTraceStack = new Stack<string>();
            StringBuilder messageBuilder = new StringBuilder();
            StringBuilder stackBuilder = new StringBuilder();

            exceptionMessages.Push(ex.Message);
            stackTraceStack.Push(ex.StackTrace);

            while (ex != null && ex.InnerException != null)
            {
                exceptionMessages.Push(ex.InnerException.Message);
                stackTraceStack.Push(ex.InnerException.StackTrace);
                ex = ex.InnerException;
            }

            if (exceptionMessages.Count == 1 && exceptionMessages.TryPop(out string message))
            {
                messageBuilder.AppendLine(message);
            }
            else
            {
                while (exceptionMessages.TryPop(out message))
                {
                    messageBuilder.AppendLine(LoggingUtils.BEGIN_MESSAGE);
                    messageBuilder.AppendLine(message);
                    messageBuilder.AppendLine(LoggingUtils.END_MESSAGE);
                }
            }

            if (exceptionMessages.Count == 1 && stackTraceStack.TryPop(out string stacktrace))
            {
                stackBuilder.AppendLine(stacktrace);
            }
            else
            {
                while (stackTraceStack.TryPop(out stacktrace))
                {
                    stackBuilder.AppendLine(LoggingUtils.BEGIN_STACKTRACE);
                    stackBuilder.AppendLine(stacktrace);
                    stackBuilder.AppendLine(LoggingUtils.END_STACKTRACE);
                }
            }

            return this.Log(new LogRecord
            {
                Level = LogLevel.Error,
                Message = messageBuilder.ToString(),
                CallerFileName = callerFileName,
                CallerName = callerName,
                StackTrace = stackBuilder.ToString(),
                ExceptionSource = FormatSource(ex.Source),
                Username = Thread.CurrentPrincipal?.Identity?.Name ?? DefaultConstants.SYSTEM_USER,
            });
        }

        public int LogException(string message, Exception ex, [CallerFilePath] string callerFileName = null, [CallerMemberName] string callerName = null)
        {
            Exception outerEx = new Exception(message, ex);
            return LogException(outerEx, callerFileName, callerName);
        }

        public int LogInfo(string message, [CallerFilePath] string callerFileName = null, [CallerMemberName] string callerName = null)
        {
            return this.Log(new LogRecord
            {
                Level = LogLevel.Information,
                Message = message,
                CallerFileName = callerFileName,
                CallerName = callerName,
                Username = Thread.CurrentPrincipal?.Identity?.Name ?? DefaultConstants.SYSTEM_USER,
            });
        }

        public int LogWarning(string message, [CallerFilePath] string callerFileName = null, [CallerMemberName] string callerName = null)
        {
            return this.Log(new LogRecord
            {
                Level = LogLevel.Warning,
                Message = message,
                CallerFileName = callerFileName,
                CallerName = callerName,
                Username = Thread.CurrentPrincipal?.Identity?.Name ?? DefaultConstants.SYSTEM_USER,
            });
        }

        private string FormatSource(string source)
        {
            if (!string.IsNullOrEmpty(source))
            {
                string[] paths = source.Replace("\\", "/").Split('/');
                List<string> parts = new List<string>(paths.Length);

                bool start = false;

                foreach (var path in paths)
                {
                    if (path.StartsWith("IARA"))
                    {
                        start = true;
                        parts.Add(path);
                    }
                    else if (start)
                    {
                        parts.Add(path);
                    }
                }

                return string.Format("/", parts);
            }
            else
            {
                return source;
            }
        }

        private void LogInFile(StringBuilder builder)
        {
            string dirFullName = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
            if (!Directory.Exists(dirFullName))
            {
                Directory.CreateDirectory(dirFullName);
            }

            string fileName = $"ErrorLog-{DateTime.Now.Date:yyyy-MM-dd}.txt";

            object padlock = filePadlocks.GetOrAdd(fileName, new object());
            lock (padlock)
            {
                using (FileStream fileStream = new FileStream(Path.Combine(dirFullName, fileName), FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read))
                {
                    using (StreamWriter writer = new StreamWriter(fileStream))
                    {
                        if (fileStream.Length > 0)
                        {
                            fileStream.Seek(0, SeekOrigin.End);
                        }

                        writer.WriteLine(builder);
                        writer.Flush();
                        writer.Close();
                    }
                }

                filePadlocks.TryRemove(fileName, out _);
            }
        }


    }
}
