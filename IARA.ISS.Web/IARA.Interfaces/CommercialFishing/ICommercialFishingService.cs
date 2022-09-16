using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.CommercialFishingRegister;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.QualifiedFrishersRegister;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces.Applications;

namespace IARA.Interfaces.CommercialFishing
{
    public interface ICommercialFishingService : IService, IRegisterDeliveryService
    {
        IQueryable<CommercialFishingPermitRegisterDTO> GetAllCommercialFishingPermits(CommercialFishingRegisterFilters filters);
        List<CommercialFishingPermitLicenseRegisterDTO> GetPermitLicensesForTable(IEnumerable<int> permitIds, CommercialFishingRegisterFilters filters);
        CommercialFishingEditDTO GetPermit(int id);
        CommercialFishingEditDTO GetPermitLicense(int id);
        CommercialFishingEditDTO GetPermitByApplicationId(int applicationId);
        CommercialFishingEditDTO GetPermitLicenseByApplicationId(int applicationId);
        CommercialFishingApplicationEditDTO GetPermitLicenseApplicationDataFromPermit(string permitNumber, int applicationId);
        CommercialFishingApplicationEditDTO GetPermitLicenseApplicationDataFromPermit(int permitId, int applicationId);
        CommercialFishingEditDTO GetPermitApplicationDataForRegister(int applicationId);
        CommercialFishingEditDTO GetPermitLicenseApplicationDataForRegister(int applicationId);
        Task<DownloadableFileDTO> GetPermitRegisterFileForDownload(int registerId, CommercialFishingTypesEnum permitType, bool isDuplicate = false);
        Task<DownloadableFileDTO> GetPermitLicenseRegisterFileForDownload(int registerId, CommercialFishingTypesEnum permitLicenseType, bool isDuplicate = false);
        CommercialFishingTypesEnum GetPermitType(int registerId);
        CommercialFishingTypesEnum GetPermitLicenseType(int registerId);
        int AddPermit(CommercialFishingEditDTO permit);
        int AddPermitLicense(CommercialFishingEditDTO permitLicense, bool ignoreLogBookConflicts);
        void EditPermit(CommercialFishingEditDTO permit, int currentUserId);
        void EditPermitLicense(CommercialFishingEditDTO permitLicense, int currentUserId, bool ignoreLogBookConflicts);
        CommercialFishingApplicationEditDTO GetPermitApplication(int applicationId);
        CommercialFishingApplicationEditDTO GetPermitLicenseApplication(int applicationId);
        CommercialFishingApplicationEditDTO GetPermitLicenseForRenewal(int permitLicenseId);
        RegixChecksWrapperDTO<CommercialFishingRegixDataDTO> GetPermitRegixData(int applicationId);
        RegixChecksWrapperDTO<CommercialFishingRegixDataDTO> GetPermitLicenseRegixData(int applicationId);
        Task<List<CommercialFishingValidationErrorsEnum>> ValidateApplicationData(PageCodeEnum pageCode,
                                                                                  ApplicationSubmittedForRegixDataDTO submittedFor,
                                                                                  ApplicationSubmittedByRegixDataDTO submittedBy,
                                                                                  QualifiedFisherBasicDataDTO qualifiedFisher,
                                                                                  int shipId,
                                                                                  int deliveryTypeId,
                                                                                  int waterTypeId,
                                                                                  bool? isHolderShipOwner,
                                                                                  string permitRegistrationNumber = null);
        List<CommercialFishingValidationErrorsEnum> ValidateApplicationRegiXData(PageCodeEnum pageCode,
                                                                                 ApplicationSubmittedForRegixDataDTO submittedFor,
                                                                                 ApplicationSubmittedByRegixDataDTO submittedBy,
                                                                                 int id,
                                                                                 bool isPermit);
        int AddPermitApplication(CommercialFishingApplicationEditDTO permit, ApplicationStatusesEnum? nextManualStatus);
        CommercialFishingApplicationEditDTO AddPermitApplicationAndStartPermitLicenseApplication(CommercialFishingApplicationEditDTO permit,
                                                                                                 int currentUserId,
                                                                                                 ApplicationStatusesEnum? nextManualStatus);
        int AddPermitLicenseApplication(CommercialFishingApplicationEditDTO permitLicense,
                                        bool isFromPublicApp = false,
                                        ApplicationStatusesEnum? nextManualStatus = null);
        void EditPermitApplication(CommercialFishingApplicationEditDTO permit, ApplicationStatusesEnum? manualStatus = null);
        void EditPermitLicenseApplication(CommercialFishingApplicationEditDTO permitLicense, bool isFromPublicApp = false, ApplicationStatusesEnum? manualStatus = null);
        void EditPermitApplicationRegixData(CommercialFishingRegixDataDTO permit);
        void EditPermitLicenseApplicationRegixData(CommercialFishingRegixDataDTO permitLicense);
        List<PermitLicenseForRenewalDTO> GetPermitLicensesForRenewal(int permitId, PageCodeEnum pageCode);
        List<PaymentTariffDTO> CalculatePermitLicenseAppliedTariffs(PermitLicenseTariffCalculationParameters tariffCalculationParameters);
        List<PermitLicenseForRenewalDTO> GetPermitLicensesForRenewal(string permitNumber, PageCodeEnum pageCode);
        SimpleAuditDTO GetPermitLicenseSimpleAudit(int id);
        SimpleAuditDTO GetPermitSuspensionSimpleAudit(int id);
        SimpleAuditDTO GetPermitLicenseSuspensionSimpleAudit(int id);
        CommercialFishingRegixDataDTO GetApplicationRegixData(int applicationId);
        void UpdatePermitsIsSuspendedFlag();
        void UpdatePermitLicensesIsSuspendedFlag();
    }
}
