using System.Collections.Generic;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class FishingGearWithMarksDto
    {
        public List<FishingGearInspectionNomenclatureDto> FishingGears { get; set; }
        public List<FishingGearMarkInspectionNomenclatureDto> FishingGearMarks { get; set; }
    }
}
