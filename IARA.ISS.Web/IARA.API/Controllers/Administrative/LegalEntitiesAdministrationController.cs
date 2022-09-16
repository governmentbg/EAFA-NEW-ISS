using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Enums;
using IARA.Common.Exceptions;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.ApplicationsRegister;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.LegalEntities;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces;
using IARA.Interfaces.Legals;
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
    public class LegalEntitiesAdministrationController : BaseAuditController
    {
        private readonly ILegalEntitiesService service;
        private readonly ILegalService legalService;
        private readonly IFileService fileService;
        private readonly IApplicationService applicationService;
        private readonly IApplicationsRegisterService applicationsRegisterService;
        private readonly IUserService userService;

        public LegalEntitiesAdministrationController(ILegalEntitiesService service,
                                                     ILegalService legalService,
                                                     IFileService fileService,
                                                     IPermissionsService permissionsService,
                                                     IApplicationService applicationService,
                                                     IApplicationsRegisterService applicationsRegisterService,
                                                     IUserService userService)
            : base(permissionsService)
        {
            this.service = service;
            this.legalService = legalService;
            this.fileService = fileService;
            this.applicationService = applicationService;
            this.applicationsRegisterService = applicationsRegisterService;
            this.userService = userService;
        }

        // Register
        [HttpPost]
        [CustomAuthorize(Permissions.LegalEntitiesReadAll, Permissions.LegalEntitiesRead)]
        public IActionResult GetAllLegalEntities([FromBody] GridRequestModel<LegalEntitiesFilters> request)
        {
            if (!CurrentUser.Permissions.Contains(Permissions.LegalEntitiesReadAll))
            {
                int? territoryUnitId = userService.GetUserTerritoryUnitId(CurrentUser.ID);

                if (territoryUnitId.HasValue)
                {
                    if (request.Filters == null)
                    {
                        request.Filters = new LegalEntitiesFilters
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

            IQueryable<LegalEntityDTO> result = service.GetAllLegalEntities(request.Filters);
            return PageResult(result, request, false);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.LegalEntitiesReadAll, Permissions.LegalEntitiesRead)]
        public IActionResult GetLegalEntity([FromQuery] int id)
        {
            LegalEntityEditDTO legal = service.GetLegalEntity(id);
            return Ok(legal);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.LegalEntitiesApplicationsInspectAndCorrectRegiXData)]
        public IActionResult GetLegalEntityRegixData([FromQuery] int applicationId)
        {
            RegixChecksWrapperDTO<LegalEntityRegixDataDTO> data = service.GetLegalEntityRegixData(applicationId);
            return Ok(data);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.LegalEntitiesReadAll, Permissions.LegalEntitiesRead)]
        public IActionResult GetApplicationDataForRegister([FromQuery] int applicationId)
        {
            LegalEntityEditDTO legal = service.GetApplicationDataForRegister(applicationId);
            return Ok(legal);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.LegalEntitiesReadAll, Permissions.LegalEntitiesRead)]
        public IActionResult GetRegisterByApplicationId([FromQuery] int applicationId)
        {
            LegalEntityEditDTO permit = service.GetRegisterByApplicationId(applicationId);
            return Ok(permit);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.LegalEntitiesAddRecords)]
        public IActionResult AddLegalEntity([FromForm] LegalEntityEditDTO legalEntity)
        {
            try
            {
                int id = service.AddLegalEntity(legalEntity);
                return Ok(id);
            }
            catch (ObjectException error)
            {
                return BadRequest(error.Object);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.LegalEntitiesEditRecords)]
        public IActionResult EditLegalEntity([FromForm] LegalEntityEditDTO legalEntity)
        {
            try
            {
                service.EditLegalEntity(legalEntity);
                return Ok();
            }
            catch (ObjectException error)
            {
                return BadRequest(error.Object);
            }
        }

        [HttpGet]
        [CustomAuthorize(Permissions.LegalEntitiesReadAll,
                         Permissions.LegalEntitiesRead,
                         Permissions.LegalEntitiesApplicationsReadAll,
                         Permissions.LegalEntitiesApplicationsRead)]
        public override IActionResult GetAuditInfo(int id)
        {
            SimpleAuditDTO audit = service.GetSimpleAudit(id);
            return Ok(audit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.LegalEntitiesReadAll,
                         Permissions.LegalEntitiesRead,
                         Permissions.LegalEntitiesApplicationsReadAll,
                         Permissions.LegalEntitiesApplicationsRead)]
        public IActionResult DownloadFile(int id)
        {
            DownloadableFileDTO file = fileService.GetFileForDownload(id);
            return File(file.Bytes, file.MimeType, file.FileName);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.LegalEntitiesReadAll,
                         Permissions.LegalEntitiesRead,
                         Permissions.LegalEntitiesApplicationsReadAll,
                         Permissions.LegalEntitiesApplicationsRead)]
        public IActionResult GetAuthorizedPersonSimpleAudit([FromQuery] int id)
        {
            SimpleAuditDTO audit = service.GetAuthorizedPersonSimpleAudit(id);
            return Ok(audit);
        }

        // nomenclatures
        [HttpGet]
        [CustomAuthorize(Permissions.LegalEntitiesReadAll,
                         Permissions.LegalEntitiesRead,
                         Permissions.LegalEntitiesApplicationsReadAll,
                         Permissions.LegalEntitiesApplicationsRead)]
        public IActionResult GetActiveLegals()
        {
            List<NomenclatureDTO> result = legalService.GetActiveLegals();
            return Ok(result);
        }

        // Applications
        [HttpPost]
        [CustomAuthorize(Permissions.LegalEntitiesApplicationsReadAll, Permissions.LegalEntitiesApplicationsRead)]
        public IActionResult GetAllApplications([FromBody] GridRequestModel<ApplicationsRegisterFilters> request)
        {
            if (!CurrentUser.Permissions.Contains(Permissions.LegalEntitiesApplicationsReadAll))
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

            IQueryable<ApplicationRegisterDTO> legalApplications = applicationsRegisterService.GetAllApplications(request.Filters, null, new PageCodeEnum[] { PageCodeEnum.LE });
            return PageResult(legalApplications, request, false);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.LegalEntitiesApplicationsReadAll, Permissions.LegalEntitiesApplicationsRead)]
        public IActionResult GetLegalEntityApplication([FromQuery] int id)
        {
            LegalEntityApplicationEditDTO result = service.GetLegalEntityApplication(id);
            return Ok(result);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.LegalEntitiesApplicationsAddRecords)]
        public IActionResult AddLegalEntityApplication([FromForm] LegalEntityApplicationEditDTO legalEntity)
        {
            try
            {
                int id = service.AddLegalEntityApplication(legalEntity, ApplicationStatusesEnum.EXT_CHK_STARTED);
                return Ok(id);
            }
            catch (ObjectException error)
            {
                return BadRequest(error.Object);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.LegalEntitiesApplicationsEditRecords)]
        public IActionResult EditLegalEntityApplication([FromQuery] bool saveAsDraft, [FromForm] LegalEntityApplicationEditDTO legalEntity)
        {
            try
            {
                if (saveAsDraft)
                {
                    service.EditLegalEntityApplication(legalEntity);
                }
                else
                {
                    service.EditLegalEntityApplication(legalEntity, ApplicationStatusesEnum.EXT_CHK_STARTED);
                }
                return Ok();
            }
            catch (ObjectException error)
            {
                return BadRequest(error.Object);
            }
        }

        [HttpPut]
        [CustomAuthorize(Permissions.LegalEntitiesApplicationsEditRecords)]
        public IActionResult EditLegalEntityApplicationAndStartRegixChecks([FromBody] LegalEntityRegixDataDTO legalEntity)
        {
            try
            {
                service.EditLegalEntityApplicationRegixData(legalEntity);
                return Ok();
            }
            catch (ObjectException error)
            {
                return BadRequest(error.Object);
            }
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.LegalEntitiesApplicationsDeleteRecords)]
        public IActionResult DeleteApplication([FromQuery] int id)
        {
            applicationsRegisterService.DeleteApplication(id);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.LegalEntitiesApplicationsRestoreRecords)]
        public IActionResult UndoDeleteApplication([FromQuery] int id)
        {
            applicationsRegisterService.UndoDeleteApplication(id);
            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.LegalEntitiesApplicationsEditRecords)]
        public IActionResult AssignApplicationViaAccessCode([FromQuery] string accessCode)
        {
            try
            {
                AssignedApplicationInfoDTO applicationData = applicationService.AssignApplicationViaAccessCode(accessCode, CurrentUser.ID, new PageCodeEnum[] { PageCodeEnum.LE });
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
        [CustomAuthorize(Permissions.LegalEntitiesApplicationsReadAll, Permissions.LegalEntitiesApplicationsRead)]
        public IActionResult GetApplicationStatuses()
        {
            List<NomenclatureDTO> statuses = applicationsRegisterService.GetApplicationStatuses(ApplicationHierarchyTypesEnum.Online, ApplicationHierarchyTypesEnum.OnPaper);
            return Ok(statuses);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.LegalEntitiesApplicationsReadAll, Permissions.LegalEntitiesApplicationsRead)]
        public IActionResult GetApplicationTypes()
        {
            List<NomenclatureDTO> types = applicationsRegisterService.GetApplicationTypes(PageCodeEnum.LE);
            return Ok(types);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.LegalEntitiesApplicationsReadAll, Permissions.LegalEntitiesApplicationsRead)]
        public IActionResult GetApplicationSources()
        {
            List<NomenclatureDTO> sources = applicationsRegisterService.GetApplicationSources(ApplicationHierarchyTypesEnum.Online, ApplicationHierarchyTypesEnum.OnPaper);
            return Ok(sources);
        }
    }
}
