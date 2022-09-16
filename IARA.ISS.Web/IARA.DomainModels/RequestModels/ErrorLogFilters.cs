using System;
using System.Collections.Generic;

namespace IARA.DomainModels.RequestModels
{
    public class ErrorLogFilters : BaseRequestModel
    {
        public DateTime? ErrorLogDateFrom { get; set; }
        public DateTime? ErrorLogDateTo { get; set; }
        public List<string> Severity { get; set; }
        public string Class { get; set; }
        public int? UserId { get; set; }
    }
}
