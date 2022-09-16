using System.Collections.Generic;
using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Duplicates;

namespace IARA.Interfaces
{
    public interface IDuplicatesRegisterService : IService
    {
        List<DuplicatesEntryDTO> GetDuplicateEntries(int? buyerId = null, int? fisherId = null, int? permitId = null, int? permitLicenceId = null);

        DuplicatesApplicationDTO GetDuplicatesApplication(int applicationId);

        RegixChecksWrapperDTO<DuplicatesApplicationRegixDataDTO> GetDuplicatesRegixData(int applicationId);

        DuplicatesApplicationRegixDataDTO GetApplicationDuplicatesRegixData(int applicationId);

        DuplicatesRegisterEditDTO GetApplicationDataForRegister(int applicationId);

        DuplicatesRegisterEditDTO GetRegisterByApplicationId(int applicationId);

        int AddDuplicatesApplication(DuplicatesApplicationDTO application, ApplicationStatusesEnum? nextManualStatus = null);

        void EditDuplicatesApplication(DuplicatesApplicationDTO application, ApplicationStatusesEnum? manualStatus = null);

        void EditDuplicatesRegixData(DuplicatesApplicationRegixDataDTO application);

        DuplicatesRegisterEditDTO GetDuplicateRegister(int id);

        int AddDuplicateRegister(DuplicatesRegisterEditDTO duplicate);

        Task<byte[]> DownloadDuplicateRegister(int id);
    }
}
