using System;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.FishingActivityReports
{
    public class FishingActivityReportItemDTO
    {
        public int Id { get; set; }

        public int RequestId { get; set; }

        public string Uuid { get; set; }

        public string Purpose { get; set; }

        public string ReportType { get; set; }

        public string FaType { get; set; }

        public DateTime Date { get; set; }

        public string Status { get; set; }

        public string ErrorMessage { get; set; }

        public bool IsActive { get; set; }
    }
}
