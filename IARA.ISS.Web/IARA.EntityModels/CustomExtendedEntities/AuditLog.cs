using System.ComponentModel.DataAnnotations.Schema;
using TL.Logging.Abstractions.Interfaces.Models.Audit;

namespace IARA.EntityModels.Entities
{
    public partial class AuditLog : IAuditLog<string>
    {
        [NotMapped]
        public string SystemModule { get => this.Action; set => this.Action = value; }

        [NotMapped]
        public string UserId { get => this.Username; set => this.Username = value; }
    }
}
