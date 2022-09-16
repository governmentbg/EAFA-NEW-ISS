using System.Collections.Generic;
using IARA.DomainModels.DTOModels.ScientificFishing;
using IARA.DomainModels.Nomenclatures;

namespace IARA.Interfaces.Nomenclatures
{
    public interface IScientificFishingNomenclaturesService
    {
        List<ScientificFishingReasonNomenclatureDTO> GetPermitReasons();

        List<NomenclatureDTO> GetPermitStatuses();
    }
}
