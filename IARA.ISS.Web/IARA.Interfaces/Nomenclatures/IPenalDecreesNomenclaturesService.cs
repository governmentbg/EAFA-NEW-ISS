using System.Collections.Generic;
using IARA.DomainModels.Nomenclatures;

namespace IARA.Interfaces.Nomenclatures
{
    public interface IPenalDecreesNomenclaturesService
    {
        List<NomenclatureDTO> GetAllAuans();

        List<NomenclatureDTO> GetDecreeInspDeliveryTypes();

        List<NomenclatureDTO> GetPenalDecreeStatusTypes();

        List<NomenclatureDTO> GetPenalDecreeAuthorityTypes();

        List<NomenclatureDTO> GetCourts();

        List<NomenclatureDTO> GetPenalDecreeTypes();

        List<NomenclatureDTO> GetPenalDecreeSanctionTypes();

        List<NomenclatureDTO> GetConfiscationInstitutions();
    }
}
