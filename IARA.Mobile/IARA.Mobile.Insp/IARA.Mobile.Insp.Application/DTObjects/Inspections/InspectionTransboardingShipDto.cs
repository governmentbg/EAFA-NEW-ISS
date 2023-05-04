using System.Collections.Generic;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.Interfaces.Dtos;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class InspectionTransboardingShipDto : IFishingShipInspection
    {
        public int? InspectionPortId { get; set; }
        public string UnregisteredPortName { get; set; }
        public int? UnregisteredPortCountryId { get; set; }
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
