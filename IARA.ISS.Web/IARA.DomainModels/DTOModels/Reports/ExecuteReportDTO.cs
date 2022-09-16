using System.Collections.Generic;
using IARA.Common.Enums;

namespace IARA.DomainModels.DTOModels.Reports
{
    public class ExecuteReportDTO
    {
        public int Id { get; set; }
        public int ReportGroupId { get; set; }
        public string Name { get; set; }
        public string SqlQuery { get; set; }
        public ReportTypesEnum ReportType { get; set; }
        public List<ReportParameterExecuteDTO> Parameters { get; set; }
    }
}
