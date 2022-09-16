using System.Collections.Generic;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class InspectionTransboardingDto : InspectionEditDto
    {
        public List<InspectionCatchMeasureDto> TransboardedCatchMeasures { get; set; }

        public InspectionTransboardingShipDto ReceivingShipInspection { get; set; }

        public InspectionTransboardingShipDto SendingShipInspection { get; set; }

        public List<InspectedFishingGearDto> FishingGears { get; set; }
    }
}
