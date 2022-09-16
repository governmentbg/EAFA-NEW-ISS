using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class InspectionObservationAtSeaDTO : InspectionEditDTO
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public VesselDuringInspectionDTO ObservedVessel { get; set; }
        public string Course { get; set; }
        public decimal? Speed { get; set; }
        public List<InspectionObservationToolDTO> ObservationTools { get; set; }
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? HasShipContact { get; set; }
        public bool? HasShipCommunication { get; set; }
        public string ShipCommunicationDescription { get; set; }
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<InspectionVesselActivityNomenclatureDTO> ObservedVesselActivities { get; set; }
    }
}
