using IARA.Mobile.Insp.Domain.Enums;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class InspectedFishingGearDto
    {
        public FishingGearDto PermittedFishingGear { get; set; }
        public FishingGearDto InspectedFishingGear { get; set; }
        public InspectedFishingGearEnum? CheckInspectedMatchingRegisteredGear { get; set; }
    }
}
