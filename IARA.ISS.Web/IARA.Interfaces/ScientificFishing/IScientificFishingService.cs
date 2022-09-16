using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.ScientificFishing;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces.Applications;

namespace IARA.Interfaces
{
    public interface IScientificFishingService : IService, IRegisterDeliveryService
    {
        IQueryable<ScientificFishingPermitDTO> GetAllPermits(ScientificFishingFilters filters);

        IQueryable<ScientificFishingPermitDTO> GetAllPermits(ScientificFishingPublicFilters filters, int currentUserId);

        void SetPermitHoldersForTable(List<ScientificFishingPermitDTO> permitS);

        ScientificFishingPermitEditDTO GetPermit(int id, int? userId = null);

        int AddPermit(ScientificFishingPermitEditDTO permit);

        void EditPermit(ScientificFishingPermitEditDTO permit);

        void DeletePermit(int id);

        void UndoDeletePermit(int id);

        Task<DownloadableFileDTO> GetRegisterFileForDownload(int registerId, SciFiPrintTypesEnum printType);

        ApplicationSubmittedByDTO GetUserAsSubmittedBy(int userId);

        string GetPermitHolderPhoto(int holderId);

        ScientificFishingApplicationEditDTO GetPermitApplication(int applicationId);

        RegixChecksWrapperDTO<ScientificFishingPermitRegixDataDTO> GetPermitRegixData(int applicationId);

        ScientificFishingPermitRegixDataDTO GetApplicationRegixData(int applicationId);

        ScientificFishingPermitEditDTO GetApplicationDataForRegister(int applicationId);

        ScientificFishingPermitEditDTO GetRegisterByApplicationId(int applicationId);

        int AddPermitApplication(ScientificFishingApplicationEditDTO permit, ApplicationStatusesEnum? nextManualStatus);

        void EditPermitApplication(ScientificFishingApplicationEditDTO permit, ApplicationStatusesEnum? manualStatus = null);

        void EditPermitApplicationRegixData(ScientificFishingPermitRegixDataDTO permitRegixData);

        int AddOuting(ScientificFishingOutingDTO outing);

        SimpleAuditDTO GetPermitHolderSimpleAudit(int id);

        SimpleAuditDTO GetPermitOutingSimpleAudit(int id);

        bool HasUserAccessToPermits(int userId, List<int> permitIds);

        bool HasUserAccessToPermitHolder(int userId, int holderId);

        bool HasUserAccessToPermitOuting(int userId, int outingId);

        bool HasUserAccessToPermitFile(int userId, int fileId);
    }
}
