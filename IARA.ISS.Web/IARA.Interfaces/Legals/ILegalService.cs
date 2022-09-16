using System.Collections.Generic;
using System.Linq;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.Nomenclatures;

namespace IARA.Interfaces.Legals
{
    public interface ILegalService
    {
        List<NomenclatureDTO> GetActiveLegals();

        List<AddressRegistrationDTO> GetAddressRegistrations(int legalId);

        ILookup<int, AddressRegistrationDTO> GetAddressRegistrations(List<int> legalIds);

        string GetLegalPhoneNumber(int legalId);

        Dictionary<int, string> GetLegalPhoneNumbers(List<int> legalsIds);

        string GetLegalEmail(int legalId);

        Dictionary<int, string> GetLegalEmails(List<int> legalIds);

        RegixLegalDataDTO GetRegixLegalData(int legalId);

        Dictionary<int, RegixLegalDataDTO> GetRegixLegalsData(List<int> legalIds);

        string GetLegalPostalCode(int legalId);
    }
}
