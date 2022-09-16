using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class InspectionCheckWaterObjectDTO : InspectionEditDTO
    {
        [StringLength(500, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string ObjectName { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? WaterObjectTypeId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public LocationDTO WaterObjectLocation { get; set; }

        public List<WaterInspectionFishingGearDTO> FishingGears { get; set; }
        public List<WaterInspectionVesselDTO> Vessels { get; set; }
        public List<WaterInspectionEngineDTO> Engines { get; set; }
        public List<InspectionCatchMeasureDTO> Catches { get; set; }
    }
}
