using System;

namespace IARA.Logging.Abstractions.Models
{
    public class AuditRecord
    {
        public string BrowserInfo { get; set; }
        public string EntityName { get; set; }
        public object NewValues { get; set; }
        public object OldValues { get; set; }
        public string PrimaryKeys { get; set; }
        public string IPAddress { get; set; }
        public string Endpoint { get; set; } //Endpoint from which data has been modified
        public string UserName { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ActionType { get; set; }
    }
}
