using System.Collections.Generic;
using IARA.DomainModels.Nomenclatures;

namespace IARA.Interfaces.Nomenclatures
{
    public interface IPoundnetNomenclaturesService
    {
        List<NomenclatureDTO> GetSeasonalTypes();

        List<NomenclatureDTO> GetPoundnetCategories();

        List<NomenclatureDTO> GetPoundnetStatuses();
    }
}
