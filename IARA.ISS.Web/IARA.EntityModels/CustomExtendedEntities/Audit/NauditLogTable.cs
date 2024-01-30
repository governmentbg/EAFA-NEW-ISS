using System.ComponentModel.DataAnnotations.Schema;
using TL.Logging.Abstractions.Interfaces.Models.Audit;

namespace IARA.EntityModels.Entities
{
    public partial class NauditLogTable : IAuditLogTable
    {
        //[NotMapped]
        //bool IAuditLogTable.IsAuditLogEnabled { get => this.IsAuditLogEnabled ?? this.IsAuditLogEnabled.Value; set => this.IsAuditLogEnabled = value; }

        [NotMapped]
        public string FullTableName => $"{this.SchemaName}.{this.TableName}";
    }
}
