using IARA.Mobile.Application.DTObjects.Nomenclatures;
using System.Collections.Generic;

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
