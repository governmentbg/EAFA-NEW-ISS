using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.CommercialFishingRegister;

namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class InspectedFishingGearDTO
    {
        public FishingGearDTO PermittedFishingGear { get; set; }
        public FishingGearDTO InspectedFishingGear { get; set; }
        public bool? HasAttachedAppliances { get; set; }
        public InspectedFishingGearEnum? CheckInspectedMatchingRegisteredGear { get; set; }
        public bool IsActive { get; set; }
    }
}
