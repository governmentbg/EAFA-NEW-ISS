using System;
using IARA.DomainModels.DTOModels.ControlActivity.AuanRegister;

namespace IARA.DomainModels.DTOModels.ControlActivity.PenalPoints
{
    public class PenalPointsAuanDecreeDataDTO
    {
        public int? TerritoryUnitId { get; set; }

        public string AuanNum { get; set; }

        public DateTime? AuanDate { get; set; }

        public string DecreeNum { get; set; }

        public DateTime? DecreeIssueDate { get; set; }

        public DateTime? DecreeEffectiveDate { get; set; }

        public AuanInspectedEntityDTO InspectedEntity { get; set; }
    }
}
