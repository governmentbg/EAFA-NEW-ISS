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
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.FishingCapacity;
using IARA.DomainModels.DTOModels.FishingCapacity.Duplicates;
using IARA.DomainModels.DTOModels.FishingCapacity.IncreaseCapacity;
using IARA.DomainModels.DTOModels.FishingCapacity.ReduceCapacity;
using IARA.DomainModels.DTOModels.FishingCapacity.TransferCapacity;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Excel.Tools.Models;
using IARA.Interfaces;
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
    public class FishingCapacityAdministrationController : BaseAuditController
    {
        private static readonly PageCodeEnum[] PAGE_CODES = new PageCodeEnum[]
        {
            PageCodeEnum.IncreaseFishCap,
            PageCodeEnum.ReduceFishCap,
            PageCodeEnum.TransferFishCap,
            PageCodeEnum.CapacityCertDup
        };

        private readonly IFishingCapacityService service;
        private readonly IFileService fileService;
        private readonly IApplicationService applicationService;
        private readonly IApplicationsRegisterService applicationsRegisterService;
        private readonly IUserService userService;
        private readonly IDeliveryService deliveryService;

        public FishingCapacityAdministrationController(IFishingCapacityService service,
                                                       IFileService fileService,
                                                       IApplicationService applicationService,
                                                       IApplicationsRegisterService applicationsRegisterService,
                                                       IPermissionsService permissionsService,
                                                       IUserService userService,
                                                       IDeliveryService deliveryService)
            : base(permissionsService)
        {
            this.service = service;
            this.fileService = fileService;
            this.applicationService = applicationService;
            this.applicationsRegisterService = applicationsRegisterService;
            this.userService = userService;
            this.deliveryService = deliveryService;
        }

        // Register
        [HttpPost]
        [CustomAuthorize(Permissions.FishingCapacityReadAll, Permissions.FishingCapacityRead)]
        public IActionResult GetAllShipCapacities([FromBody] GridRequestModel<FishingCapacityFilters> request)
        {
            if (!CurrentUser.Permissions.Contains(Permissions.FishingCapacityReadAll))
            {
                int? territoryUnitId = userService.GetUserTerritoryUnitId(CurrentUser.ID);

                if (territoryUnitId.HasValue)
                {
                    if (request.Filters == null)
                    {
                        request.Filters = new FishingCapacityFilters
                        {
                            ShowInactiveRecords = false
                        };
                    }

                    request.Filters.TerritoryUnitId = territoryUnitId.Value;
                }
                else
                {
                    return Ok(Enumerable.Empty<ShipFishingCapacityDTO>());
                }
            }

            IQueryable<ShipFishingCapacityDTO> caps = service.GetAllShipCapacities(request.Filters);
            GridResultModel<ShipFishingCapacityDTO> result = new(caps, request, false);
            service.SetShipCapacityHistories(result.Records);

            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FishingCapacityReadAll, Permissions.FishingCapacityRead)]
        public override IActionResult GetAuditInfo(int id)
        {
            SimpleAuditDTO audit = service.GetSimpleAudit(id);
            return Ok(audit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FishingCapacityReadAll, Permissions.FishingCapacityRead, Permissions.FishingCapacityApplicationsRead)]
        public IActionResult GetFishingCapacityHolderSimpleAudit(int id)
        {
            SimpleAuditDTO audit = service.GetFishingCapacityHolderSimpleAudit(id);
            return Ok(audit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FishingCapacityReadAll, Permissions.FishingCapacityRead, Permissions.FishingCapacityApplicationsRead)]
        public IActionResult DownloadFile(int id)
        {
            DownloadableFileDTO file = fileService.GetFileForDownload(id);
            return File(file.Bytes, file.MimeType, file.FileName);
        }

        // Capacity certificates
        [HttpPost]
        [CustomAuthorize(Permissions.FishingCapacityCertificatesRead)]
        public IActionResult GetAllCapacityCertificates([FromBody] GridRequestModel<FishingCapacityCertificatesFilters> request)
        {
            IQueryable<FishingCapacityCertificateDTO> capacities = service.GetAllCapacityCertificates(request.Filters);
            GridResultModel< FishingCapacityCertificateDTO> result = new(capacities, request, false);
            service.SetCapacityCertificateHistory(result.Records);

            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FishingCapacityCertificatesRead)]
        public IActionResult GetCapacityCertificate([FromQuery] int id)
        {
            FishingCapacityCertificateEditDTO capacity = service.GetCapacityCertificate(id);
            return Ok(capacity);
        }

        [HttpPut]
        [CustomAuthorize(Permissions.FishingCapacityCertificatesEditRecords)]
        public IActionResult EditCapacityCertificate([FromBody] FishingCapacityCertificateEditDTO capacity)
        {
            service.EditCapacityCertificate(capacity);
            return Ok();
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.FishingCapacityCertificatesDeleteRecords)]
        public IActionResult DeleteCapacityCertificate([FromQuery] int id)
        {
            service.DeleteCapacityCertificate(id);
            return Ok();
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.FishingCapacityCertificatesRestoreRecords)]
        public IActionResult UndoDeleteCapacityCertificate([FromQuery] int id)
        {
            service.UndoDeleteCapacityCertificate(id);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FishingCapacityCertificatesRead)]
        public IActionResult GetFishingCapacityCertificateSimpleAudit([FromQuery] int id)
        {
            SimpleAuditDTO audit = service.GetFishingCapacityCertificateSimpleAudit(id);
            return Ok(audit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FishingCapacityCertificatesRead)]
        public async Task<IActionResult> DownloadFishingCapacityCertificate([FromQuery] int certificateId)
        {
            byte[] file = await service.DownloadFishingCapacityCertificate(certificateId);
            return File(file, "application/pdf");
        }

        [HttpPost]
        [CustomAuthorize(Permissions.FishingCapacityCertificatesRead)]
        public IActionResult DownloadFishingCapacityCertificateExcel([FromBody] ExcelExporterRequestModel<FishingCapacityCertificatesFilters> request)
        {
            Stream stream = service.DownloadFishingCapacityCertificateExcel(request);
            return ExcelFile(stream, request.Filename);
        }

        // Nomenclatures

        [HttpGet]
        [CustomAuthorize(Permissions.FishingCapacityReadAll,
                         Permissions.FishingCapacityRead,
                         Permissions.FishingCapacityApplicationsReadAll,
                         Permissions.FishingCapacityApplicationsRead,
                         Permissions.ShipsRegisterApplicationReadAll,
                         Permissions.ShipsRegisterApplicationsRead)]
        public IActionResult GetAllCapacityCertificateNomenclatures()
        {
            List<FishingCapacityCertificateNomenclatureDTO> licences = service.GetAllCapacityCertificateNomenclatures();
            return Ok(licences);
        }

        // Applications
        [HttpPost]
        [CustomAuthorize(Permissions.FishingCapacityApplicationsReadAll, Permissions.FishingCapacityApplicationsRead)]
        public IActionResult GetAllApplications([FromBody] GridRequestModel<ApplicationsRegisterFilters> request)
        {
            if (!CurrentUser.Permissions.Contains(Permissions.FishingCapacityApplicationsReadAll))
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

            IQueryable<ApplicationRegisterDTO> permits = applicationsRegisterService.GetAllApplications(request.Filters, null, PAGE_CODES);
            return PageResult(permits, request, false);
        }

        // Increase capacity application
        [HttpGet]
        [CustomAuthorize(Permissions.FishingCapacityApplicationsReadAll, Permissions.FishingCapacityApplicationsRead)]
        public IActionResult GetIncreaseFishingCapacityApplication([FromQuery] int id)
        {
            IncreaseFishingCapacityApplicationDTO result = service.GetIncreaseFishingCapacityApplication(id);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FishingCapacityApplicationsInspectAndCorrectRegiXData)]
        public IActionResult GetIncreaseFishingCapacityRegixData([FromQuery] int applicationId)
        {
            RegixChecksWrapperDTO<IncreaseFishingCapacityRegixDataDTO> data = service.GetIncreaseFishingCapacityRegixData(applicationId);
            return Ok(data);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FishingCapacityApplicationsReadAll, Permissions.FishingCapacityApplicationsRead)]
        public IActionResult GetCapacityDataFromIncreaseCapacityApplication([FromQuery] int applicationId)
        {
            IncreaseFishingCapacityDataDTO data = service.GetCapacityDataFromIncreaseCapacityApplication(applicationId);
            return Ok(data);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.FishingCapacityApplicationsAddRecords)]
        public async Task<IActionResult> AddIncreaseFishingCapacityApplication([FromForm] IncreaseFishingCapacityApplicationDTO application)
        {
            IActionResult result = await CheckModel(application);

            if (result != null)
            {
                return result;
            }

            int id = service.AddIncreaseFishingCapacityApplication(application, ApplicationStatusesEnum.EXT_CHK_STARTED);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.FishingCapacityApplicationsEditRecords)]
        public async Task<IActionResult> EditIncreaseFishingCapacityApplication([FromQuery] bool saveAsDraft,
                                                                                [FromForm] IncreaseFishingCapacityApplicationDTO application)
        {
            IActionResult result = await CheckModel(application);

            if (result != null)
            {
                return result;
            }

            if (saveAsDraft)
            {
                service.EditIncreaseFishingCapacityApplication(application);
            }
            else
            {
                service.EditIncreaseFishingCapacityApplication(application, ApplicationStatusesEnum.EXT_CHK_STARTED);
            }

            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.FishingCapacityApplicationsEditRecords)]
        public IActionResult EditIncreaseFishingCapacityApplicationAndStartRegixChecks([FromForm] IncreaseFishingCapacityRegixDataDTO application)
        {
            service.EditIncreaseFishingCapacityRegixData(application);
            return Ok();
        }

        // Reduce fishing capacity application
        [HttpGet]
        [CustomAuthorize(Permissions.FishingCapacityApplicationsReadAll, Permissions.FishingCapacityApplicationsRead)]
        public IActionResult GetReduceFishingCapacityApplication([FromQuery] int id)
        {
            ReduceFishingCapacityApplicationDTO result = service.GetReduceFishingCapacityApplication(id);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FishingCapacityApplicationsInspectAndCorrectRegiXData)]
        public IActionResult GetReduceFishingCapacityRegixData([FromQuery] int applicationId)
        {
            RegixChecksWrapperDTO<ReduceFishingCapacityRegixDataDTO> data = service.GetReduceFishingCapacityRegixData(applicationId);
            return Ok(data);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FishingCapacityApplicationsReadAll, Permissions.FishingCapacityApplicationsRead)]
        public IActionResult GetCapacityDataFromReduceCapacityApplication([FromQuery] int applicationId)
        {
            ReduceFishingCapacityDataDTO data = service.GetCapacityDataFromReduceCapacityApplication(applicationId);
            return Ok(data);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.FishingCapacityApplicationsAddRecords)]
        public async Task<IActionResult> AddReduceFishingCapacityApplication([FromForm] ReduceFishingCapacityApplicationDTO application)
        {
            IActionResult result = await CheckModel(application);

            if (result != null)
            {
                return result;
            }

            int id = service.AddReduceFishingCapacityApplication(application, ApplicationStatusesEnum.EXT_CHK_STARTED);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.FishingCapacityApplicationsEditRecords)]
        public async Task<IActionResult> EditReduceFishingCapacityApplication([FromQuery] bool saveAsDraft,
                                                                              [FromForm] ReduceFishingCapacityApplicationDTO application)
        {
            IActionResult result = await CheckModel(application);

            if (result != null)
            {
                return result;
            }

            if (saveAsDraft)
            {
                service.EditReduceFishingCapacityApplication(application);
            }
            else
            {
                service.EditReduceFishingCapacityApplication(application, ApplicationStatusesEnum.EXT_CHK_STARTED);
            }

            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.FishingCapacityApplicationsEditRecords)]
        public IActionResult EditReduceFishingCapacityApplicationAndStartRegixChecks([FromForm] ReduceFishingCapacityRegixDataDTO application)
        {
            service.EditReduceFishingCapacityRegixData(application);
            return Ok();
        }

        // Transfer fishing capacity application
        [HttpGet]
        [CustomAuthorize(Permissions.FishingCapacityApplicationsReadAll, Permissions.FishingCapacityApplicationsRead)]
        public IActionResult GetTransferFishingCapacityApplication([FromQuery] int id)
        {
            TransferFishingCapacityApplicationDTO result = service.GetTransferFishingCapacityApplication(id);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FishingCapacityApplicationsInspectAndCorrectRegiXData)]
        public IActionResult GetTransferFishingCapacityRegixData([FromQuery] int applicationId)
        {
            RegixChecksWrapperDTO<TransferFishingCapacityRegixDataDTO> data = service.GetTransferFishingCapacityRegixData(applicationId);
            return Ok(data);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.FishingCapacityApplicationsAddRecords)]
        public async Task<IActionResult> AddTransferFishingCapacityApplication([FromForm] TransferFishingCapacityApplicationDTO application)
        {
            IActionResult result = await CheckModel(application);

            if (result != null)
            {
                return result;
            }

            int id = service.AddTransferFishingCapacityApplication(application, ApplicationStatusesEnum.EXT_CHK_STARTED);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.FishingCapacityApplicationsEditRecords)]
        public async Task<IActionResult> EditTransferFishingCapacityApplication([FromQuery] bool saveAsDraft,
                                                                                [FromForm] TransferFishingCapacityApplicationDTO application)
        {
            IActionResult result = await CheckModel(application);

            if (result != null)
            {
                return result;
            }

            if (saveAsDraft)
            {
                service.EditTransferFishingCapacityApplication(application);
            }
            else
            {
                service.EditTransferFishingCapacityApplication(application, ApplicationStatusesEnum.EXT_CHK_STARTED);
            }

            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.FishingCapacityApplicationsEditRecords)]
        public IActionResult EditTransferFishingCapacityApplicationAndStartRegixChecks([FromForm] TransferFishingCapacityRegixDataDTO application)
        {
            service.EditTransferFishingCapacityRegixData(application);
            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.FishingCapacityApplicationsAddRecords)]
        public IActionResult CompleteTransferFishingCapacityApplication([FromForm] TransferFishingCapacityApplicationDTO application)
        {
            service.CompleteTransferFishingCapacityApplication(application);
            return Ok();
        }

        // Capacity certificate duplicate application
        [HttpGet]
        [CustomAuthorize(Permissions.FishingCapacityApplicationsReadAll, Permissions.FishingCapacityApplicationsRead)]
        public IActionResult GetCapacityCertificateDuplicateApplication([FromQuery] int id)
        {
            CapacityCertificateDuplicateApplicationDTO result = service.GetCapacityCertificateDuplicateApplication(id);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FishingCapacityApplicationsInspectAndCorrectRegiXData)]
        public IActionResult GetCapacityCertificateDuplicateRegixData([FromQuery] int applicationId)
        {
            RegixChecksWrapperDTO<CapacityCertificateDuplicateRegixDataDTO> data = service.GetCapacityCertificateDuplicateRegixData(applicationId);
            return Ok(data);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.FishingCapacityApplicationsAddRecords)]
        public async Task<IActionResult> AddCapacityCertificateDuplicateApplication([FromForm] CapacityCertificateDuplicateApplicationDTO application)
        {
            IActionResult result = await CheckModel(application);

            if (result != null)
            {
                return result;
            }

            int id = service.AddCapacityCertificateDuplicateApplication(application, ApplicationStatusesEnum.EXT_CHK_STARTED);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.FishingCapacityApplicationsEditRecords)]
        public async Task<IActionResult> EditCapacityCertificateDuplicateApplication([FromQuery] bool saveAsDraft,
                                                                                     [FromForm] CapacityCertificateDuplicateApplicationDTO application)
        {
            IActionResult result = await CheckModel(application);

            if (result != null)
            {
                return result;
            }

            if (saveAsDraft)
            {
                service.EditCapacityCertificateDuplicateApplication(application);
            }
            else
            {
                service.EditCapacityCertificateDuplicateApplication(application, ApplicationStatusesEnum.EXT_CHK_STARTED);
            }

            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.FishingCapacityApplicationsEditRecords)]
        public IActionResult EditCapacityCertificateDuplicateApplicationAndStartRegixChecks([FromForm] CapacityCertificateDuplicateRegixDataDTO application)
        {
            service.EditCapacityCertificateDuplicateRegixData(application);
            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.FishingCapacityApplicationsAddRecords)]
        public IActionResult CompleteCapacityCertificateDuplicateApplication([FromForm] CapacityCertificateDuplicateApplicationDTO application)
        {
            service.CompleteCapacityCertificateDuplicateApplication(application);
            return Ok();
        }

        // Common application endpoints
        [HttpDelete]
        [CustomAuthorize(Permissions.FishingCapacityApplicationsDeleteRecords)]
        public IActionResult DeleteApplication([FromQuery] int id)
        {
            applicationsRegisterService.DeleteApplication(id);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.FishingCapacityApplicationsRestoreRecords)]
        public IActionResult UndoDeleteApplication([FromQuery] int id)
        {
            applicationsRegisterService.UndoDeleteApplication(id);
            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.FishingCapacityApplicationsEditRecords)]
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
        [CustomAuthorize(Permissions.FishingCapacityApplicationsReadAll, Permissions.FishingCapacityApplicationsRead)]
        public IActionResult GetApplicationStatuses()
        {
            List<NomenclatureDTO> statuses = applicationsRegisterService.GetApplicationStatuses(ApplicationHierarchyTypesEnum.Online, ApplicationHierarchyTypesEnum.OnPaper);
            return Ok(statuses);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FishingCapacityApplicationsReadAll, Permissions.FishingCapacityApplicationsRead)]
        public IActionResult GetApplicationTypes()
        {
            List<NomenclatureDTO> types = applicationsRegisterService.GetApplicationTypes(PAGE_CODES);
            return Ok(types);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FishingCapacityApplicationsReadAll, Permissions.FishingCapacityApplicationsRead)]
        public IActionResult GetApplicationSources()
        {
            List<NomenclatureDTO> sources = applicationsRegisterService.GetApplicationSources(ApplicationHierarchyTypesEnum.Online, ApplicationHierarchyTypesEnum.OnPaper);
            return Ok(sources);
        }

        // Maximum capacity
        [HttpPost]
        [CustomAuthorize(Permissions.MaximumCapacityRead)]
        public IActionResult GetAllMaximumCapacities([FromBody] GridRequestModel<MaximumFishingCapacityFilters> request)
        {
            IQueryable<MaximumFishingCapacityDTO> capacities = service.GetAllMaximumCapacities(request.Filters);
            return PageResult(capacities, request, false);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.MaximumCapacityRead)]
        public IActionResult GetMaximumCapacity([FromQuery] int id)
        {
            MaximumFishingCapacityEditDTO capacity = service.GetMaximumCapacity(id);
            return Ok(capacity);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.MaximumCapacityRead)]
        public IActionResult GetLatestMaximumCapacities()
        {
            LatestMaximumCapacityDTO result = service.GetLatestMaximumCapacities();
            return Ok(result);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.MaximumCapacityAddRecords)]
        public IActionResult AddMaximumCapacity([FromBody] MaximumFishingCapacityEditDTO capacity)
        {
            int id = service.AddMaximumCapacity(capacity);
            return Ok(id);
        }

        [HttpPut]
        [CustomAuthorize(Permissions.MaximumCapacityEditRecords)]
        public IActionResult EditMaximumCapacity([FromBody] MaximumFishingCapacityEditDTO capacity)
        {
            service.EditMaximumCapacity(capacity);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.MaximumCapacityRead)]
        public IActionResult GetMaximumCapacitySimpleAudit([FromQuery] int id)
        {
            SimpleAuditDTO audit = service.GetMaximumCapacitySimpleAudit(id);
            return Ok(audit);
        }

        // Analysis
        [HttpGet]
        [CustomAuthorize(Permissions.FishingCapacityAnalysis)]
        public IActionResult GetFishingCapacityStatistics([FromQuery] int year, [FromQuery] int month, [FromQuery] int day)
        {
            try
            {
                FishingCapacityStatisticsDTO result = service.GetFishingCapacityStatistics(year, month, day);
                return Ok(result);
            }
            catch (NoMaximumFishingCapacityToDateException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, errorCode: ErrorCode.NoMaximumFishingCapacityToDate);
            }
        }

        // Utils
        private async Task<IActionResult> CheckModel(IDeliverableApplication application)
        {
            bool hasDelivery = deliveryService.HasApplicationDelivery(application.ApplicationId!.Value);

            if (hasDelivery)
            {
                if (application.DeliveryData == null)
                {
                    return BadRequest("No delivery data provided for new free capacity certificate");
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

        private async Task<IActionResult> CheckModel(IncreaseFishingCapacityApplicationDTO application)
        {
            if (application.RemainingCapacityAction != null && application.RemainingCapacityAction.Action != FishingCapacityRemainderActionEnum.NoCertificate)
            {
                return await CheckModel(application as IDeliverableApplication);
            }
            return null;
        }

        private async Task<IActionResult> CheckModel(ReduceFishingCapacityApplicationDTO application)
        {
            if (application.FreedCapacityAction.Action != FishingCapacityRemainderActionEnum.NoCertificate)
            {
                return await CheckModel(application as IDeliverableApplication);
            }
            return null;
        }
    }
}
