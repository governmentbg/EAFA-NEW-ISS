using System.Collections.Generic;
using IARA.Common.GridModels;
using IARA.DomainModels.DTOModels.Reports;

namespace IARA.DomainModels.RequestModels
{
    public class ReportGridRequestDTO : BaseGridRequestModel
    {
        public int ReportId { get; set; }
        public string SqlQuery { get; set; }
        public int? UserId { get; set; }
        public List<ExecutionParamDTO> Parameters { get; set; }

    }
}
