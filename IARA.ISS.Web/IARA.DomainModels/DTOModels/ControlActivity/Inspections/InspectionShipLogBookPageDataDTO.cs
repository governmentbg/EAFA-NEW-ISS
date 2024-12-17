using System.Collections.Generic;

namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class InspectionShipLogBookPageDataDTO
    {
        public InspectedFishingGearDTO FishingGear { get; set; }

        public List<InspectionCatchMeasureDTO> CatchRecords { get; set; }
    }
}
