using IARA.Mobile.Application.DTObjects.Common;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class VesselDuringInspectionDto : VesselDto
    {
        public int? Id { get; set; }
        public LocationDto Location { get; set; }
        public string LocationText { get; set; }
        public int? CatchZoneId { get; set; }
        public string LocationDescription { get; set; }
        public int? ShipAssociationId { get; set; }
    }
}
