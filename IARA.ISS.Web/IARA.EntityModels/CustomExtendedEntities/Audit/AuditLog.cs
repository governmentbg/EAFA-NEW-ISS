using System;
using System.ComponentModel.DataAnnotations.Schema;
using TL.Logging.Abstractions.Interfaces.Models.Audit;

namespace IARA.EntityModels.Entities
{
    public partial class AuditLog : IAuditLog<string>, IEventIdentity
    {
        [NotMapped]
        public string SystemModule { get => this.Action; set => this.Action = value; }

        [NotMapped]
        public string UserId { get => this.Username; set => this.Username = value; }

        [NotMapped]
        public Guid EventId { get => this.EventUid; set => this.EventUid = value; }

        [NotMapped]
        public string IpAddress { get => this.Ipaddress; set => this.Ipaddress = value; }
    }
}
