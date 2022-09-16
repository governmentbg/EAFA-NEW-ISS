using System.Collections.Generic;
using IARA.DomainModels.DTOModels.ShipsRegister;
using IARA.Flux.Models;
using IARA.FluxModels.Enums;

namespace IARA.Interfaces.FluxIntegrations.Ships
{
    public interface IVesselToFluxVesselReportMapper
    {
        FLUXReportVesselInformationType MapVesselToFluxSub(ShipRegisterEditDTO vessel, ReportPurposeCodes purpose);
        FLUXReportVesselInformationType MapVesselToFluxSubVcd(ShipRegisterEditDTO vessel, ReportPurposeCodes purpose);
        FLUXReportVesselInformationType MapVesselToFluxVed(ShipRegisterEditDTO vessel, ReportPurposeCodes purpose);
        FLUXReportVesselInformationType MapVesselToFluxSub(List<ShipRegisterEditDTO> vessels, ReportPurposeCodes purpose);
        FLUXReportVesselInformationType MapVesselToFluxSubVcd(List<ShipRegisterEditDTO> vessels, ReportPurposeCodes purpose);
        FLUXReportVesselInformationType MapVesselToFluxVed(List<ShipRegisterEditDTO> vessels, ReportPurposeCodes purpose);
    }
}
