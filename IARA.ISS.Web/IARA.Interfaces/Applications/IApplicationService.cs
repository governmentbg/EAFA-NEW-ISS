using System;
using System.Collections.Generic;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.Nomenclatures;

namespace IARA.Interfaces
{
    public interface IApplicationService
    {
        List<ApplicationForChoiceDTO> GetApplicationsForChoice(PageCodeEnum[] pageCodes, int assignedUserId);

        List<ApplicationRegiXCheckDTO> GetLatestApplicationChangeHistoryRegiXChecks(int applicationChangeHistoryId);

        List<ApplicationTypeDTO> GetApplicationTypesForChoice(ApplicationHierarchyTypesEnum applicationHierarchyType);

        ApplicationStatusesEnum GetApplicationCurrentStatusCode(int applicationId);

        ApplicationContentDTO GetApplicationChangeHistoryContent(int applicationChangeHistoryId);

        int GetLastGeneratedApplicationFileId(int applicationId);

        string GetApplicationAccessCode(int applicationId);

        PaymentSummaryDTO GetApplicationPaymentSummary(int applicationId);

        List<PaymentTariffDTO> GetApplicationAppliedTariffs(int applicationId);

        bool IsApplicationPaid(int applicationId);

        ApplicationPaymentInformationDTO GetApplicationPaymentInformation(int applicationId);

        Tuple<int, string> AddApplication(int applicationTypeId,
                                          ApplicationHierarchyTypesEnum applicationHierarchyType,
                                          int currentUserId,
                                          ApplicationSubmissionDTO applicationSubmission = null);

        ApplicationStatusesEnum UpdateEventisNumber(int applicationId, EventisDataDTO eventisData);

        AssignedApplicationInfoDTO AssignApplicationViaAccessCode(string accessCode, int userId, PageCodeEnum[] pageCodes);

        ApplicationStatusesEnum UpdateDraftContent(int applicationId, string draftContent, List<FileInfoDTO> files);

        ApplicationStatusesEnum ApplicationAnnulment(int applicationId, string reason);

        string GetSubmittedByRoleCodeById(int id);

        int GetSubmittedByRoleId(SubmittedByRolesEnum role);

        ApplicationSubmittedByDTO GetUserAsSubmittedBy(int userId);

        ApplicationSubmittedByRegixDataDTO GetApplicationSubmittedByRegixData(int applicationId);

        ApplicationSubmittedByDTO GetApplicationSubmittedBy(int applicationId);

        ApplicationSubmittedForRegixDataDTO GetApplicationSubmittedForRegixData(int applicationId);

        ApplicationSubmittedForDTO GetApplicationSubmittedFor(int applicationId);

        ApplicationSubmittedForDTO GetRegisterSubmittedFor(int applicationId, int? submittedForPersonId, int? submittedForLegalId);

        LetterOfAttorneyDTO GetLetterOfAttorney(int applicationId);

        List<TariffNomenclatureDTO> GetApplicationTypeActiveTariffs(int applicationTypeId, bool onlyCalculated = false);

        PaymentStatusesEnum CalculatePaymentStatus(PaymentDataDTO paymentData, decimal price);

        PaymentStatusesEnum CalculatePaymentStatus(PaymentDataDTO paymentData, IEnumerable<decimal> prices);

        ApplicationStatusesEnum InitiateApplicationCorrections(int applicationId);

        ApplicationStatusesEnum SendApplicationForUserCorrections(int applicationId, string statusReason);

        ApplicationStatusesEnum ConfirmApplicationDataIrregularity(int applicationId, string statusReason);

        ApplicationStatusesEnum ConfirmApplicationDataRegularity(int applicationId);

        ApplicationStatusesEnum ConfirmNoErrorsForApplication(int applicationId);

        ApplicationStatusesEnum EnterApplicationPaymentData(int applicationId, PaymentDataDTO paymentData);

        void EnterOfflineTicketApplicationPaymentData(int applicationId, PaymentDataDTO paymentData);

        ApplicationStatusesEnum RenewApplicationPayment(int applicationId);

        ApplicationStatusesEnum ApplicationPaymentRefusal(int applicationId, string reason);

        ApplicationStatusesEnum UploadSignedApplication(int applicationId, FileInfoDTO fileInfo);

        ApplicationStatusesEnum StartRegixChecks(int applicationId);

        ApplicationStatusesEnum CompleteApplicationFillingByApplicant(int applicationId);

        ApplicationStatusesEnum InitiateManualApplicationFillByApplicant(int applicationId);

        bool IsApplicationSubmittedByUser(int userId, int applicationId);

        bool IsApplicationSubmittedByUserOrPerson(int userId, int applicationId);

        bool AreApplicationsSubmittedByUser(int userId, List<int> applicationIds);

        bool AreApplicationsSubmittedByUserOrPerson(int userId, List<int> applicationIds);

        bool IsApplicationHierarchyType(int applicationId, ApplicationHierarchyTypesEnum type);
    }
}
