using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class InspectionAtSeaDTO : InspectionEditDTO
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public VesselDuringInspectionDTO InspectedShip { get; set; }
        public PortVisitDTO LastPortVisit { get; set; }

        public string CaptainComment { get; set; }

        public List<InspectionCatchMeasureDTO> CatchMeasures { get; set; }
        public List<InspectedFishingGearDTO> FishingGears { get; set; }

        public List<InspectionPermitDTO> PermitLicenses { get; set; }
        public List<InspectionLogBookDTO> LogBooks { get; set; }
    }
}
