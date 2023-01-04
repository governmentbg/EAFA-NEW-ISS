using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.Interfaces.Dtos;
using System.Collections.Generic;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class InspectionTransboardingShipDto : IFishingShipInspection
    {
        public string NNNShipStatus { get; set; }
        public string CaptainComment { get; set; }
        public VesselDuringInspectionDto InspectedShip { get; set; }
        public PortVisitDto LastPortVisit { get; set; }
        public List<InspectionCatchMeasureDto> CatchMeasures { get; set; }
        public List<InspectionSubjectPersonnelDto> Personnel { get; set; }
        public List<InspectionCheckDto> Checks { get; set; }
        public List<InspectionPermitDto> PermitLicenses { get; set; }
        public List<InspectionPermitDto> Permits { get; set; }
        public List<InspectionLogBookDto> LogBooks { get; set; }
    }
}
