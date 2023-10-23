using System;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.FLUXVMSRequests
{
    public class FluxAcdrReportDTO
    {
        public int Id { get; set; }

        public string RequestUUID { get; set; }

        public string WebServiceName { get; set; }

        public string ResponseStatus { get; set; }

        public DateTime? ResponseDateTime { get; set; }

        public string ErrorDescription { get; set; }

        public bool IsActive { get; set; }

        public DateTime ReportCreatedOn { get; set; }

        public int RequestId { get; set; }

        public bool? IsModified { get; set; }

        public FluxAcdrReportStatusEnum ReportStatus { get; set; }

        public string ReportStatusName { get; set; }

        public DateTime? PeriodStart { get; set; }

        public DateTime? PeriodEnd { get; set; }
    }
}
