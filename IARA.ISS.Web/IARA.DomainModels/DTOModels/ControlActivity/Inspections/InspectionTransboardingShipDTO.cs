using System.Collections.Generic;

namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class InspectionTransboardingShipDTO
    {
        public string NNNShipStatus { get; set; }
        public string CaptainComment { get; set; }
        public VesselDuringInspectionDTO InspectedShip { get; set; }
        public PortVisitDTO LastPortVisit { get; set; }
        public List<InspectionCatchMeasureDTO> CatchMeasures { get; set; }
        public List<InspectionSubjectPersonnelDTO> Personnel { get; set; }
        public List<InspectionCheckDTO> Checks { get; set; }
        public List<InspectionPermitDTO> PermitLicenses { get; set; }
        public List<InspectionLogBookDTO> LogBooks { get; set; }
    }
}
