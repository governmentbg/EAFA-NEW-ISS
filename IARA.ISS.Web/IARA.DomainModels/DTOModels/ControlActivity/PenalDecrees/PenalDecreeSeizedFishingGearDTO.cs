using IARA.DomainModels.DTOModels.ControlActivity.AuanRegister;

namespace IARA.DomainModels.DTOModels.ControlActivity.PenalDecrees
{
    public class PenalDecreeSeizedFishingGearDTO : AuanConfiscatedFishingGearDTO
    {
        public int? StorageTerritoryUnitId { get; set; }
    }
}
