using System.Collections.Generic;
using IARA.DomainModels.Nomenclatures;

namespace IARA.Interfaces.Nomenclatures
{
    public interface IAquacultureFacilitiesNomenclaturesService
    {
        List<NomenclatureDTO> GetAllAquacultureNomenclatures();
        List<NomenclatureDTO> GetAquaculturePowerSupplyTypes();
        List<NomenclatureDTO> GetAquacultureWaterAreaTypes();
        List<NomenclatureDTO> GetWaterLawCertificateTypes();
        List<NomenclatureDTO> GetAquacultureInstallationTypes();
        List<NomenclatureDTO> GetInstallationBasinPurposeTypes();
        List<NomenclatureDTO> GetInstallationBasinMaterialTypes();
        List<NomenclatureDTO> GetHatcheryEquipmentTypes();
        List<NomenclatureDTO> GetInstallationNetCageTypes();
        List<NomenclatureDTO> GetInstallationCollectorTypes();
        List<NomenclatureDTO> GetAquacultureStatusTypes();
    }
}
