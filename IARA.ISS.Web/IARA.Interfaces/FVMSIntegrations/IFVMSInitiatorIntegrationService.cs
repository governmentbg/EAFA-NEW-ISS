using System.Collections.Generic;
using System.Threading.Tasks;
using IARA.FVMSModels;
using IARA.FVMSModels.CrossChecks;
using IARA.FVMSModels.ExternalModels;
using IARA.FVMSModels.GeoZones;
using IARA.FVMSModels.NISS;

namespace IARA.Interfaces.FVMSIntegrations
{
    public interface IFVMSInitiatorIntegrationService
    {
        CCheckReport ReceiveCCheckQuery(CCheckQuery query);
        List<FishingGear> GetFishingGears(NISSQuery query);
        GeoZoneReport GetGeoZoneReport(GeoZoneQuery query);
        Certificate GetLicense(NISSQuery query);
        List<Certificate> GetLicenses(NISSQuery query);
        Permit GetPermit(NISSQuery query);
        List<Permit> GetPermitsByCFR(NISSQuery query);
        Task<bool> ReceiveCrossCheckReport(CCheckReport report);
        Task<bool> ReceiveTelemetryData(List<TelemetryStatus> telemetryData);
    }
}
