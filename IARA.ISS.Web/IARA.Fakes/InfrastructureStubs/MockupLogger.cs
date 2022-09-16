using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IARA.Logging.Abstractions.Interfaces;
using IARA.Logging.Abstractions.Models;
using Microsoft.Extensions.Logging;

namespace IARA.Fakes.InfrastructureStubs
{
    public class MockupLogger : IExtendedLogger
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
        }

        public List<int> Log(List<LogRecord> records)
        {
            return new List<int>();
        }

        public int Log(LogRecord record)
        {
            return -1;
        }

        public int LogException(Exception ex, [CallerFilePath] string callerFileName = null, [CallerMemberName] string callerName = null)
        {
            return -1;
        }

        public int LogException(string message, Exception ex, [CallerFilePath] string callerFileName = null, [CallerMemberName] string callerName = null)
        {
            return -1;
        }

        public int LogInfo(string message, [CallerFilePath] string callerFileName = null, [CallerMemberName] string callerName = null)
        {
            return -1;
        }

        public int LogWarning(string message, [CallerFilePath] string callerFileName = null, [CallerMemberName] string callerName = null)
        {
            return -1;
        }
    }
}
