using System.Collections.Generic;
using IARA.Common.Constants;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;
using IARA.Interfaces;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace IARA.Web.Controllers.Public
{
    // TODO add security checks
    [AreaRoute(AreaType.Public)]
    public class ApplicationsPublicController : BaseController
    {
        private readonly IApplicationService service;
        private readonly IFileService fileService;

        public ApplicationsPublicController(IApplicationService service, IPermissionsService permissions, IFileService fileService)
            : base(permissions)
        {
            this.service = service;
            this.fileService = fileService;
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsAddRecords)]
        public IActionResult GetApplicationTypesForChoice()
        {
            return Ok(this.service.GetApplicationTypesForChoice(ApplicationHierarchyTypesEnum.Online));
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead)]
        public IActionResult GetApplicationChangeHistoryContent([FromQuery] int applicationChangeHistoryId)
        {
            ApplicationContentDTO result = service.GetApplicationChangeHistoryContent(applicationChangeHistoryId);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize()]
        public IActionResult GetCurrentUserAsSubmittedBy()
        {
            ApplicationSubmittedByDTO result = service.GetUserAsSubmittedBy(CurrentUser.ID);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsProcessPaymentData)]
        public IActionResult GetApplicationPaymentSummary([FromQuery] int applicationId)
        {
            PaymentSummaryDTO result = service.GetApplicationPaymentSummary(applicationId);
            return Ok(result);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsAddRecords)]
        public IActionResult AddApplication([FromQuery] int applicationTypeId)
        {
            return Ok(this.service.AddApplication(applicationTypeId, ApplicationHierarchyTypesEnum.Online, CurrentUser.ID));
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsEditRecords)]
        public IActionResult UpdateDraftContent([FromForm] ApplicationContentDTO applicationContent)
        {
            this.service.UpdateDraftContent(applicationContent.ApplicationId, applicationContent.DraftContent, applicationContent.Files);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsCancelRecords)]
        public IActionResult ApplicationAnnulment([FromQuery] int applicationId, [FromBody] ReasonDTO reasonData)
        {
            this.service.ApplicationAnnulment(applicationId, reasonData.Reason);
            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsEditRecords, Permissions.TicketApplicationsEdit)]
        public IActionResult InitiateApplicationCorrections([FromQuery] int applicationId)
        {
            ApplicationStatusesEnum newStatus = this.service.InitiateApplicationCorrections(applicationId);
            return Ok(newStatus);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsProcessPaymentData)]
        public IActionResult RenewApplicationPayment([FromQuery] int applicationId)
        {
            ApplicationStatusesEnum newStatus = this.service.RenewApplicationPayment(applicationId);
            return Ok(newStatus);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.AssociationsTicketApplicationsProcessPaymentData)]
        public IActionResult EnterApplicationPaymentData([FromQuery] int applicationId, [FromBody] PaymentDataDTO paymentData)
        {
            ApplicationStatusesEnum newStatus = this.service.EnterApplicationPaymentData(applicationId, paymentData);
            return Ok(newStatus);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult DownloadFile([FromQuery] int id)
        {
            DownloadableFileDTO file = fileService.GetFileForDownload(id);

            return File(file.Bytes, file.MimeType, file.FileName);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsEditRecords, Permissions.OnlineSubmittedApplicationsDownloadRecords)]
        public IActionResult CompleteApplicationFillingByApplicantAndDownload([FromQuery] int applicationId)
        {
            service.CompleteApplicationFillingByApplicant(applicationId);

            int fileId = service.GetLastGeneratedApplicationFileId(applicationId);
            DownloadableFileDTO file = fileService.GetFileForDownload(fileId);

            return File(file.Bytes, file.MimeType, file.FileName);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsDownloadRecords)]
        public IActionResult DownloadGeneratedApplicationFile([FromQuery] int applicationId)
        {
            int fileId = service.GetLastGeneratedApplicationFileId(applicationId);
            DownloadableFileDTO file = fileService.GetFileForDownload(fileId);
            return File(file.Bytes, file.MimeType, file.FileName);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsUploadRecords)]
        public IActionResult UploadSignedApplicationAndStartRegixChecks([FromQuery] int applicationId, [FromForm] FileInfoDTO fileInfo)
        {
            if (fileInfo.ContentType != MimeTypes.PDF)
            {
                return ValidationFailedResult(new List<string> { ErrorResources.msgInvalidApplicationFileType });
            }
            else
            {
                ApplicationStatusesEnum newStatus = this.service.UploadSignedApplication(applicationId, fileInfo);
                return Ok(newStatus);
            }
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsEditRecords)]
        public IActionResult GoBackToFillApplicationByApplicant([FromQuery] int applicationId)
        {
            this.service.InitiateManualApplicationFillByApplicant(applicationId);
            return Ok();
        }
    }
}
