using System.Collections.Generic;
using System;

namespace IARA.DomainModels.RequestModels
{
    public class FluxAcdrRequestFilters : BaseRequestModel
    {
        public string WebServiceName { get; set; }

        public DateTime? RequestDateFrom { get; set; }

        public DateTime? RequestDateTo { get; set; }

        public DateTime? ResponseDateFrom { get; set; }

        public DateTime? ResponseDateTo { get; set; }

        public DateTime? RequestMonthDateFrom { get; set; }

        public string RequestUUID { get; set; }

        public string ResponseUUID { get; set; }

        public string RequestContent { get; set; }

        public string ResponseContent { get; set; }

        public List<string> ResponseStatuses { get; set; }

        public List<string> DomainNames { get; set; }

        public List<string> ReportStatuses { get; set; }
    }
}
