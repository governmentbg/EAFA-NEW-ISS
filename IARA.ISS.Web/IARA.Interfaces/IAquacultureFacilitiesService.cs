using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.AquacultureFacilities;
using IARA.DomainModels.DTOModels.AquacultureFacilities.ChangeOfCircumstances;
using IARA.DomainModels.DTOModels.AquacultureFacilities.Deregistration;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Excel.Tools.Models;
using IARA.Interfaces.Applications;

namespace IARA.Interfaces
{
    public interface IAquacultureFacilitiesService : IService, IRegisterDeliveryService
    {
        // Register
        IQueryable<AquacultureFacilityDTO> GetAllAquacultures(AquacultureFacilitiesFilters filters);

        AquacultureFacilityEditDTO GetAquaculture(int id);

        int AddAquaculture(AquacultureFacilityEditDTO aquaculture, bool ignoreLogBookConflicts);

        void EditAquaculture(AquacultureFacilityEditDTO aquaculture, bool ignoreLogBookConflicts);

        void UpdateAquacultureStatus(int aquacultureId, CancellationHistoryEntryDTO status, int? applicationId);

        void DeleteAquaculture(int id);

        void UndoDeleteAquaculture(int id);

        Task<byte[]> DownloadAquacultureFacility(int aquacultureId);

        Stream DownloadAquacultureFacilitiesExcel(ExcelExporterRequestModel<AquacultureFacilitiesFilters> request);

        SimpleAuditDTO GetAquacultureInstallationSimpleAudit(int id);

        SimpleAuditDTO GetAquacultureInstallationNetCageAudit(int id);

        SimpleAuditDTO GetAquacultureUsageDocumentSimpleAudit(int id);

        SimpleAuditDTO GetAquacultureWaterLawCertificateSimpleAudit(int id);

        SimpleAuditDTO GetAquacultureOvosCertificateSimpleAudit(int id);

        SimpleAuditDTO GetAquacultureBabhCertificateSimpleAudit(int id);

        // Applications
        AquacultureApplicationEditDTO GetAquacultureApplication(int applicationId);

        RegixChecksWrapperDTO<AquacultureRegixDataDTO> GetAquacultureRegixData(int applicationId);

        AquacultureRegixDataDTO GetApplicationRegistrationRegixData(int applicationId);

        AquacultureFacilityEditDTO GetApplicationDataForRegister(int applicationId);

        AquacultureFacilityEditDTO GetRegisterByApplicationId(int applicationId);

        AquacultureFacilityEditDTO GetRegisterByChangeOfCircumstancesApplicationId(int applicationId);

        int AddAquacultureApplication(AquacultureApplicationEditDTO aquaculture, ApplicationStatusesEnum? nextManualStatus = null);

        void EditAquacultureApplication(AquacultureApplicationEditDTO aquaculture, ApplicationStatusesEnum? manualStatus = null);

        void EditAquacultureApplicationRegixData(AquacultureRegixDataDTO aquaculture);

        // Change of circumstances
        AquacultureChangeOfCircumstancesApplicationDTO GetAquacultureChangeOfCircumstancesApplication(int applicationId);

        RegixChecksWrapperDTO<AquacultureChangeOfCircumstancesRegixDataDTO> GetAquacultureChangeOfCircumstancesRegixData(int applicationId);

        AquacultureChangeOfCircumstancesRegixDataDTO GetApplicationChangeOfCircumstancesRegixData(int applicationId);

        int AddAquacultureChangeOfCircumstancesApplication(AquacultureChangeOfCircumstancesApplicationDTO application, ApplicationStatusesEnum? nextManualStatus = null);

        void EditAquacultureChangeOfCircumstancesApplication(AquacultureChangeOfCircumstancesApplicationDTO application, ApplicationStatusesEnum? manualStatus = null);

        void EditAquacultureChangeOfCircumstancesRegixData(AquacultureChangeOfCircumstancesRegixDataDTO application);

        AquacultureFacilityEditDTO GetAquacultureFromChangeOfCircumstancesApplication(int applicationId);

        void CompleteChangeOfCircumstancesApplication(AquacultureFacilityEditDTO aquaculture, bool ignoreLogBookConflicts);

        // Deregistration
        AquacultureDeregistrationApplicationDTO GetAquacultureDeregistrationApplication(int applicationId);

        RegixChecksWrapperDTO<AquacultureDeregistrationRegixDataDTO> GetAquacultureDeregistrationRegixData(int applicationId);

        AquacultureDeregistrationRegixDataDTO GetApplicationDeregistrationRegixData(int applicationId);

        int AddAquacultureDeregistrationApplication(AquacultureDeregistrationApplicationDTO application, ApplicationStatusesEnum? nextManualStatus = null);

        void EditAquacultureDeregistrationApplication(AquacultureDeregistrationApplicationDTO application, ApplicationStatusesEnum? manualStatus = null);

        void EditAquacultureDeregistrationRegixData(AquacultureDeregistrationRegixDataDTO application);

        AquacultureFacilityEditDTO GetAquacultureFromDeregistrationApplication(int applicationId);
    }
}
