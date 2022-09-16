using System.Collections.Generic;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.Mobile.Reports
{
    public class MobileReportDTO
    {
        public int Id { get; set; }
        public string Icon { get; set; }
        public string Name { get; set; }
        public ReportTypesEnum ReportType { get; set; }
        public List<MobileReportParameterDTO> Parameters { get; set; }
    }
}
