using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using IARA.Common;
using IARA.Common.Constants;
using IARA.DataAccess.Abstractions;
using IARA.EntityModels.Entities;
using IARA.Logging.Abstractions.Interfaces;
using IARA.Logging.Abstractions.Models;
using TL.BatchWorkers;
using TL.BatchWorkers.Interfaces;
using TL.BatchWorkers.Models.Parameters.AsyncWorker;

namespace IARA.Logging
{
    public class UserActionsAuditLogger : IUserActionsAuditLogger
    {
        private const string UNKNOWN = nameof(UNKNOWN);

        private readonly IAsyncWorkerTaskQueue<AuditRecord, bool> auditLogQueue;
        private List<string> auditEntityNames;
        private object auditEntityPadlock;
        private Task backgroundTask;
        private ManualResetEvent backgroundTaskMutex;
        private CancellationTokenSource cancellationTokenSource;
        private volatile bool isTaskRunning;
        private object padlock;
        private object queuePadlock;

        private ManualResetEvent refreshTaskMutex;

        private ScopedServiceProviderFactory scopedServiceProviderFactory;

        public UserActionsAuditLogger(ScopedServiceProviderFactory scopedServiceProviderFactory)
        {
            this.scopedServiceProviderFactory = scopedServiceProviderFactory;
            this.queuePadlock = new object();
            this.isTaskRunning = false;
            this.backgroundTaskMutex = new ManualResetEvent(false);
            this.refreshTaskMutex = new ManualResetEvent(false);
            this.cancellationTokenSource = new CancellationTokenSource();
            this.padlock = new object();
            this.auditEntityPadlock = new object();
            this.auditLogQueue = AsyncWorkerQueueBuilder.CreateInMemoryWorker(new LocalAsyncWorkerSettings<AuditRecord, bool>(WriteDownAuditRecords));
        }

        protected List<string> AuditEntityNames
        {
            get
            {
                //lock (auditEntityPadlock)
                {
                    return auditEntityNames;
                }
            }
            set
            {
                //lock (auditEntityPadlock)
                {
                    auditEntityNames = value;
                }
            }
        }

        public void ApplyComplexAudit(string entityName, object currentEntry, object oldEntry, string primaryKeys, string actionType, string auditUserName)
        {
            try
            {
                RequestData requestData = FillRequestData();
                if (requestData != null)
                {
                    AuditRecord auditRecord = new AuditRecord
                    {
                        CreatedOn = DateTime.Now,
                        Endpoint = requestData.Endpoint,
                        IPAddress = requestData.IPAddress,
                        NewValues = currentEntry,
                        OldValues = oldEntry,
                        PrimaryKeys = primaryKeys,
                        UserName = auditUserName,
                        ActionType = actionType,
                        EntityName = entityName,
                        BrowserInfo = requestData.BrowserInfo
                    };

                    AddEntryToLog(auditRecord);
                }
            }
            catch (Exception ex)
            {
                using IScopedServiceProvider provider = scopedServiceProviderFactory.GetServiceProvider();

                IExtendedLogger logger = provider.GetRequiredService<IExtendedLogger>();
                logger.LogException(ex);
            }
        }

        public IReadOnlyList<string> GetAuditableEntities()
        {
            if (AuditEntityNames == null)
            {
                if (!Monitor.IsEntered(padlock))
                {
                    RefreshAuditEntities();
                }
                else
                {
                    refreshTaskMutex.WaitOne();
                }

                return AuditEntityNames;
            }
            else
            {
                return AuditEntityNames;
            }
        }

        public bool IsEntityAuditable(Type entityType)
        {
            string schema = entityType.GetCustomAttribute<TableAttribute>()?.Schema;
            string entityName = $"{schema}.{entityType.Name}";

            return GetAuditableEntities().Contains(entityName);
        }

        public void RefreshAuditEntities()
        {
            if (Monitor.TryEnter(padlock))
            {
                refreshTaskMutex.Set();
                try
                {
                    using IScopedServiceProvider serviceProvider = scopedServiceProviderFactory.GetServiceProvider();
                    using ILoggingDbContext dbContext = serviceProvider.GetService<ILoggingDbContext>();

                    Dictionary<string, string> tableNames = dbContext.GetTableAndEntityNames();

                    AuditEntityNames = dbContext
                                            .NauditLogTables
                                            .Where(x => x.IsActive
                                                     && x.IsAuditLogEnabled == true)
                                            .Select(x => $"{x.SchemaName}.{x.TableName}")
                                            .ToList();

                    List<string> auditEntities = new List<string>();

                    foreach (var tableName in AuditEntityNames)
                    {
                        if (tableNames.ContainsKey(tableName))
                        {
                            auditEntities.Add(tableNames[tableName]);
                        }
                    }

                    this.AuditEntityNames = auditEntities;
                }
                finally
                {
                    Monitor.Exit(padlock);
                    refreshTaskMutex.Reset();
                }
            }
        }

        private Task<bool> AddEntryToLog(AuditRecord record)
        {
            return auditLogQueue.Enqueue(record);
        }

        private RequestData FillRequestData()
        {
            if (Thread.CurrentPrincipal != null && Thread.CurrentPrincipal.Identity != null && Thread.CurrentPrincipal.Identity is ClaimsIdentity)
            {
                var claimsIdentity = (ClaimsIdentity)Thread.CurrentPrincipal.Identity;
                string ipAddress = claimsIdentity.Claims.Where(x => x.Type == DefaultConstants.IP).Select(x => x.Value).FirstOrDefault();
                string endpoint = claimsIdentity.Claims.Where(x => x.Type == DefaultConstants.ENDPOINT).Select(x => x.Value).FirstOrDefault();
                string browserInfo = claimsIdentity.Claims.Where(x => x.Type == DefaultConstants.BROWSER_INFO).Select(x => x.Value).FirstOrDefault();

                return new RequestData
                {
                    BrowserInfo = browserInfo,
                    Endpoint = endpoint,
                    IPAddress = ipAddress
                };
            }
            else
            {
                return null;
            }
        }

        private string SerializeObject(Type type, object value)
        {
            MethodInfo serializeMethod = typeof(JsonSerializer).GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Where(x => x.Name == nameof(JsonSerializer.Serialize) && x.IsGenericMethod && x.GetParameters().Count() == 2).First();

            return serializeMethod.MakeGenericMethod(type)
                  .Invoke(null, new object[] { value, null }) as string;
        }

        private Task<bool> WriteDownAuditRecords(AuditRecord record, CancellationToken token)
        {
            using IScopedServiceProvider serviceProvider = scopedServiceProviderFactory.GetServiceProvider();

            try
            {
                ILoggingDbContext dbContext = serviceProvider.GetService<ILoggingDbContext>();

                Type recordType = record.NewValues.GetType();

                string newValues = record.NewValues != null ? SerializeObject(recordType, record.NewValues) : null;
                string oldValues = record.OldValues != null ? SerializeObject(recordType, record.OldValues) : null;

                string action, application;

                if (!string.IsNullOrEmpty(record.Endpoint))
                {
                    string[] urlSections = record.Endpoint.Split('/', StringSplitOptions.RemoveEmptyEntries);
                    action = urlSections[urlSections.Length - 1];
                    application = string.Join('/', urlSections.Take(urlSections.Length - 1));
                }
                else
                {
                    action = UNKNOWN;
                    application = UNKNOWN;
                }

                var logRecord = new AuditLog
                {
                    NewValue = newValues,
                    OldValue = oldValues,
                    Action = action,
                    Ipaddress = record.IPAddress ?? "SYSTEM",
                    LogDate = record.CreatedOn,
                    TableId = record.PrimaryKeys,
                    TableName = record.EntityName,
                    BrowserInfo = record.BrowserInfo ?? "SYSTEM",
                    Username = record.UserName,
                    ActionType = record.ActionType,
                    Application = application,
                    SchemaName = recordType.GetCustomAttribute<TableAttribute>()?.Schema ?? UNKNOWN
                };

                dbContext.AuditLogs.Add(logRecord);

                dbContext.SaveChanges();

                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                serviceProvider.GetService<IExtendedLogger>().LogException(ex);
                return Task.FromResult(false);
            }
        }
    }
}
