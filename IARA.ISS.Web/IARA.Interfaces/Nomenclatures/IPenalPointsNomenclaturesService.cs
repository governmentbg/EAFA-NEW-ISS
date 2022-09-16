using System.Collections.Generic;
using IARA.DomainModels.Nomenclatures;

namespace IARA.Interfaces.Nomenclatures
{
    public interface IPenalPointsNomenclaturesService
    {
        List<NomenclatureDTO> GetAllPenalDecrees();

        List<NomenclatureDTO> GetAllPenalPoinsStatuses();
    }
}
