using System.Collections.Generic;
using IARA.DomainModels.Nomenclatures;

namespace IARA.Interfaces.Nomenclatures
{
    public interface ISystemLogNomenclaturesService
    {
        List<NomenclatureDTO> GetActionTypeCategories();
    }
}
