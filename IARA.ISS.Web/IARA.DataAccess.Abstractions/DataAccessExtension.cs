using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using IARA.Common;
using IARA.Common.Constants;
using IARA.EntityModels.Interfaces;
using IARA.Logging.Abstractions.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace IARA.DataAccess.Abstractions
{
    public static class DataAccessUtils
    {
        public static string AuditUserName
        {
            get
            {
                string auditUserName = Thread.CurrentPrincipal?.Identity?.Name;
                return !string.IsNullOrEmpty(auditUserName) ? auditUserName : DefaultConstants.SYSTEM_USER;
            }
        }

        public static Dictionary<string, string> GetTableAndEntityNames(this DbContext dbContext)
        {
            Type dbSetType = typeof(DbSet<>);

            List<Type> entityTypes = dbContext.GetType()
                                              .GetProperties()
                                              .Select(x => x.PropertyType)
                                              .Where(x => x.IsGenericType
                                                      && x.GetGenericTypeDefinition() == dbSetType)
                                              .Select(x => x.GetGenericArguments()[0])
                                              .ToList();

            Dictionary<string, string> tableEntityDict = new Dictionary<string, string>();
            foreach (var entityType in entityTypes)
            {
                var mapping = dbContext.Model.FindEntityType(entityType);

                string schema = mapping.GetSchema();
                string tableName = mapping.GetTableName();
                tableEntityDict.Add($"{schema}.{tableName}", $"{schema}.{entityType.Name}");
            }

            return tableEntityDict;
        }


        public static List<ChangedEntityModel> ApplyAudit(ChangeTracker changeTracker)
        {
            List<ChangedEntityModel> entities = new List<ChangedEntityModel>();

            foreach (EntityEntry entry in changeTracker.Entries().Where(t => t.State == EntityState.Added && t.Entity is IAuditEntity))
            {
                IAuditEntity audit = entry.Entity as IAuditEntity;
                audit.CreatedBy = AuditUserName;
                audit.CreatedOn = DateTime.Now;

                //Валидация трябва да се случва след set-ване на одит полетата
                var validationContext = new ValidationContext(entry.Entity);
                Validator.ValidateObject(entry.Entity, validationContext);
                entities.Add(new ChangedEntityModel
                {
                    Type = entry.Entity.GetType(),
                    State = entry.State,
                    EntityEntry = entry,
                    OldValues = entry.OriginalValues?.ToObject(),
                    NewValues = entry.CurrentValues?.ToObject()
                });
            }

            foreach (EntityEntry entry in changeTracker.Entries().Where(t => t.State == EntityState.Modified && t.Entity is IAuditEntity))
            {
                IAuditEntity entity = entry.Entity as IAuditEntity;
                entity.UpdatedBy = AuditUserName;
                entity.UpdatedOn = DateTime.Now;
                var validationContext = new ValidationContext(entry.Entity);
                Validator.ValidateObject(entry.Entity, validationContext);
                entities.Add(new ChangedEntityModel
                {
                    Type = entry.Entity.GetType(),
                    State = entry.State,
                    EntityEntry = entry,
                    OldValues = entry.OriginalValues?.ToObject(),
                    NewValues = entry.CurrentValues?.ToObject()
                });
            }

            foreach (EntityEntry entry in changeTracker.Entries().Where(t => t.State == EntityState.Deleted && t.Entity is IAuditEntity))
            {
                entities.Add(new ChangedEntityModel
                {
                    Type = entry.Entity.GetType(),
                    State = entry.State,
                    EntityEntry = entry,
                    OldValues = entry.OriginalValues?.ToObject(),
                    NewValues = entry.CurrentValues?.ToObject()
                });
            }

            return entities;
        }

        public static void ApplyComplexAudit(this IUserActionsAuditLogger userActionsAuditLogger, ScopedServiceProviderFactory scopedServiceProviderFactory, IEnumerable<ChangedEntityModel> entityEntries)
        {
            try
            {
                foreach (var entry in entityEntries)
                {
                    if (userActionsAuditLogger != null && userActionsAuditLogger.IsEntityAuditable(entry.Type))
                    {
                        var properties = entry.EntityEntry.Metadata.FindPrimaryKey().Properties;
                        string entityName = entry.Type.Name;
                        string primaryKeys = GetPrimaryKey(properties, entry.EntityEntry.CurrentValues.ToObject());
                        userActionsAuditLogger.ApplyComplexAudit(entityName, entry.NewValues, entry.OldValues, primaryKeys, entry.State.ToString(), AuditUserName);
                    }
                }
            }
            catch (Exception ex)
            {
                using IScopedServiceProvider scopedServiceProvider = scopedServiceProviderFactory.GetServiceProvider();
                IExtendedLogger logger = scopedServiceProvider.GetService<IExtendedLogger>();
                logger.LogException(ex);
            }
        }

        public static string GetPrimaryKey(IReadOnlyList<IProperty> properties, object entityEntry)
        {
            List<string> primaryKeyValues = new List<string>();

            foreach (var property in properties)
            {
                object value = entityEntry.GetType().GetProperty(property.Name).GetValue(entityEntry);
                if (value != null)
                {
                    primaryKeyValues.Add(value.ToString());
                }
                else
                {
                    primaryKeyValues.Add(string.Empty);
                }
            }

            return string.Join(",", primaryKeyValues);
        }
    }
}
