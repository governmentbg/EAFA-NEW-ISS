using System.Collections.Generic;

namespace IARA.DomainModels.DTOModels.Reports
{
    public class ReportMobileRequestDTO
    {
        public int ReportId { get; set; }
        public List<ExecutionParamDTO> Parameters { get; set; }
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
    }
}
