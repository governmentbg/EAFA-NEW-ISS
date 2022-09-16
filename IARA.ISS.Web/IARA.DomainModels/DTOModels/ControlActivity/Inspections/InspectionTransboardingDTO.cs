using System.Collections.Generic;

namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class InspectionTransboardingDTO : InspectionEditDTO
    {
        public List<InspectionCatchMeasureDTO> TransboardedCatchMeasures { get; set; }

        public InspectionTransboardingShipDTO ReceivingShipInspection { get; set; }

        public InspectionTransboardingShipDTO SendingShipInspection { get; set; }

        /// <summary>
        /// Used only for the port inspection.
        /// </summary>
        public List<InspectedFishingGearDTO> FishingGears { get; set; }
    }
}
