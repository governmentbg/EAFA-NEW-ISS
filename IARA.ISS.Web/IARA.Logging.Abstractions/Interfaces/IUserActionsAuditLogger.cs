using System;
using System.Collections.Generic;

namespace IARA.Logging.Abstractions.Interfaces
{
    public interface IUserActionsAuditLogger
    {
        bool IsEntityAuditable(Type entityType);

        void RefreshAuditEntities();

        IReadOnlyList<string> GetAuditableEntities();

        void ApplyComplexAudit(string entityName, object currentEntry, object oldEntry, string primaryKeys, string actionType, string auditUserName);
    }
}
