using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.Common.Exceptions;
using IARA.DomainModels.DTOInterfaces;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.ApplicationsRegister;
using IARA.DomainModels.DTOModels.AquacultureFacilities;
using IARA.DomainModels.DTOModels.AquacultureFacilities.ChangeOfCircumstances;
using IARA.DomainModels.DTOModels.AquacultureFacilities.Deregistration;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Excel.Tools.Models;
using IARA.Interfaces;
using IARA.Interfaces.CatchSales;
using IARA.Interfaces.Nomenclatures;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Enums;
using IARA.WebHelpers.Models;
using Microsoft.AspNetCore.Mvc;

namespace IARA.WebAPI.Controllers.Administrative
{
    [AreaRoute(AreaType.Administrative)]
    public class AquacultureFacilitiesAdministrationController : BaseAuditController
    {
        private static readonly PageCodeEnum[] PAGE_CODES = new PageCodeEnum[]
        {
            PageCodeEnum.AquaFarmReg,
            PageCodeEnum.AquaFarmChange,
            PageCodeEnum.AquaFarmDereg
        };

        private readonly IAquacultureFacilitiesService service;
        private readonly IAquacultureFacilitiesNomenclaturesService nomenclaturesService;
        private readonly IApplicationService applicationService;
        private readonly IApplicationsRegisterService applicationsRegisterService;
        private readonly IDeliveryService deliveryService;
        private readonly IFileService fileService;
        private readonly IUserService userService;
        private readonly ILogBooksService logBooksService;

        public AquacultureFacilitiesAdministrationController(IAquacultureFacilitiesService service,
                                                             IAquacultureFacilitiesNomenclaturesService nomenclaturesService,
                                                             IApplicationService applicationService,
                                                             IApplicationsRegisterService applicationsRegisterService,
                                                             IDeliveryService deliveryService,
                                                             IFileService fileService,
                                                             IPermissionsService permissionsService,
                                                             IUserService userService,
                                                             ILogBooksService logBooksService)
            : base(permissionsService)
        {
            this.service = service;
            this.applicationService = applicationService;
            this.nomenclaturesService = nomenclaturesService;
            this.applicationsRegisterService = applicationsRegisterService;
            this.deliveryService = deliveryService;
            this.fileService = fileService;
            this.userService = userService;
            this.logBooksService = logBooksService;
        }

        // Register
        [HttpPost]
        [CustomAuthorize(Permissions.AquacultureFacilitiesReadAll, Permissions.AquacultureFacilitiesRead)]
        public IActionResult GetAllAquacultures([FromBody] GridRequestModel<AquacultureFacilitiesFilters> request)
        {
            if (!CurrentUser.Permissions.Contains(Permissions.AquacultureFacilitiesReadAll))
            {
                int? territoryUnitId = userService.GetUserTerritoryUnitId(CurrentUser.ID);

                if (territoryUnitId.HasValue)
                {
                    if (request.Filters == null)
                    {
                        request.Filters = new AquacultureFacilitiesFilters
                        {
                            ShowInactiveRecords = false
                        };
                    }

                    request.Filters.TerritoryUnitId = territoryUnitId.Value;
                }
                else
                {
                    return Ok(Enumerable.Empty<AquacultureFacilityDTO>());
                }
            }

            IQueryable<AquacultureFacilityDTO> result = service.GetAllAquacultures(request.Filters);
            return PageResult(result, request, false);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesReadAll, Permissions.AquacultureFacilitiesRead)]
        public IActionResult GetAquaculture([FromQuery] int id)
        {
            AquacultureFacilityEditDTO aquaculture = service.GetAquaculture(id);
            return Ok(aquaculture);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.AquacultureFacilitiesAddRecords)]
        [RequestFormLimits(ValueCountLimit = 16384)]
        public IActionResult AddAquaculture([FromForm] AquacultureFacilityEditDTO aquaculture, [FromQuery] bool ignoreLogBookConflicts)
        {
            try
            {
                int id = service.AddAquaculture(aquaculture, ignoreLogBookConflicts);
                return Ok(id);
            }
            catch (InvalidLogBookPagesRangeException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, ErrorCode.InvalidLogBookPagesRange);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.AquacultureFacilitiesEditRecords)]
        [RequestFormLimits(ValueCountLimit = 16384)]
        public IActionResult EditAquaculture([FromForm] AquacultureFacilityEditDTO aquaculture, [FromQuery] bool ignoreLogBookConflicts)
        {
            try
            {
                service.EditAquaculture(aquaculture, ignoreLogBookConflicts);
                return Ok();
            }
            catch (InvalidLogBookPagesRangeException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, ErrorCode.InvalidLogBookPagesRange);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.AquacultureFacilitiesEditRecords)]
        [RequestFormLimits(ValueCountLimit = 16384)]
        public async Task<IActionResult> EditAndDownloadAquaculture([FromForm] AquacultureFacilityEditDTO aquaculture, [FromQuery] bool ignoreLogBookConflicts)
        {
            try
            {
                service.EditAquaculture(aquaculture, ignoreLogBookConflicts);

                byte[] file = await service.DownloadAquacultureFacility(aquaculture.Id!.Value);
                return File(file, "application/pdf");
            }
            catch (InvalidLogBookPagesRangeException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, errorCode: ErrorCode.InvalidLogBookPagesRange);
            }
        }

        [HttpPut]
        [CustomAuthorize(Permissions.AquacultureFacilitiesEditRecords)]
        public IActionResult UpdateAquacultureStatus([FromQuery] int aquacultureId, [FromQuery] int? applicationId, [FromBody] CancellationHistoryEntryDTO status)
        {
            service.UpdateAquacultureStatus(aquacultureId, status, applicationId);
            return Ok();
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.AquacultureFacilitiesDeleteRecords)]
        public IActionResult DeleteAquaculture([FromQuery] int id)
        {
            service.DeleteAquaculture(id);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.AquacultureFacilitiesRestoreRecords)]
        public IActionResult UndoDeleteAquaculture([FromQuery] int id)
        {
            service.UndoDeleteAquaculture(id);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesReadAll, Permissions.AquacultureFacilitiesRead)]
        public async Task<IActionResult> DownloadAquacultureFacility([FromQuery] int aquacultureId)
        {
            byte[] file = await service.DownloadAquacultureFacility(aquacultureId);
            return File(file, "application/pdf");
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesReadAll,
                         Permissions.AquacultureFacilitiesRead,
                         Permissions.AquacultureFacilitiesApplicationsReadAll,
                         Permissions.AquacultureFacilitiesApplicationsRead)]
        public IActionResult DownloadFile([FromQuery] int id)
        {
            DownloadableFileDTO file = fileService.GetFileForDownload(id);
            return File(file.Bytes, file.MimeType, file.FileName);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ShipsRegisterReadAll, Permissions.ShipsRegisterRead)]
        public IActionResult DownloadAquacultureFacilitiesExcel([FromBody] ExcelExporterRequestModel<AquacultureFacilitiesFilters> request)
        {
            Stream stream = service.DownloadAquacultureFacilitiesExcel(request);
            return ExcelFile(stream, request.Filename);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesReadAll,
                         Permissions.AquacultureFacilitiesRead,
                         Permissions.AquacultureFacilitiesApplicationsReadAll,
                         Permissions.AquacultureFacilitiesApplicationsRead)]
        public IActionResult GetAquacultureInstallationSimpleAudit([FromQuery] int id)
        {
            SimpleAuditDTO audit = service.GetAquacultureInstallationSimpleAudit(id);
            return Ok(audit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesReadAll,
                         Permissions.AquacultureFacilitiesRead,
                         Permissions.AquacultureFacilitiesApplicationsReadAll,
                         Permissions.AquacultureFacilitiesApplicationsRead)]
        public IActionResult GetAquacultureInstallationNetCageAudit([FromQuery] int id)
        {
            SimpleAuditDTO audit = service.GetAquacultureInstallationNetCageAudit(id);
            return Ok(audit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesReadAll, Permissions.AquacultureFacilitiesRead)]
        public IActionResult GetAquacultureUsageDocumentSimpleAudit([FromQuery] int id)
        {
            SimpleAuditDTO audit = service.GetAquacultureUsageDocumentSimpleAudit(id);
            return Ok(audit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesReadAll, Permissions.AquacultureFacilitiesRead)]
        public IActionResult GetAquacultureWaterLawCertificateSimpleAudit([FromQuery] int id)
        {
            SimpleAuditDTO audit = service.GetAquacultureWaterLawCertificateSimpleAudit(id);
            return Ok(audit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesReadAll, Permissions.AquacultureFacilitiesRead)]
        public IActionResult GetAquacultureOvosCertificateSimpleAudit([FromQuery] int id)
        {
            SimpleAuditDTO audit = service.GetAquacultureOvosCertificateSimpleAudit(id);
            return Ok(audit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesReadAll, Permissions.AquacultureFacilitiesRead)]
        public IActionResult GetAquacultureBabhCertificateSimpleAudit([FromQuery] int id)
        {
            SimpleAuditDTO audit = service.GetAquacultureBabhCertificateSimpleAudit(id);
            return Ok(audit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesReadAll, Permissions.AquacultureFacilitiesRead)]
        public IActionResult GetAquacultureLogBookSimpleAudit([FromQuery] int id)
        {
            SimpleAuditDTO audit = logBooksService.GetSimpleAudit(id);
            return Ok(audit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesReadAll,
                         Permissions.AquacultureFacilitiesRead,
                         Permissions.AquacultureFacilitiesApplicationsReadAll,
                         Permissions.AquacultureFacilitiesApplicationsRead)]
        public override IActionResult GetAuditInfo([FromQuery] int id)
        {
            SimpleAuditDTO audit = service.GetSimpleAudit(id);
            return Ok(audit);
        }

        // Applications
        [HttpPost]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsReadAll, Permissions.AquacultureFacilitiesApplicationsRead)]
        public IActionResult GetAllApplications([FromBody] GridRequestModel<ApplicationsRegisterFilters> request)
        {
            if (!CurrentUser.Permissions.Contains(Permissions.AquacultureFacilitiesApplicationsReadAll))
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
                    return Ok(Enumerable.Empty<AquacultureFacilityDTO>());
                }
            }

            IQueryable<ApplicationRegisterDTO> permits = applicationsRegisterService.GetAllApplications(request.Filters, null, PAGE_CODES);
            return PageResult(permits, request, false);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsReadAll, Permissions.AquacultureFacilitiesApplicationsRead)]
        public IActionResult GetAquacultureApplication([FromQuery] int id)
        {
            AquacultureApplicationEditDTO aquaculture = service.GetAquacultureApplication(id);
            return Ok(aquaculture);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsInspectAndCorrectRegiXData)]
        public IActionResult GetAquacultureRegixData([FromQuery] int applicationId)
        {
            RegixChecksWrapperDTO<AquacultureRegixDataDTO> result = service.GetAquacultureRegixData(applicationId);
            return Ok(result);
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsDeleteRecords)]
        public IActionResult DeleteApplication([FromQuery] int id)
        {
            applicationsRegisterService.DeleteApplication(id);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsRestoreRecords)]
        public IActionResult UndoDeleteApplication([FromQuery] int id)
        {
            applicationsRegisterService.UndoDeleteApplication(id);
            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsEditRecords)]
        public IActionResult AssignApplicationViaAccessCode([FromQuery] string accessCode)
        {
            try
            {
                AssignedApplicationInfoDTO applicationData = applicationService.AssignApplicationViaAccessCode(accessCode, CurrentUser.ID, PAGE_CODES);
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
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsReadAll, Permissions.AquacultureFacilitiesApplicationsRead)]
        public IActionResult GetApplicationStatuses()
        {
            List<NomenclatureDTO> statuses = applicationsRegisterService.GetApplicationStatuses(ApplicationHierarchyTypesEnum.Online, ApplicationHierarchyTypesEnum.OnPaper);
            return Ok(statuses);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsReadAll, Permissions.AquacultureFacilitiesApplicationsRead)]
        public IActionResult GetApplicationTypes()
        {
            List<NomenclatureDTO> types = applicationsRegisterService.GetApplicationTypes(PAGE_CODES);
            return Ok(types);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsReadAll, Permissions.AquacultureFacilitiesApplicationsRead)]
        public IActionResult GetApplicationSources()
        {
            List<NomenclatureDTO> sources = applicationsRegisterService.GetApplicationSources(ApplicationHierarchyTypesEnum.Online, ApplicationHierarchyTypesEnum.OnPaper);
            return Ok(sources);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesReadAll, Permissions.AquacultureFacilitiesRead)]
        public IActionResult GetApplicationDataForRegister([FromQuery] int applicationId)
        {
            AquacultureFacilityEditDTO aquaculture = service.GetApplicationDataForRegister(applicationId);
            return Ok(aquaculture);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesReadAll, Permissions.AquacultureFacilitiesRead)]
        public IActionResult GetRegisterByApplicationId([FromQuery] int applicationId)
        {
            AquacultureFacilityEditDTO aquaculture = service.GetRegisterByApplicationId(applicationId);
            return Ok(aquaculture);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesReadAll, Permissions.AquacultureFacilitiesRead)]
        public IActionResult GetRegisterByChangeOfCircumstancesApplicationId([FromQuery] int applicationId)
        {
            AquacultureFacilityEditDTO aquaculture = service.GetRegisterByChangeOfCircumstancesApplicationId(applicationId);
            return Ok(aquaculture);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsAddRecords)]
        [RequestFormLimits(ValueCountLimit = 16384)]
        public async Task<IActionResult> AddAquacultureApplication([FromForm] AquacultureApplicationEditDTO aquaculture)
        {
            IActionResult result = await CheckModel(aquaculture);

            if (result != null)
            {
                return result;
            }

            int id = service.AddAquacultureApplication(aquaculture, ApplicationStatusesEnum.EXT_CHK_STARTED);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsEditRecords)]
        [RequestFormLimits(ValueCountLimit = 16384)]
        public async Task<IActionResult> EditAquacultureApplication([FromQuery] bool saveAsDraft, [FromForm] AquacultureApplicationEditDTO aquaculture)
        {
            IActionResult result = await CheckModel(aquaculture);

            if (result != null)
            {
                return result;
            }

            if (saveAsDraft)
            {
                service.EditAquacultureApplication(aquaculture);
            }
            else
            {
                service.EditAquacultureApplication(aquaculture, ApplicationStatusesEnum.EXT_CHK_STARTED);
            }

            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsInspectAndCorrectRegiXData)]
        public IActionResult EditAquacultureApplicationAndStartRegixChecks([FromForm] AquacultureRegixDataDTO aquaculture)
        {
            service.EditAquacultureApplicationRegixData(aquaculture);
            return Ok();
        }

        // Change of circumstances
        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsReadAll, Permissions.AquacultureFacilitiesApplicationsRead)]
        public IActionResult GetAquacultureChangeOfCircumstancesApplication([FromQuery] int id)
        {
            AquacultureChangeOfCircumstancesApplicationDTO result = service.GetAquacultureChangeOfCircumstancesApplication(id);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsInspectAndCorrectRegiXData)]
        public IActionResult GetAquacultureChangeOfCircumstancesRegixData([FromQuery] int applicationId)
        {
            RegixChecksWrapperDTO<AquacultureChangeOfCircumstancesRegixDataDTO> data = service.GetAquacultureChangeOfCircumstancesRegixData(applicationId);
            return Ok(data);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsAddRecords)]
        public async Task<IActionResult> AddAquacultureChangeOfCircumstancesApplication([FromForm] AquacultureChangeOfCircumstancesApplicationDTO application)
        {
            IActionResult result = await CheckModel(application);

            if (result != null)
            {
                return result;
            }

            int id = service.AddAquacultureChangeOfCircumstancesApplication(application, ApplicationStatusesEnum.EXT_CHK_STARTED);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsEditRecords)]
        public async Task<IActionResult> EditAquacultureChangeOfCircumstancesApplication([FromQuery] bool saveAsDraft,
                                                                                         [FromForm] AquacultureChangeOfCircumstancesApplicationDTO application)
        {
            IActionResult result = await CheckModel(application);

            if (result != null)
            {
                return result;
            }

            if (saveAsDraft)
            {
                service.EditAquacultureChangeOfCircumstancesApplication(application);
            }
            else
            {
                service.EditAquacultureChangeOfCircumstancesApplication(application, ApplicationStatusesEnum.EXT_CHK_STARTED);
            }

            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsInspectAndCorrectRegiXData)]
        public IActionResult EditAquacultureChangeOfCircumstancesApplicationAndStartRegixChecks([FromForm] AquacultureChangeOfCircumstancesRegixDataDTO application)
        {
            service.EditAquacultureChangeOfCircumstancesRegixData(application);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsReadAll, Permissions.AquacultureFacilitiesApplicationsRead)]
        public IActionResult GetAquacultureFromChangeOfCircumstancesApplication([FromQuery] int applicationId)
        {
            AquacultureFacilityEditDTO result = service.GetAquacultureFromChangeOfCircumstancesApplication(applicationId);
            return Ok(result);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsEditRecords)]
        [RequestFormLimits(ValueCountLimit = 16384)]
        public IActionResult CompleteChangeOfCircumstancesApplication([FromForm] AquacultureFacilityEditDTO aquaculture, [FromQuery] bool ignoreLogBookConflicts)
        {
            service.CompleteChangeOfCircumstancesApplication(aquaculture, ignoreLogBookConflicts);
            return Ok();
        }

        // Deregistration
        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsReadAll, Permissions.AquacultureFacilitiesApplicationsRead)]
        public IActionResult GetAquacultureDeregistrationApplication([FromQuery] int id)
        {
            AquacultureDeregistrationApplicationDTO result = service.GetAquacultureDeregistrationApplication(id);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsInspectAndCorrectRegiXData)]
        public IActionResult GetAquacultureDeregistrationRegixData([FromQuery] int applicationId)
        {
            RegixChecksWrapperDTO<AquacultureDeregistrationRegixDataDTO> data = service.GetAquacultureDeregistrationRegixData(applicationId);
            return Ok(data);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsAddRecords)]
        public async Task<IActionResult> AddAquacultureDeregistrationApplication([FromForm] AquacultureDeregistrationApplicationDTO application)
        {
            IActionResult result = await CheckModel(application);

            if (result != null)
            {
                return result;
            }

            int id = service.AddAquacultureDeregistrationApplication(application, ApplicationStatusesEnum.EXT_CHK_STARTED);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsEditRecords)]
        public async Task<IActionResult> EditAquacultureDeregistrationApplication([FromQuery] bool saveAsDraft,
                                                                                  [FromForm] AquacultureDeregistrationApplicationDTO application)
        {
            IActionResult result = await CheckModel(application);

            if (result != null)
            {
                return result;
            }

            if (saveAsDraft)
            {
                service.EditAquacultureDeregistrationApplication(application);
            }
            else
            {
                service.EditAquacultureDeregistrationApplication(application, ApplicationStatusesEnum.EXT_CHK_STARTED);
            }

            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsEditRecords)]
        public IActionResult EditAquacultureDeregistrationApplicationAndStartRegixChecks([FromForm] AquacultureDeregistrationRegixDataDTO application)
        {
            service.EditAquacultureDeregistrationRegixData(application);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsReadAll, Permissions.AquacultureFacilitiesApplicationsRead)]
        public IActionResult GetAquacultureFromDeregistrationApplication([FromQuery] int applicationId)
        {
            AquacultureFacilityEditDTO result = service.GetAquacultureFromDeregistrationApplication(applicationId);
            return Ok(result);
        }

        // Nomenclatures
        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsReadAll,
                         Permissions.AquacultureFacilitiesApplicationsRead,
                         Permissions.AquacultureFacilitiesReadAll,
                         Permissions.AquacultureFacilitiesRead)]
        public IActionResult GetAllAquacultureNomenclatures()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetAllAquacultureNomenclatures();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsReadAll,
                         Permissions.AquacultureFacilitiesApplicationsRead,
                         Permissions.AquacultureFacilitiesReadAll,
                         Permissions.AquacultureFacilitiesRead)]
        public IActionResult GetAquaculturePowerSupplyTypes()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetAquaculturePowerSupplyTypes();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsReadAll,
                         Permissions.AquacultureFacilitiesApplicationsRead,
                         Permissions.AquacultureFacilitiesReadAll,
                         Permissions.AquacultureFacilitiesRead)]
        public IActionResult GetAquacultureWaterAreaTypes()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetAquacultureWaterAreaTypes();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsReadAll,
                         Permissions.AquacultureFacilitiesApplicationsRead,
                         Permissions.AquacultureFacilitiesReadAll,
                         Permissions.AquacultureFacilitiesRead)]
        public IActionResult GetWaterLawCertificateTypes()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetWaterLawCertificateTypes();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsReadAll,
                         Permissions.AquacultureFacilitiesApplicationsRead,
                         Permissions.AquacultureFacilitiesReadAll,
                         Permissions.AquacultureFacilitiesRead)]
        public IActionResult GetAquacultureInstallationTypes()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetAquacultureInstallationTypes();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsReadAll,
                         Permissions.AquacultureFacilitiesApplicationsRead,
                         Permissions.AquacultureFacilitiesReadAll,
                         Permissions.AquacultureFacilitiesRead)]
        public IActionResult GetInstallationBasinPurposeTypes()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetInstallationBasinPurposeTypes();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsReadAll,
                         Permissions.AquacultureFacilitiesApplicationsRead,
                         Permissions.AquacultureFacilitiesReadAll,
                         Permissions.AquacultureFacilitiesRead)]
        public IActionResult GetInstallationBasinMaterialTypes()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetInstallationBasinMaterialTypes();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsReadAll,
                         Permissions.AquacultureFacilitiesApplicationsRead,
                         Permissions.AquacultureFacilitiesReadAll,
                         Permissions.AquacultureFacilitiesRead)]
        public IActionResult GetHatcheryEquipmentTypes()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetHatcheryEquipmentTypes();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsReadAll,
                         Permissions.AquacultureFacilitiesApplicationsRead,
                         Permissions.AquacultureFacilitiesReadAll,
                         Permissions.AquacultureFacilitiesRead)]
        public IActionResult GetInstallationNetCageTypes()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetInstallationNetCageTypes();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsReadAll,
                         Permissions.AquacultureFacilitiesApplicationsRead,
                         Permissions.AquacultureFacilitiesReadAll,
                         Permissions.AquacultureFacilitiesRead)]
        public IActionResult GetInstallationCollectorTypes()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetInstallationCollectorTypes();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsReadAll,
                         Permissions.AquacultureFacilitiesApplicationsRead,
                         Permissions.AquacultureFacilitiesReadAll,
                         Permissions.AquacultureFacilitiesRead)]
        public IActionResult GetAquacultureStatusTypes()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetAquacultureStatusTypes();
            return Ok(result);
        }

        private async Task<IActionResult> CheckModel(IDeliverableApplication application)
        {
            bool hasDelivery = deliveryService.HasApplicationDelivery(application.ApplicationId!.Value);

            if (hasDelivery)
            {
                if (application.DeliveryData == null)
                {
                    return BadRequest("No delivery data provided");
                }
                else
                {
                    bool hasEDelivery = await deliveryService.HasSubmittedForEDelivery(application.DeliveryData.DeliveryTypeId,
                                                                                       application.SubmittedFor,
                                                                                       application.SubmittedBy);

                    if (hasEDelivery == false)
                    {
                        return ValidationFailedResult(new List<string> { nameof(ApplicationValidationErrorsEnum.NoEDeliveryRegistration) });
                    }
                }
            }

            return null;
        }
    }
}
