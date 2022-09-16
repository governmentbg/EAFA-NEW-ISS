using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.ApplicationsRegister;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.ScientificFishing;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces;
using IARA.Interfaces.Nomenclatures;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Enums;
using IARA.WebHelpers.Models;
using Microsoft.AspNetCore.Mvc;

namespace IARA.Web.Controllers.Administrative
{
    [AreaRoute(AreaType.Administrative)]
    public class ScientificFishingAdministrationController : BaseAuditController
    {
        private readonly IScientificFishingService service;
        private readonly IScientificFishingNomenclaturesService nomenclatures;
        private readonly IFileService fileService;
        private readonly IApplicationService applicationService;
        private readonly IApplicationsRegisterService applicationsRegisterService;
        private readonly IUserService userService;
        private readonly IDeliveryService deliveryService;

        public ScientificFishingAdministrationController(IScientificFishingService service,
                                                         IScientificFishingNomenclaturesService nomenclatures,
                                                         IFileService fileService,
                                                         IPermissionsService permissionsService,
                                                         IApplicationService applicationService,
                                                         IApplicationsRegisterService applicationsRegisterService,
                                                         IUserService userService,
                                                         IDeliveryService deliveryService)
            : base(permissionsService)
        {
            this.service = service;
            this.nomenclatures = nomenclatures;
            this.fileService = fileService;
            this.applicationService = applicationService;
            this.applicationsRegisterService = applicationsRegisterService;
            this.userService = userService;
            this.deliveryService = deliveryService;
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ScientificFishingReadAll, Permissions.ScientificFishingRead)]
        public IActionResult GetAllPermits([FromBody] GridRequestModel<ScientificFishingFilters> request)
        {
            if (!CurrentUser.Permissions.Contains(Permissions.ScientificFishingReadAll))
            {
                int? territoryUnitId = userService.GetUserTerritoryUnitId(CurrentUser.ID);

                if (territoryUnitId.HasValue)
                {
                    if (request.Filters == null)
                    {
                        request.Filters = new ScientificFishingFilters
                        {
                            ShowInactiveRecords = false
                        };
                    }

                    request.Filters.TerritoryUnitId = territoryUnitId.Value;
                }
                else
                {
                    return Ok(Enumerable.Empty<ScientificFishingPermitDTO>());
                }
            }

            IQueryable<ScientificFishingPermitDTO> permits = service.GetAllPermits(request.Filters);
            GridResultModel<ScientificFishingPermitDTO> result = new(permits, request, false);
            service.SetPermitHoldersForTable(result.Records);

            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ScientificFishingReadAll, Permissions.ScientificFishingRead)]
        public IActionResult GetPermit([FromQuery] int id)
        {
            ScientificFishingPermitEditDTO permit = service.GetPermit(id);
            return Ok(permit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ScientificFishingApplicationsInspectAndCorrectRegiXData)]
        public IActionResult GetPermitRegixData([FromQuery] int applicationId)
        {
            RegixChecksWrapperDTO<ScientificFishingPermitRegixDataDTO> scientificPermitRegixData = this.service.GetPermitRegixData(applicationId);
            return Ok(scientificPermitRegixData);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ScientificFishingReadAll, Permissions.ScientificFishingRead)]
        public IActionResult GetApplicationDataForRegister([FromQuery] int applicationId)
        {
            ScientificFishingPermitEditDTO permit = service.GetApplicationDataForRegister(applicationId);
            return Ok(permit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ScientificFishingReadAll, Permissions.ScientificFishingRead)]
        public IActionResult GetRegisterByApplicationId([FromQuery] int applicationId)
        {
            ScientificFishingPermitEditDTO permit = service.GetRegisterByApplicationId(applicationId);
            return Ok(permit);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ScientificFishingAddRecords)]
        public IActionResult AddPermit([FromForm] ScientificFishingPermitEditDTO permit)
        {
            int id = service.AddPermit(permit);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ScientificFishingAddRecords)]
        public async Task<IActionResult> AddAndDownloadRegister([FromForm] ScientificFishingPermitEditDTO permit, [FromQuery] SciFiPrintTypesEnum printType)
        {
            int id = service.AddPermit(permit);

            DownloadableFileDTO file = await service.GetRegisterFileForDownload(id, printType);
            return File(file.Bytes, file.MimeType, file.FileName);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ScientificFishingEditRecords)]
        public IActionResult EditPermit([FromForm] ScientificFishingPermitEditDTO permit)
        {
            service.EditPermit(permit);
            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ScientificFishingEditRecords)]
        public async Task<IActionResult> EditAndDownloadRegister([FromForm] ScientificFishingPermitEditDTO permit, [FromQuery] SciFiPrintTypesEnum printType)
        {
            service.EditPermit(permit);

            DownloadableFileDTO file = await service.GetRegisterFileForDownload(permit.Id!.Value, printType);
            return File(file.Bytes, file.MimeType, file.FileName);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ScientificFishingReadAll,
                         Permissions.ScientificFishingRead,
                         Permissions.ScientificFishingApplicationsReadAll,
                         Permissions.ScientificFishingApplicationsRead)]
        public async Task<IActionResult> DownloadRegister([FromQuery] int id, [FromQuery] SciFiPrintTypesEnum printType)
        {
            DownloadableFileDTO file = await service.GetRegisterFileForDownload(id, printType);
            return File(file.Bytes, file.MimeType, file.FileName);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ScientificFishingReadAll,
                         Permissions.ScientificFishingRead,
                         Permissions.ScientificFishingApplicationsReadAll,
                         Permissions.ScientificFishingApplicationsRead)]
        public IActionResult GetPermitHolderPhoto([FromQuery] int holderId)
        {
            string photo = service.GetPermitHolderPhoto(holderId);
            return Ok(photo);
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.ScientificFishingDeleteRecords)]
        public IActionResult DeletePermit([FromQuery] int id)
        {
            service.DeletePermit(id);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.ScientificFishingRestoreRecords)]
        public IActionResult UndoDeletePermit([FromQuery] int id)
        {
            service.UndoDeletePermit(id);
            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ScientificFishingAddOutings)]
        public IActionResult AddOuting([FromBody] ScientificFishingOutingDTO outing)
        {
            int id = service.AddOuting(outing);
            return Ok(id);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ScientificFishingReadAll,
                         Permissions.ScientificFishingRead,
                         Permissions.ScientificFishingApplicationsReadAll,
                         Permissions.ScientificFishingApplicationsRead)]
        public IActionResult DownloadFile(int id)
        {
            DownloadableFileDTO file = fileService.GetFileForDownload(id);
            return File(file.Bytes, file.MimeType, file.FileName);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ScientificFishingReadAll,
                         Permissions.ScientificFishingRead,
                         Permissions.ScientificFishingApplicationsReadAll,
                         Permissions.ScientificFishingApplicationsRead)]
        public IActionResult GetPermitReasons()
        {
            List<ScientificFishingReasonNomenclatureDTO> reasons = nomenclatures.GetPermitReasons();
            return Ok(reasons);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ScientificFishingReadAll,
                         Permissions.ScientificFishingRead,
                         Permissions.ScientificFishingApplicationsReadAll,
                         Permissions.ScientificFishingApplicationsRead)]
        public IActionResult GetPermitStatuses()
        {
            List<NomenclatureDTO> statuses = nomenclatures.GetPermitStatuses();
            return Ok(statuses);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ScientificFishingReadAll,
                         Permissions.ScientificFishingRead,
                         Permissions.ScientificFishingApplicationsReadAll,
                         Permissions.ScientificFishingApplicationsRead)]
        public override IActionResult GetAuditInfo([FromQuery] int id)
        {
            SimpleAuditDTO audit = service.GetSimpleAudit(id);
            return Ok(audit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ScientificFishingReadAll,
                         Permissions.ScientificFishingRead,
                         Permissions.ScientificFishingApplicationsReadAll,
                         Permissions.ScientificFishingApplicationsRead)]
        public IActionResult GetPermitHolderSimpleAudit([FromQuery] int id)
        {
            SimpleAuditDTO audit = service.GetPermitHolderSimpleAudit(id);
            return Ok(audit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ScientificFishingReadAll, Permissions.ScientificFishingRead)]
        public IActionResult GetPermitOutingSimpleAudit([FromQuery] int id)
        {
            SimpleAuditDTO audit = service.GetPermitOutingSimpleAudit(id);
            return Ok(audit);
        }

        // Applications
        [HttpPost]
        [CustomAuthorize(Permissions.ScientificFishingApplicationsReadAll, Permissions.ScientificFishingApplicationsRead)]
        public IActionResult GetAllApplications([FromBody] GridRequestModel<ApplicationsRegisterFilters> request)
        {
            if (!CurrentUser.Permissions.Contains(Permissions.ScientificFishingApplicationsReadAll))
            {
                int? territoryUnitId = userService.GetUserTerritoryUnitId(CurrentUser.ID);

                if (territoryUnitId.HasValue)
                {
                    if (request.Filters == null)
                    {
                        request.Filters = new ApplicationsRegisterFilters
                        {
                            ShowInactiveRecords = false
                        };
                    }

                    request.Filters.TerritoryUnitId = territoryUnitId.Value;
                }
                else
                {
                    return Ok(Enumerable.Empty<ApplicationRegisterDTO>());
                }
            }

            IQueryable<ApplicationRegisterDTO> permits = applicationsRegisterService.GetAllApplications(request.Filters, null, new PageCodeEnum[] { PageCodeEnum.SciFi });
            return PageResult(permits, request, false);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ScientificFishingApplicationsEditRecords)]
        public IActionResult AssignApplicationViaAccessCode([FromQuery] string accessCode)
        {
            try
            {
                AssignedApplicationInfoDTO applicationData = applicationService.AssignApplicationViaAccessCode(accessCode, CurrentUser.ID, new PageCodeEnum[] { PageCodeEnum.SciFi });
                return Ok(applicationData);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException)
            {
                return ValidationFailedResult(errorCode: ErrorCode.InvalidStateMachineTransitionOperation);
            }
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ScientificFishingApplicationsReadAll, Permissions.ScientificFishingApplicationsRead)]
        public IActionResult GetPermitApplication([FromQuery] int id)
        {
            ScientificFishingApplicationEditDTO result = service.GetPermitApplication(id);
            return Ok(result);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ScientificFishingApplicationsAddRecords)]
        public async Task<IActionResult> AddPermitApplication([FromForm] ScientificFishingApplicationEditDTO permit)
        {
            bool hasDelivery = deliveryService.HasApplicationDelivery(permit.ApplicationId!.Value);
            if (hasDelivery != permit.HasDelivery!.Value)
            {
                throw new Exception("Mismatch between HasDelivery in model and in database");
            }

            if (hasDelivery)
            {
                ApplicationSubmittedForRegixDataDTO submittedFor = new ApplicationSubmittedForRegixDataDTO
                {
                    Legal = permit.Receiver,
                    Addresses = new List<AddressRegistrationDTO>(),
                    SubmittedByRole = permit.RequesterLetterOfAttorney != null
                        ? SubmittedByRolesEnum.LegalRepresentative
                        : SubmittedByRolesEnum.LegalOwner
                };

                ApplicationSubmittedByRegixDataDTO submittedBy = new ApplicationSubmittedByRegixDataDTO
                {
                    Person = permit.Requester,
                    Addresses = new List<AddressRegistrationDTO>()
                };

                bool hasEDelivery = await deliveryService.HasSubmittedForEDelivery(permit.DeliveryData.DeliveryTypeId,
                                                                                   submittedFor,
                                                                                   submittedBy);

                if (hasEDelivery == false)
                {
                    return ValidationFailedResult(new List<string> { nameof(ApplicationValidationErrorsEnum.NoEDeliveryRegistration) }, ErrorCode.NoEDeliveryRegistration);
                }
            }

            int id = service.AddPermitApplication(permit, ApplicationStatusesEnum.EXT_CHK_STARTED);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ScientificFishingApplicationsEditRecords)]
        public async Task<IActionResult> EditPermitApplication([FromQuery] bool fromSaveAsDraft, [FromForm] ScientificFishingApplicationEditDTO permit)
        {
            bool hasDelivery = deliveryService.HasApplicationDelivery(permit.ApplicationId!.Value);
            if (hasDelivery != permit.HasDelivery!.Value)
            {
                throw new Exception("Mismatch between HasDelivery in model and in database");
            }

            if (hasDelivery)
            {
                ApplicationSubmittedForRegixDataDTO submittedFor = new ApplicationSubmittedForRegixDataDTO
                {
                    Legal = permit.Receiver,
                    Addresses = new List<AddressRegistrationDTO>(),
                    SubmittedByRole = permit.RequesterLetterOfAttorney != null
                        ? SubmittedByRolesEnum.LegalRepresentative
                        : SubmittedByRolesEnum.LegalOwner
                };

                ApplicationSubmittedByRegixDataDTO submittedBy = new ApplicationSubmittedByRegixDataDTO
                {
                    Person = permit.Requester,
                    Addresses = new List<AddressRegistrationDTO>()
                };

                bool hasEDelivery = await deliveryService.HasSubmittedForEDelivery(permit.DeliveryData.DeliveryTypeId,
                                                                                   submittedFor,
                                                                                   submittedBy);

                if (hasEDelivery == false)
                {
                    return ValidationFailedResult(new List<string> { nameof(ApplicationValidationErrorsEnum.NoEDeliveryRegistration) }, ErrorCode.NoEDeliveryRegistration);
                }
            }

            if (fromSaveAsDraft)
            {
                service.EditPermitApplication(permit);
            }
            else
            {
                service.EditPermitApplication(permit, ApplicationStatusesEnum.EXT_CHK_STARTED);
            }

            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ScientificFishingApplicationsEditRecords)]
        public IActionResult EditPermitApplicationAndStartRegixChecks([FromForm] ScientificFishingPermitRegixDataDTO permit)
        {
            service.EditPermitApplicationRegixData(permit);
            return Ok();
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.ScientificFishingApplicationsDeleteRecords)]
        public IActionResult DeleteApplication([FromQuery] int id)
        {
            applicationsRegisterService.DeleteApplication(id);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.ScientificFishingApplicationsRestoreRecords)]
        public IActionResult UndoDeleteApplication([FromQuery] int id)
        {
            applicationsRegisterService.UndoDeleteApplication(id);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ScientificFishingApplicationsReadAll, Permissions.ScientificFishingApplicationsRead)]
        public IActionResult GetApplicationStatuses()
        {
            List<NomenclatureDTO> statuses = applicationsRegisterService.GetApplicationStatuses(ApplicationHierarchyTypesEnum.Online, ApplicationHierarchyTypesEnum.OnPaper);
            return Ok(statuses);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ScientificFishingApplicationsReadAll, Permissions.ScientificFishingApplicationsRead)]
        public IActionResult GetApplicationTypes()
        {
            List<NomenclatureDTO> types = applicationsRegisterService.GetApplicationTypes(PageCodeEnum.SciFi);
            return Ok(types);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ScientificFishingApplicationsReadAll, Permissions.ScientificFishingApplicationsRead)]
        public IActionResult GetApplicationSources()
        {
            List<NomenclatureDTO> sources = applicationsRegisterService.GetApplicationSources(ApplicationHierarchyTypesEnum.Online, ApplicationHierarchyTypesEnum.OnPaper);
            return Ok(sources);
        }
    }
}
