using IARA.DomainModels.DTOModels.ControlActivity.AuanRegister;

namespace IARA.DomainModels.DTOModels.ControlActivity.PenalDecrees
{
    public class PenalDecreeSeizedFishDTO : AuanConfiscatedFishDTO
    {
        public int? StorageTerritoryUnitId { get; set; }
    }
}
