using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using System.Collections.Generic;

namespace IARA.Mobile.Insp.Application.Interfaces.Dtos
{
    public interface IFishingShipInspection
    {
        VesselDuringInspectionDto InspectedShip { get; set; }
        List<InspectionSubjectPersonnelDto> Personnel { get; set; }
        List<InspectionCatchMeasureDto> CatchMeasures { get; set; }
        List<InspectionCheckDto> Checks { get; set; }
        List<InspectionPermitDto> PermitLicenses { get; set; }
        List<InspectionPermitDto> Permits { get; set; }
        List<InspectionLogBookDto> LogBooks { get; set; }
    }
}
