using System.Collections.Generic;
using IARA.EntityModels.Entities;
using Microsoft.EntityFrameworkCore;

namespace IARA.DataAccess.Abstractions
{
    public interface ILoggingDbContext : IBaseDbContext
    {
        DbSet<ErrorLog> ErrorLogs { get; }
        DbSet<AuditLog> AuditLogs { get; }
        DbSet<NauditLogActionType> NauditLogActionTypes { get; }
        DbSet<NauditLogTable> NauditLogTables { get; }
        Dictionary<string, string> GetTableAndEntityNames();
    }
}
