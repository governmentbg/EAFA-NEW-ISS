using System;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;
using IARA.Interfaces;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace IARA.Web.Controllers.Administrative
{
    [AreaRoute(AreaType.Administrative)]
    public class ApplicationsAdministrationController : BaseController
    {
        private readonly IApplicationService service;
        private readonly IFileService fileService;

        public ApplicationsAdministrationController(IApplicationService service,
                                                    IPermissionsService permissions,
                                                    IFileService fileService)
            : base(permissions)
        {
            this.service = service;
            this.fileService = fileService;
        }

        [HttpPost]
        [CustomAuthorize(Permissions.AssociationsAddRecords,
                         Permissions.BuyersAddRecords,
                         Permissions.LegalEntitiesAddRecords,
                         Permissions.QualifiedFishersAddRecords,
                         Permissions.ScientificFishingAddRecords,
                         Permissions.ShipsRegisterAddRecords,
                         Permissions.CommercialFishingPermitRegisterAddRecords,
                         Permissions.CommercialFishingPermitLicenseRegisterAddRecords,
                         Permissions.StatisticalFormsAquaFarmAddRecords,
                         Permissions.StatisticalFormsFishVesselAddRecords,
                         Permissions.StatisticalFormsReworkAddRecords,
                         Permissions.AquacultureFacilitiesAddRecords,
                         Permissions.FishingCapacityAddRecords)]
        public IActionResult GetApplicationsForChoice([FromBody] PageCodeEnum[] pageCodes)
        {
            return Ok(this.service.GetApplicationsForChoice(pageCodes, CurrentUser.ID));
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ApplicationsEnterEventisNumber)]
        public IActionResult GetApplicationAccessCode([FromQuery] int applicationId)
        {
            string accessCode = this.service.GetApplicationAccessCode(applicationId);

            return Ok(accessCode);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsProcessPaymentData,
                         Permissions.BuyersApplicationsProcessPaymentData,
                         Permissions.CommercialFishingPermitApplicationsProcessPaymentData,
                         Permissions.CommercialFishingPermitLicenseApplicationsProcessPaymentData,
                         Permissions.QualifiedFishersApplicationsProcessPaymentData,
                         Permissions.ScientificFishingApplicationsProcessPaymentData,
                         Permissions.LegalEntitiesApplicationsProcessPaymentData,
                         Permissions.FishingCapacityApplicationsProcessPaymentData,
                         Permissions.ShipsRegisterApplicationsProcessPaymentData,
                         Permissions.StatisticalFormsAquaFarmApplicationsProcessPaymentData,
                         Permissions.StatisticalFormsReworkApplicationsProcessPaymentData,
                         Permissions.StatisticalFormsFishVesselsApplicationsProcessPaymentData,
                         Permissions.TicketApplicationsProcessPaymentData,
                         Permissions.AssociationsTicketApplicationsProcessPaymentData)]
        public IActionResult GetApplicationPaymentSummary([FromQuery] int applicationId)
        {
            PaymentSummaryDTO result = service.GetApplicationPaymentSummary(applicationId);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsReadAll, Permissions.AquacultureFacilitiesApplicationsRead,
                         Permissions.BuyersApplicationsReadAll, Permissions.BuyersApplicationsRead,
                         Permissions.CommercialFishingPermitApplicationsReadAll, Permissions.CommercialFishingPermitApplicationsRead,
                         Permissions.CommercialFishingPermitLicenseApplicationsReadAll, Permissions.CommercialFishingPermitLicenseApplicationsRead,
                         Permissions.QualifiedFishersApplicationsReadAll, Permissions.QualifiedFishersApplicationsRead,
                         Permissions.LegalEntitiesApplicationsReadAll, Permissions.LegalEntitiesApplicationsRead,
                         Permissions.ScientificFishingApplicationsReadAll, Permissions.ScientificFishingApplicationsRead,
                         Permissions.ShipsRegisterApplicationReadAll, Permissions.ShipsRegisterApplicationsRead,
                         Permissions.FishingCapacityApplicationsReadAll, Permissions.FishingCapacityApplicationsRead,
                         Permissions.StatisticalFormsAquaFarmApplicationsReadAll, Permissions.StatisticalFormsAquaFarmApplicationsRead,
                         Permissions.StatisticalFormsReworkApplicationsReadAll, Permissions.StatisticalFormsReworkApplicationsRead,
                         Permissions.StatisticalFormsFishVesselsApplicationsReadAll, Permissions.StatisticalFormsFishVesselsApplicationsRead)]
        public IActionResult GetApplicationChangeHistoryContent([FromQuery] int applicationChangeHistoryId)
        {
            ApplicationContentDTO result = service.GetApplicationChangeHistoryContent(applicationChangeHistoryId);
            return Ok(result);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ApplicationsAddRecords)]
        public IActionResult AddApplication([FromBody] int applicationTypeId)
        {
            Tuple<int, string> applicationInfo = this.service.AddApplication(applicationTypeId,
                                                                             ApplicationHierarchyTypesEnum.OnPaper,
                                                                             CurrentUser.ID);
            return Ok(applicationInfo);
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.ApplicationsEnterEventisNumber)]
        public IActionResult EnterEventisNumber([FromQuery] int applicationId, [FromBody] EventisDataDTO eventisData)
        {
            service.UpdateEventisNumber(applicationId, eventisData);
            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ApplicationsEditRecords,
                         Permissions.AquacultureFacilitiesApplicationsEditRecords,
                         Permissions.BuyersApplicationsEditRecords,
                         Permissions.CommercialFishingPermitApplicationsEditRecords,
                         Permissions.CommercialFishingPermitLicenseApplicationsEditRecords,
                         Permissions.QualifiedFishersApplicationsEditRecords,
                         Permissions.ScientificFishingApplicationsEditRecords,
                         Permissions.LegalEntitiesApplicationsEditRecords,
                         Permissions.FishingCapacityApplicationsEditRecords,
                         Permissions.AssociationsTicketApplicationsEditRecords,
                         Permissions.ShipsRegisterApplicationsEditRecords,
                         Permissions.StatisticalFormsAquaFarmApplicationsEditRecords,
                         Permissions.StatisticalFormsReworkApplicationsEditRecords,
                         Permissions.StatisticalFormsFishVesselsApplicationsEditRecords)]
        public IActionResult UpdateDraftContent([FromForm] ApplicationContentDTO applicationContent)
        {
            service.UpdateDraftContent(applicationContent.ApplicationId, applicationContent.DraftContent, applicationContent.Files);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsCancel,
                         Permissions.BuyersApplicationsCancel,
                         Permissions.CommercialFishingPermitApplicationsCancel,
                         Permissions.CommercialFishingPermitLicenseApplicationsCancel,
                         Permissions.QualifiedFishersApplicationsCancel,
                         Permissions.ScientificFishingApplicationsCancel,
                         Permissions.LegalEntitiesApplicationsCancel,
                         Permissions.FishingCapacityApplicationsCancel,
                         Permissions.AssociationsTicketApplicationsCancel,
                         Permissions.ApplicationsCancelRecords,
                         Permissions.ShipsRegisterApplicationsCancel,
                         Permissions.TicketApplicationsCancelRecord,
                         Permissions.StatisticalFormsAquaFarmApplicationsCancel,
                         Permissions.StatisticalFormsReworkApplicationsCancel,
                         Permissions.StatisticalFormsFishVesselsApplicationsCancel)]
        public IActionResult ApplicationAnnulment([FromQuery] int applicationId, [FromBody] ReasonDTO reasonData)
        {
            service.ApplicationAnnulment(applicationId, reasonData.Reason);
            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ApplicationsEditRecords,
                         Permissions.AquacultureFacilitiesApplicationsEditRecords,
                         Permissions.BuyersApplicationsEditRecords,
                         Permissions.CommercialFishingPermitApplicationsEditRecords,
                         Permissions.CommercialFishingPermitLicenseApplicationsEditRecords,
                         Permissions.QualifiedFishersApplicationsEditRecords,
                         Permissions.ScientificFishingApplicationsEditRecords,
                         Permissions.LegalEntitiesApplicationsEditRecords,
                         Permissions.FishingCapacityApplicationsEditRecords,
                         Permissions.AssociationsTicketApplicationsEditRecords,
                         Permissions.ShipsRegisterApplicationsEditRecords,
                         Permissions.StatisticalFormsAquaFarmApplicationsEditRecords,
                         Permissions.StatisticalFormsReworkApplicationsEditRecords,
                         Permissions.StatisticalFormsFishVesselsApplicationsEditRecords,
                         Permissions.TicketApplicationsEditRecords)]
        public IActionResult InitiateApplicationCorrections([FromQuery] int applicationId)
        {
            ApplicationStatusesEnum newStatus = this.service.InitiateApplicationCorrections(applicationId);
            return Ok(newStatus);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ApplicationsInspectAndCorrectRegiXData,
                         Permissions.AquacultureFacilitiesApplicationsInspectAndCorrectRegiXData,
                         Permissions.BuyersApplicationsInspectAndCorrectRegiXData,
                         Permissions.CommercialFishingPermitApplicationsInspectAndCorrectRegiXData,
                         Permissions.CommercialFishingPermitLicenseApplicationsInspectAndCorrectRegiXData,
                         Permissions.QualifiedFishersApplicationsInspectAndCorrectRegiXData,
                         Permissions.ScientificFishingApplicationsInspectAndCorrectRegiXData,
                         Permissions.LegalEntitiesApplicationsInspectAndCorrectRegiXData,
                         Permissions.FishingCapacityApplicationsInspectAndCorrectRegiXData,
                         Permissions.ShipsRegisterApplicationsInspectAndCorrectRegiXData,
                         Permissions.StatisticalFormsAquaFarmApplicationsInspectAndCorrectRegiXData,
                         Permissions.StatisticalFormsReworkApplicationsInspectAndCorrectRegiXData,
                         Permissions.StatisticalFormsFishVesselsApplicationsInspectAndCorrectRegiXData,
                         Permissions.TicketApplicationsInspectAndCorrectRegiXData,
                         Permissions.AssociationsTicketApplicationsInspectAndCorrectRegiXData)]
        public IActionResult SendApplicationForUserCorrections([FromQuery] int applicationId, [FromBody] ReasonDTO statusReason)
        {
            ApplicationStatusesEnum newStatus = this.service.SendApplicationForUserCorrections(applicationId, statusReason.Reason);
            return Ok(newStatus);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ApplicationsInspectAndCorrectRegiXData,
                         Permissions.AquacultureFacilitiesApplicationsInspectAndCorrectRegiXData,
                         Permissions.BuyersApplicationsInspectAndCorrectRegiXData,
                         Permissions.CommercialFishingPermitApplicationsInspectAndCorrectRegiXData,
                         Permissions.CommercialFishingPermitLicenseApplicationsInspectAndCorrectRegiXData,
                         Permissions.QualifiedFishersApplicationsInspectAndCorrectRegiXData,
                         Permissions.ScientificFishingApplicationsInspectAndCorrectRegiXData,
                         Permissions.LegalEntitiesApplicationsInspectAndCorrectRegiXData,
                         Permissions.FishingCapacityApplicationsInspectAndCorrectRegiXData,
                         Permissions.ShipsRegisterApplicationsInspectAndCorrectRegiXData,
                         Permissions.StatisticalFormsAquaFarmApplicationsInspectAndCorrectRegiXData,
                         Permissions.StatisticalFormsReworkApplicationsInspectAndCorrectRegiXData,
                         Permissions.StatisticalFormsFishVesselsApplicationsInspectAndCorrectRegiXData,
                         Permissions.TicketApplicationsInspectAndCorrectRegiXData,
                         Permissions.AssociationsTicketApplicationsInspectAndCorrectRegiXData)]
        public IActionResult ConfirmNoErrorsForApplication([FromQuery] int applicationId)
        {
            ApplicationStatusesEnum newStatus = this.service.ConfirmNoErrorsForApplication(applicationId);
            return Ok(newStatus);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsCheckDataRegularity,
                         Permissions.BuyersApplicationsCheckDataRegularity,
                         Permissions.CommercialFishingPermitApplicationsCheckDataRegularity,
                         Permissions.CommercialFishingPermitLicenseApplicationsCheckDataRegularity,
                         Permissions.QualifiedFishersApplicationsCheckDataRegularity,
                         Permissions.ScientificFishingApplicationsCheckDataRegularity,
                         Permissions.LegalEntitiesApplicationsCheckDataRegularity,
                         Permissions.FishingCapacityApplicationsCheckDataRegularity,
                         Permissions.ShipsRegisterApplicationsCheckDataRegularity,
                         Permissions.StatisticalFormsAquaFarmApplicationsCheckDataRegularity,
                         Permissions.StatisticalFormsReworkApplicationsCheckDataRegularity,
                         Permissions.StatisticalFormsFishVesselsApplicationsCheckDataRegularity,
                         Permissions.TicketApplicationsCheckDataRegularity,
                         Permissions.AssociationsTicketApplicationsCheckDataRegularity)]
        public IActionResult ConfirmApplicationDataIrregularity([FromQuery] int applicationId, [FromBody] ReasonDTO reasonData)
        {
            ApplicationStatusesEnum newStatus = this.service.ConfirmApplicationDataIrregularity(applicationId, reasonData.Reason);
            return Ok(newStatus);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsCheckDataRegularity,
                         Permissions.BuyersApplicationsCheckDataRegularity,
                         Permissions.CommercialFishingPermitApplicationsCheckDataRegularity,
                         Permissions.CommercialFishingPermitLicenseApplicationsCheckDataRegularity,
                         Permissions.QualifiedFishersApplicationsCheckDataRegularity,
                         Permissions.ScientificFishingApplicationsCheckDataRegularity,
                         Permissions.LegalEntitiesApplicationsCheckDataRegularity,
                         Permissions.FishingCapacityApplicationsCheckDataRegularity,
                         Permissions.ShipsRegisterApplicationsCheckDataRegularity,
                         Permissions.StatisticalFormsAquaFarmApplicationsCheckDataRegularity,
                         Permissions.StatisticalFormsReworkApplicationsCheckDataRegularity,
                         Permissions.StatisticalFormsFishVesselsApplicationsCheckDataRegularity,
                         Permissions.TicketApplicationsCheckDataRegularity,
                         Permissions.AssociationsTicketApplicationsCheckDataRegularity)]
        public IActionResult ConfirmApplicationDataRegularity([FromQuery] int applicationId)
        {
            ApplicationStatusesEnum newStatus = this.service.ConfirmApplicationDataRegularity(applicationId);
            return Ok(newStatus);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsProcessPaymentData,
                         Permissions.BuyersApplicationsProcessPaymentData,
                         Permissions.CommercialFishingPermitApplicationsProcessPaymentData,
                         Permissions.CommercialFishingPermitLicenseApplicationsProcessPaymentData,
                         Permissions.QualifiedFishersApplicationsProcessPaymentData,
                         Permissions.ScientificFishingApplicationsProcessPaymentData,
                         Permissions.LegalEntitiesApplicationsProcessPaymentData,
                         Permissions.FishingCapacityApplicationsProcessPaymentData,
                         Permissions.ShipsRegisterApplicationsProcessPaymentData,
                         Permissions.StatisticalFormsAquaFarmApplicationsProcessPaymentData,
                         Permissions.StatisticalFormsReworkApplicationsProcessPaymentData,
                         Permissions.StatisticalFormsFishVesselsApplicationsProcessPaymentData,
                         Permissions.TicketApplicationsProcessPaymentData,
                         Permissions.AssociationsTicketApplicationsProcessPaymentData)]
        public IActionResult EnterApplicationPaymentData([FromQuery] int applicationId, [FromBody] PaymentDataDTO paymentData)
        {
            ApplicationStatusesEnum newStatus = this.service.EnterApplicationPaymentData(applicationId, paymentData);
            return Ok(newStatus);
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsProcessPaymentData,
                         Permissions.BuyersApplicationsProcessPaymentData,
                         Permissions.CommercialFishingPermitApplicationsProcessPaymentData,
                         Permissions.CommercialFishingPermitLicenseApplicationsProcessPaymentData,
                         Permissions.QualifiedFishersApplicationsProcessPaymentData,
                         Permissions.ScientificFishingApplicationsProcessPaymentData,
                         Permissions.LegalEntitiesApplicationsProcessPaymentData,
                         Permissions.FishingCapacityApplicationsProcessPaymentData,
                         Permissions.ShipsRegisterApplicationsProcessPaymentData,
                         Permissions.StatisticalFormsAquaFarmApplicationsProcessPaymentData,
                         Permissions.StatisticalFormsReworkApplicationsProcessPaymentData,
                         Permissions.StatisticalFormsFishVesselsApplicationsProcessPaymentData,
                         Permissions.TicketApplicationsProcessPaymentData,
                         Permissions.AssociationsTicketApplicationsProcessPaymentData)]
        public IActionResult ApplicationPaymentRefusal([FromQuery] int applicationId, [FromBody] ReasonDTO reasonData)
        {
            service.ApplicationPaymentRefusal(applicationId, reasonData.Reason);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ApplicationsRead)]
        public IActionResult DownloadFile(int id)
        {
            DownloadableFileDTO file = fileService.GetFileForDownload(id);
            return File(file.Bytes, file.MimeType, file.FileName);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ApplicationsInspectAndCorrectRegiXData)]
        public IActionResult ManualRegixChecksStart([FromQuery] int applicationId)
        {
            ApplicationStatusesEnum newStatus = this.service.StartRegixChecks(applicationId);
            return Ok(newStatus);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ApplicationsRead)]
        public IActionResult GetApplicationTypesForChoice()
        {
            return Ok(this.service.GetApplicationTypesForChoice(ApplicationHierarchyTypesEnum.OnPaper));
        }
    }
}
