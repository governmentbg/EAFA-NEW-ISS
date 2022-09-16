using System.Collections.Generic;
using IARA.FVMSModels.ExternalModels;

namespace IARA.Interfaces.FVMSIntegrations
{
    public interface IPermitsAndLicencesService
    {
        List<FishingGear> GetFishingGears(string licenseNumber);
        Certificate GetLicense(string licenseNumber);
        List<Certificate> GetLicensesByPermitNumber(string permitNumber);
        List<Certificate> GetLicensesByCFR(string cfr);
        Permit GetPermit(string permitNumber);
        List<Permit> GetPermits(string cfr);
    }
}
