using System;
using System.Collections.Generic;

namespace IARA.DomainModels.RequestModels
{
    public class FLUXVMSRequestFilters : BaseRequestModel
    {
        public string WebServiceName { get; set; }

        public DateTime? RequestDateFrom { get; set; }

        public DateTime? RequestDateTo { get; set; }

        public DateTime? ResponseDateFrom { get; set; }

        public DateTime? ResponseDateTo { get; set; }

        public string RequestUUID { get; set; }

        public string ResponseUUID { get; set; }

        public List<string> ResponseStatuses { get; set; }

        public List<string> DomainNames { get; set; }
    }
}
