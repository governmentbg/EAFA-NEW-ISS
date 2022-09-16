using System;
using System.Collections.Generic;
using IARA.DomainModels.DTOModels.ControlActivity.AuanRegister;

namespace IARA.DomainModels.DTOModels.ControlActivity.PenalDecrees
{
    public class PenalDecreeAuanDataDTO
    {
        public string AuanNum { get; set; }

        public DateTime? DraftDate { get; set; }

        public int? TerritoryUnitId { get; set; }

        public string Drafter { get; set; }

        public string LocationDescription { get; set; }

        public string OffenderComments { get; set; }

        public string ConstatationComments { get; set; }

        public AuanInspectedEntityDTO InspectedEntity { get; set; }

        public List<PenalDecreeSeizedFishDTO> ConfiscatedFish { get; set; }
        public List<PenalDecreeSeizedFishDTO> ConfiscatedAppliance { get; set; }

        public List<PenalDecreeSeizedFishingGearDTO> ConfiscatedFishingGear { get; set; }

        public List<AuanViolatedRegulationDTO> ViolatedRegulations { get; set; }
    }
}
