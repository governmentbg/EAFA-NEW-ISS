using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using IARA.Common;
using IARA.Common.ConfigModels;
using IARA.Common.Constants;
using IARA.DataAccess.Abstractions;
using IARA.EntityModels.Entities;
using IARA.Logging.Abstractions.Interfaces;
using IARA.Logging.Abstractions.Models;
using Microsoft.Extensions.Logging;

namespace IARA.Logging
{
    public class DatabaseLogger : BaseLogger, ILogger, IExtendedLogger
    {
        private ScopedServiceProviderFactory scopedServiceProviderFactory;
        private ITeamsLogger teamsLogger;

        public DatabaseLogger(ScopedServiceProviderFactory scopedServiceProviderFactory, LoggingSettings settings, ITeamsLogger teamsLogger)
            : base(settings)
        {
            this.teamsLogger = teamsLogger;
            this.scopedServiceProviderFactory = scopedServiceProviderFactory;
        }

        public DatabaseLogger(ScopedServiceProviderFactory scopedServiceProviderFactory, LoggingSettings settings, ITeamsLogger teamsLogger, [CallerFilePath] string categoryName = null)
                 : this(scopedServiceProviderFactory, settings, teamsLogger)
        {
            if (!string.IsNullOrEmpty(categoryName))
            {
                var fileInfo = new FileInfo(categoryName);
                this.categoryName = fileInfo.Name.Replace(fileInfo.Extension, string.Empty);
            }
        }

        private static ErrorLog AddToLog(ILoggingDbContext db, LogRecord record)
        {
            var log = new ErrorLog
            {
                Severity = record.Level.ToString(),
                Message = record.Message,
                LogDate = record.LogDate.HasValue ? record.LogDate.Value : DateTime.Now,
                StackTrace = record.StackTrace,
                ExceptionSource = record.ExceptionSource,
                Username = !string.IsNullOrEmpty(record.Username) ? record.Username : DefaultConstants.SYSTEM_USER,
                Client = record.Client ?? "INTERNAL",
                Class = record.CallerFileName ?? "UNKNOWN",
                Method = record.CallerName ?? "UNKNOWN",
            };

            db.ErrorLogs.Add(log);

            return log;
        }

        protected override int LogToDataBase(LogRecord record)
        {
            using var serviceProvider = scopedServiceProviderFactory.GetServiceProvider();
            using ILoggingDbContext db = serviceProvider.GetService<ILoggingDbContext>();
            var log = AddToLog(db, record);

            if (!ExeptionSeenRecently(db, log, EXCEPTION_LAST_SEEN_HOURS))
            {
                this.teamsLogger.Log(record);
            }

            db.SaveChanges();

            return log.Id;
        }

        protected override List<int> LogToDataBase(List<LogRecord> records)
        {
            using var serviceProvider = scopedServiceProviderFactory.GetServiceProvider();
            using ILoggingDbContext db = serviceProvider.GetService<ILoggingDbContext>();

            List<int> ids = new List<int>(records.Count);

            foreach (var record in records)
            {
                var log = AddToLog(db, record);
                ids.Add(log.Id);
            }

            db.SaveChanges();

            return ids;
        }

        private bool ExeptionSeenRecently(ILoggingDbContext db, ErrorLog log, ushort hoursFromNow)
        {
            DateTime periodStart = DateTime.Now.AddHours(-hoursFromNow);

            bool result = (from errorLog in db.ErrorLogs
                           where errorLog.LogDate >= periodStart
                                  && errorLog.Class == log.Class
                                  && errorLog.Client == log.Client
                                  && errorLog.Method == log.Method
                                  && errorLog.ExceptionSource == log.ExceptionSource
                           select errorLog.Id).Any();

            return result;
        }

    }
}
