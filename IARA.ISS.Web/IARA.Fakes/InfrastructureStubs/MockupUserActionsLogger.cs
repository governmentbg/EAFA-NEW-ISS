using System;
using System.Collections.Generic;
using IARA.Logging.Abstractions.Interfaces;

namespace IARA.Fakes.InfrastructureStubs
{
    public class MockupUserActionsLogger : IUserActionsAuditLogger
    {
        public void ApplyComplexAudit(string entityName, object currentEntry, object oldEntry, string primaryKeys, string actionType, string auditUserName)
        {

        }

        public IReadOnlyList<string> GetAuditableEntities()
        {
            return new List<string>();
        }

        public bool IsEntityAuditable(Type entityType)
        {
            return false;
        }

        public void RefreshAuditEntities()
        {
        }
    }
}
