using System.Collections.Generic;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Pub.Application.DTObjects.AddressNomenclatures.LocalDb;
using IARA.Mobile.Pub.Application.DTObjects.DocumentTypes.LocalDb;

namespace IARA.Mobile.Pub.Application.Interfaces.Transactions
{
    public interface INomenclatureTransaction
    {
        List<CountrySelectDto> GetCountries();
        List<TerritorialUnitSelectDto> GetTerritorialUnits();

        List<DistrictSelectDto> GetDistricts();

        List<MunicipalitySelectDto> GetMuncipalitiesByDisctrict(int districtId);

        List<PopulatedAreaSelectDto> GetPopulatedAreasByMunicipality(int municipalityId);

        List<DocumentTypeSelectDto> GetDocumentTypes();

        List<NomenclatureDto> GetPermitReasons();

        List<NomenclatureDto> GetFishTypes();

        List<NomenclatureDto> GetViolationSignalTypes();

        int GetDocumentTypeIdByCode(string documentTypeCode);

        int GetActiveFileTypeIdByCode(string fileType);

        List<int> GetAllFileTypeIdsByCode(string fileType);

        int GetActiveGenderId(string genderCode);

        string GetGenderCodeById(int genderId);

        List<NomenclatureDto> GetGenders(List<string> codes);

        List<NomenclatureDto> GetGenders();

        string GetSystemParameter(string code);

        List<NomenclatureDto> GetPaymentTypes(List<string> codes);

        List<string> GetPermissions();
    }
}
