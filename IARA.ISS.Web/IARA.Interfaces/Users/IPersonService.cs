using System.Collections.Generic;
using System.Linq;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;

namespace IARA.Interfaces
{
    public interface IPersonService
    {
        List<AddressRegistrationDTO> GetAddressRegistrations(int personId);

        ILookup<int, AddressRegistrationDTO> GetAddressRegistrations(List<int> personIds);

        PersonDocumentDTO GetPersonDocument(int personId);

        Dictionary<int, PersonDocumentDTO> GetPersonDocuments(List<int> personIds);

        string GetPersonPhoneNumber(int personId);

        Dictionary<int, string> GetPersonPhoneNumbers(List<int> personIds);

        string GetPersonEmail(int personId);

        Dictionary<int, string> GetPersonEmails(List<int> personIds);

        string GetPersonPhoto(int personId);

        string GetPersonPhotoByFileId(int fileId);

        DownloadableFileDTO GetPersonPhotoAsModel(int userId);

        Dictionary<int, string> GetPersonPhotos(List<int> personIds);

        RegixPersonDataDTO GetRegixPersonData(int personId);

        Dictionary<int, RegixPersonDataDTO> GetRegixPersonsData(List<int> personIds);
    }
}
