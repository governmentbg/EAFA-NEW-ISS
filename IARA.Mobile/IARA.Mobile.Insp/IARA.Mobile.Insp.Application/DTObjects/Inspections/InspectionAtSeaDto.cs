using System.Collections.Generic;
using IARA.Mobile.Insp.Application.Interfaces.Dtos;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class InspectionAtSeaDto : InspectionEditDto, IFishingShipInspection
    {
        public string CaptainComment { get; set; }
        public VesselDuringInspectionDto InspectedShip { get; set; }
        public PortVisitDto LastPortVisit { get; set; }
        public List<InspectionCatchMeasureDto> CatchMeasures { get; set; }
        public List<InspectedFishingGearDto> FishingGears { get; set; }
        public List<InspectionPermitDto> Permits { get; set; }
        public List<InspectionPermitDto> PermitLicenses { get; set; }
        public List<InspectionLogBookDto> LogBooks { get; set; }
    }
}
