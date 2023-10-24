using System;
using System.Collections.Generic;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.FLUXVMSRequests
{
    public class FluxAcdrRequestDTO
    {
        public int FluxRequestId { get; set; }

        public bool IsOutgoing { get; set; }

        public string WebServiceName { get; set; }

        public string RequestUUID { get; set; }

        public DateTime RequestDateTime { get; set; }

        public DateTime PeriodStart { get; set; }

        public DateTime PeriodEnd { get; set; }

        public int PeriodMonth { get; set; }

        public int PeriodYear { get; set; }

        public string ResponseStatus { get; set; }

        public string ResponseUUID { get; set; }

        public DateTime? ResponseDateTime { get; set; }

        public string ErrorDescription { get; set; }

        public FluxAcdrReportStatusEnum ReportStatus { get; set; }

        public string ReportStatusName { get; set; }

        public bool IsActive { get; set; }

        public List<FluxAcdrReportDTO> HistoryRecords { get; set; }
    }
}
