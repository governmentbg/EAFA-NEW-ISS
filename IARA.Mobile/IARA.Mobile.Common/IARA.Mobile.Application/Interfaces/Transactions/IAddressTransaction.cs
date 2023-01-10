using System.Collections.Generic;
using IARA.Mobile.Application.DTObjects.Nomenclatures;

namespace IARA.Mobile.Application.Interfaces.Transactions
{
    public interface IAddressTransaction
    {
        List<SelectNomenclatureDto> GetCountries();
        List<SelectNomenclatureDto> GetDistricts();
        List<SelectNomenclatureDto> GetMuncipalities(int districtId);
        List<SelectNomenclatureDto> GetPopulatedAreas(int municipalityId);
    }
}
