using System.Collections.Generic;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures.API;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class ObservationAtSeaDto : InspectionEditDto
    {
        public VesselDuringInspectionDto ObservedVessel { get; set; }
        public string Course { get; set; }
        public decimal? Speed { get; set; }
        public List<ObservationToolDto> ObservationTools { get; set; }
        public bool? HasShipContact { get; set; }
        public bool? HasShipCommunication { get; set; }
        public string ShipCommunicationDescription { get; set; }
        public List<VesselActivityApiDto> ObservedVesselActivities { get; set; }
    }
}
