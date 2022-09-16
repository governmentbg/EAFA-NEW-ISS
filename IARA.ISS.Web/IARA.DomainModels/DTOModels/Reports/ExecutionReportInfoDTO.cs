using System.Collections.Generic;

namespace IARA.DomainModels.DTOModels.Reports
{
    public class ExecutionReportInfoDTO
    {
        public int ReportId { get; set; }
        public string Name { get; set; }
        public string SqlQuery { get; set; }
        public List<ExecutionParamDTO> Parameters { get; set; }
    }
}
