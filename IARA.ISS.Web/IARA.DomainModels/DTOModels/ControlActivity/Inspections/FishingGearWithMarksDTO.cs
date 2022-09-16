using System.Collections.Generic;

namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class FishingGearWithMarksDTO
    {
        public List<FishingGearInspectionDTO> FishingGears { get; set; }
        public List<FishingGearMarkInspectionDTO> FishingGearMarks { get; set; }
    }
}
