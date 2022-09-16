using System.Collections.Generic;

namespace IARA.DomainModels.DTOModels.ControlActivity.AuanRegister
{
    public class AuanReportDataDTO
    {
        public string ReportNum { get; set; }

        public string Drafter { get; set; }

        public int InspectionTypeId { get; set; }

        public int? TerritoryUnitId { get; set; }

        public List<AuanInspectedEntityDTO> InspectedEntities { get; set; }
    }
}
