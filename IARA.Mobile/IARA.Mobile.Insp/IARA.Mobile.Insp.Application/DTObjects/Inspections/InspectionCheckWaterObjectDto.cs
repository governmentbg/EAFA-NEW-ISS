using System.Collections.Generic;
using IARA.Mobile.Application.DTObjects.Common;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class InspectionCheckWaterObjectDto : InspectionEditDto
    {
        public string ObjectName { get; set; }
        public int? WaterObjectTypeId { get; set; }
        public LocationDto WaterObjectLocation { get; set; }
        public List<WaterInspectionFishingGearDto> FishingGears { get; set; }
        public List<WaterInspectionVesselDto> Vessels { get; set; }
        public List<WaterInspectionEngineDto> Engines { get; set; }
        public List<InspectionCatchMeasureDto> Catches { get; set; }
    }
}
