using System.Collections.Generic;
using IARA.DomainModels.DTOModels.CommercialFishingRegister;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.Nomenclatures;

namespace IARA.Interfaces.Nomenclatures
{
    public interface ICommercialFishingNomenclaturesService
    {
        List<QualifiedFisherNomenclatureDTO> GetQualifiedFishers();

        List<NomenclatureDTO> GetWaterTypes();

        List<NomenclatureDTO> GetCommercialFishingPermitTypes();

        List<NomenclatureDTO> GetCommercialFishingPermitLicenseTypes();

        List<NomenclatureDTO> GetHolderGroundForUseTypes();

        List<PoundNetNomenclatureDTO> GetPoundNets();

        List<SuspensionTypeNomenclatureDTO> GetSuspensionTypes();

        List<SuspensionReasonNomenclatureDTO> GetSuspensionReasons();

        /// <summary>
        /// Gets all permits for ship with RecordType = 'Register' and permits with RecordType = 'Application' that don't have a 'Register' entry
        /// </summary>
        /// <param name="shipId">Id of ship</param>
        /// <param name="onlyPoundNet">Flag for getting only permits for pound nets for the desired ship</param>
        /// <returns></returns>
        List<PermitNomenclatureDTO> GetShipPermits(int shipId, bool onlyPoundNet);

        List<PermitNomenclatureDTO> GetShipPermitLicenses(int shipId);
    }
}
