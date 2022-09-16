using System.Collections.Generic;
using IARA.DomainModels.DTOModels.CatchesAndSales;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.Nomenclatures;

namespace IARA.Interfaces.CatchSales
{
    public interface ILogBookNomenclaturesService
    {
        List<NomenclatureDTO> GetLogBookTypes();
        List<NomenclatureDTO> GetRegisteredBuyers();
        List<NomenclatureDTO> GetPermits();
        List<PermitLicenseNomenclatureDTO> GetPermitLicenses();
        List<NomenclatureDTO> GetTurbotSizeGroups();
        List<NomenclatureDTO> GetFishSizeCategories();
        List<NomenclatureDTO> GetCatchStates();
        List<NomenclatureDTO> GetUnloadTypes();
        List<NomenclatureDTO> GetFishPurposes();
        List<NomenclatureDTO> GetFishSizes();
        List<NomenclatureDTO> GetCatchTypes();
        List<FishingGearRegisterNomenclatureDTO> GetFishingGearsRegister(int permitLicenseId);
    }
}
