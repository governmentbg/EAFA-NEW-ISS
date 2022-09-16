using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.Common.Exceptions;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.ApplicationsRegister;
using IARA.DomainModels.DTOModels.Buyers;
using IARA.DomainModels.DTOModels.Buyers.Termination;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.Buyers.ChangeOfCircumstances;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces;
using IARA.Interfaces.CatchSales;
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
    public class BuyersController : BaseAuditController
    {
        private static readonly PageCodeEnum[] PAGE_CODES = new PageCodeEnum[]
        {
            PageCodeEnum.RegFirstSaleBuyer,
            PageCodeEnum.RegFirstSaleCenter,
            PageCodeEnum.ChangeFirstSaleBuyer,
            PageCodeEnum.ChangeFirstSaleCenter,
            PageCodeEnum.TermFirstSaleBuyer,
            PageCodeEnum.TermFirstSaleCenter,
            PageCodeEnum.DupFirstSaleBuyer,
            PageCodeEnum.DupFirstSaleCenter
        };

        private readonly IBuyersService service;
        private readonly IFileService fileService;
        private readonly IApplicationService applicationService;
        private readonly IApplicationsRegisterService applicationsRegisterService;
        private readonly IDeliveryService deliveryService;
        private readonly ILogBooksService logBoooksService;
        private readonly IUserService userService;

        public BuyersController(IBuyersService service,
                                IPermissionsService permissionsService,
                                IFileService fileService,
                                IApplicationService applicationService,
                                IApplicationsRegisterService applicationsRegisterService,
                                IDeliveryService deliveryService,
                                ILogBooksService logBoooksService,
                                IUserService userService)
            : base(permissionsService)
        {
            this.service = service;
            this.fileService = fileService;
            this.applicationService = applicationService;
            this.applicationsRegisterService = applicationsRegisterService;
            this.deliveryService = deliveryService;
            this.logBoooksService = logBoooksService;
            this.userService = userService;
        }

        // Register
        [HttpPost]
        [CustomAuthorize(Permissions.BuyersReadAll, Permissions.BuyersRead)]
        public IActionResult GetAll([FromBody] GridRequestModel<BuyersFilters> request)
        {
            if (!CurrentUser.Permissions.Contains(Permissions.BuyersReadAll))
            {
                int? territoryUnitId = userService.GetUserTerritoryUnitId(CurrentUser.ID);

                if (territoryUnitId.HasValue)
                {
                    if (request.Filters == null)
                    {
                        request.Filters = new BuyersFilters
                        {
                            ShowInactiveRecords = false
                        };
                    }

                    request.Filters.TerritoryUnitId = territoryUnitId.Value;
                }
                else
                {
                    return Ok(Enumerable.Empty<BuyerDTO>());
                }
            }

            IQueryable<BuyerDTO> results = service.GetAll(request.Filters);
            return PageResult(results, request, false);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.BuyersReadAll, Permissions.BuyersRead)]
        public IActionResult Get([FromQuery] int id)
        {
            BuyerEditDTO result = service.GetRegisterEntry(id);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.BuyersReadAll, Permissions.BuyersRead)]
        public IActionResult GetApplicationDataForRegister([FromQuery] int applicationId)
        {
            BuyerEditDTO result = service.GetEntryByApplicationId(applicationId);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.BuyersReadAll, Permissions.BuyersRead)]
        public IActionResult GetRegisterByApplicationId([FromQuery] int applicationId)
        {
            BuyerEditDTO result = service.GetRegisterByApplicationId(applicationId);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.BuyersReadAll, Permissions.BuyersRead)]
        public IActionResult GetRegisterByChangeOfCircumstancesApplicationId([FromQuery] int applicationId)
        {
            BuyerEditDTO result = service.GetRegisterByChangeOfCircumstancesApplicationId(applicationId);
            return Ok(result);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.BuyersAddRecords)]
        public IActionResult Add([FromForm] BuyerEditDTO item, [FromQuery] bool ignoreLogBookConflicts)
        {
            try
            {
                int id = service.AddRegisterEntry(item, ignoreLogBookConflicts);
                return Ok(id);
            }
            catch (InvalidLogBookPagesRangeException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, errorCode: ErrorCode.InvalidLogBookPagesRange);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.BuyersAddRecords)]
        public async Task<IActionResult> AddAndDownloadRegister([FromForm] BuyerEditDTO item, [FromQuery] bool ignoreLogBookConflicts)
        {
            try
            {
                int registerId = service.AddRegisterEntry(item, ignoreLogBookConflicts);

                DownloadableFileDTO file = await service.GetRegisterFileForDownload(registerId, item.BuyerType!.Value);
                return File(file.Bytes, file.MimeType, file.FileName);
            }
            catch (InvalidLogBookPagesRangeException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, errorCode: ErrorCode.InvalidLogBookPagesRange);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.BuyersEditRecords)]
        public IActionResult Edit([FromForm] BuyerEditDTO buyer, [FromQuery] bool ignoreLogBookConflicts)
        {
            try
            {
                service.EditRegisterEntry(buyer, ignoreLogBookConflicts);
                return Ok();
            }
            catch (InvalidLogBookPagesRangeException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, errorCode: ErrorCode.InvalidLogBookPagesRange);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.BuyersEditRecords)]
        public async Task<IActionResult> EditAndDownloadRegister([FromForm] BuyerEditDTO buyer, [FromQuery] bool ignoreLogBookConflicts)
        {
            try
            {
                service.EditRegisterEntry(buyer, ignoreLogBookConflicts);

                DownloadableFileDTO file = await service.GetRegisterFileForDownload(buyer.Id!.Value, buyer.BuyerType!.Value);
                return File(file.Bytes, file.MimeType, file.FileName);
            }
            catch (InvalidLogBookPagesRangeException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, errorCode: ErrorCode.InvalidLogBookPagesRange);
            }
        }

        [HttpGet]
        [CustomAuthorize(Permissions.BuyersReadAll, Permissions.BuyersRead)]
        public async Task<IActionResult> DownloadRegister([FromQuery] int buyerId, [FromQuery] BuyerTypesEnum buyerType)
        {
            DownloadableFileDTO file = await service.GetRegisterFileForDownload(buyerId, buyerType);
            return File(file.Bytes, file.MimeType, file.FileName);
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.BuyersDeleteRecords)]
        public IActionResult Delete([FromQuery] int id)
        {
            service.Delete(id);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.BuyersRestoreRecords)]
        public IActionResult UndoDelete([FromQuery] int id)
        {
            service.Restore(id);
            return Ok();
        }

        [HttpPut]
        [CustomAuthorize(Permissions.BuyersEditRecords)]
        public IActionResult UpdateBuyerStatus([FromQuery] int buyerId, [FromQuery] int? applicationId, [FromBody] CancellationHistoryEntryDTO status)
        {
            service.UpdateBuyerStatus(buyerId, status, applicationId);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.BuyersReadAll, Permissions.BuyersRead)]
        public IActionResult DownloadFile(int id)
        {
            DownloadableFileDTO file = fileService.GetFileForDownload(id);
            return File(file.Bytes, file.MimeType, file.FileName);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.BuyersReadAll, Permissions.BuyersRead)]
        public override IActionResult GetAuditInfo(int id)
        {
            return Ok(service.GetSimpleAudit(id));
        }

        [HttpGet]
        [CustomAuthorize(Permissions.BuyersReadAll, Permissions.BuyersRead)]
        public IActionResult GetPremiseUsageDocumentAudit([FromQuery] int id)
        {
            SimpleAuditDTO audit = service.GetPremiseUsageDocumentSimpleAudit(id);
            return Ok(audit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.BuyersReadAll, Permissions.BuyersRead)]
        public IActionResult GetBuyerLicenseAudit([FromQuery] int id)
        {
            SimpleAuditDTO audit = service.GetBuyerLicenseSimpleAudit(id);
            return Ok(audit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.BuyersReadAll, Permissions.BuyersRead)]
        public IActionResult GetLogBookAudit([FromQuery] int id)
        {
            SimpleAuditDTO audit = logBoooksService.GetSimpleAudit(id);
            return Ok(audit);
        }

        // Applications
        [HttpPost]
        [CustomAuthorize(Permissions.BuyersApplicationsReadAll, Permissions.BuyersApplicationsRead)]
        public IActionResult GetAllApplications([FromBody] GridRequestModel<ApplicationsRegisterFilters> request)
        {
            if (!CurrentUser.Permissions.Contains(Permissions.BuyersApplicationsReadAll))
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

            IQueryable<ApplicationRegisterDTO> applications = applicationsRegisterService.GetAllApplications(request.Filters, null, PAGE_CODES);
            return PageResult(applications, request, false);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.BuyersApplicationsEditRecords)]
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
        [CustomAuthorize(Permissions.BuyersApplicationsReadAll, Permissions.BuyersApplicationsRead)]
        public IActionResult GetApplication([FromQuery] int id)
        {
            BuyerApplicationEditDTO result = service.GetApplicationEntry(id);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.BuyersReadAll, Permissions.BuyersRead)]
        public IActionResult GetRegixData([FromQuery] int applicationId)
        {
            RegixChecksWrapperDTO<BuyerRegixDataDTO> result = service.GetRegixData(applicationId);
            return Ok(result);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.BuyersApplicationsAddRecords)]
        public async Task<IActionResult> AddApplication([FromForm] BuyerApplicationEditDTO application)
        {
            bool hasDelivery = deliveryService.HasApplicationDelivery(application.ApplicationId!.Value);
            if (hasDelivery != application.HasDelivery!.Value)
            {
                throw new Exception("Mismatch between HasDelivery in model and in database");
            }

            if (hasDelivery)
            {
                bool hasEDelivery = await deliveryService.HasSubmittedForEDelivery(application.DeliveryData.DeliveryTypeId,
                                                                                   application.SubmittedFor,
                                                                                   application.SubmittedBy);

                if (hasEDelivery == false)
                {
                    return ValidationFailedResult(new List<string> { nameof(ApplicationValidationErrorsEnum.NoEDeliveryRegistration) });
                }
            }

            int id = service.AddApplicationEntry(application, ApplicationStatusesEnum.EXT_CHK_STARTED);
            return Ok(id);
        }

        // Change of circumstances
        [HttpGet]
        [CustomAuthorize(Permissions.BuyersApplicationsReadAll, Permissions.BuyersApplicationsRead)]
        public IActionResult GetBuyerChangeOfCircumstancesApplication([FromQuery] int id)
        {
            BuyerChangeOfCircumstancesApplicationDTO result = service.GetBuyerChangeOfCircumstancesApplication(id);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.BuyersApplicationsInspectAndCorrectRegiXData)]
        public IActionResult GetBuyerChangeOfCircumstancesRegixData([FromQuery] int applicationId)
        {
            RegixChecksWrapperDTO<BuyerChangeOfCircumstancesRegixDataDTO> data = service.GetBuyerChangeOfCircumstancesRegixData(applicationId);
            return Ok(data);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.BuyersApplicationsAddRecords)]
        public async Task<IActionResult> AddBuyerChangeOfCircumstancesApplication([FromForm] BuyerChangeOfCircumstancesApplicationDTO application)
        {
            bool hasDelivery = deliveryService.HasApplicationDelivery(application.ApplicationId!.Value);
            if (hasDelivery != application.HasDelivery!.Value)
            {
                throw new Exception("Mismatch between HasDelivery in model and in database");
            }

            if (hasDelivery)
            {
                bool hasEDelivery = await deliveryService.HasSubmittedForEDelivery(application.DeliveryData.DeliveryTypeId,
                                                                                   application.SubmittedFor,
                                                                                   application.SubmittedBy);

                if (hasEDelivery == false)
                {
                    return ValidationFailedResult(new List<string> { nameof(ApplicationValidationErrorsEnum.NoEDeliveryRegistration) });
                }
            }

            try
            {
                int id = service.AddBuyerChangeOfCircumstancesApplication(application, ApplicationStatusesEnum.EXT_CHK_STARTED);
                return Ok(id);
            }
            catch (BuyerDoesNotExistsException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, ErrorCode.BuyerDoesNotExist);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.BuyersApplicationsEditRecords)]
        public IActionResult EditBuyerChangeOfCircumstancesApplicationAndStartRegixChecks([FromForm] BuyerChangeOfCircumstancesRegixDataDTO application)
        {
            service.EditBuyerChangeOfCircumstancesRegixData(application);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.BuyersApplicationsReadAll, Permissions.BuyersApplicationsRead)]
        public IActionResult GetBuyerFromChangeOfCircumstancesApplication([FromQuery] int applicationId)
        {
            BuyerEditDTO result = service.GetBuyerFromChangeOfCircumstancesApplication(applicationId);
            return Ok(result);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.AquacultureFacilitiesApplicationsRead)]
        public IActionResult CompleteBuyerChangeOfCircumstancesApplication([FromForm] BuyerEditDTO buyer, [FromQuery] bool ignoreLogBookConflicts)
        {
            service.CompleteChangeOfCircumstancesApplication(buyer, ignoreLogBookConflicts);
            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.BuyersApplicationsEditRecords)]
        public async Task<IActionResult> EditApplication([FromForm] BuyerApplicationEditDTO application, [FromQuery] bool saveAsDraft)
        {
            bool hasDelivery = deliveryService.HasApplicationDelivery(application.ApplicationId!.Value);
            if (hasDelivery != application.HasDelivery!.Value)
            {
                throw new Exception("Mismatch between HasDelivery in model and in database");
            }

            if (hasDelivery)
            {
                bool hasEDelivery = await deliveryService.HasSubmittedForEDelivery(application.DeliveryData.DeliveryTypeId,
                                                                                   application.SubmittedFor,
                                                                                   application.SubmittedBy);

                if (hasEDelivery == false)
                {
                    return ValidationFailedResult(new List<string> { nameof(ApplicationValidationErrorsEnum.NoEDeliveryRegistration) });
                }
            }

            int id;

            if (saveAsDraft)
            {
                id = service.EditApplicationEntry(application);
            }
            else
            {
                id = service.EditApplicationEntry(application, ApplicationStatusesEnum.EXT_CHK_STARTED);
            }

            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.BuyersApplicationsEditRecords)]
        public async Task<IActionResult> EditBuyerChangeOfCircumstancesApplication([FromForm] BuyerChangeOfCircumstancesApplicationDTO application, [FromQuery] bool saveAsDraft)
        {
            bool hasDelivery = deliveryService.HasApplicationDelivery(application.ApplicationId!.Value);
            if (hasDelivery != application.HasDelivery!.Value)
            {
                throw new Exception("Mismatch between HasDelivery in model and in database");
            }

            if (hasDelivery)
            {
                bool hasEDelivery = await deliveryService.HasSubmittedForEDelivery(application.DeliveryData.DeliveryTypeId,
                                                                                   application.SubmittedFor,
                                                                                   application.SubmittedBy);

                if (hasEDelivery == false)
                {
                    return ValidationFailedResult(new List<string> { nameof(ApplicationValidationErrorsEnum.NoEDeliveryRegistration) });
                }
            }

            try
            {
                if (saveAsDraft)
                {
                    service.EditBuyerChangeOfCircumstancesApplication(application);
                }
                else
                {
                    service.EditBuyerChangeOfCircumstancesApplication(application, ApplicationStatusesEnum.EXT_CHK_STARTED);
                }

                return Ok();
            }
            catch (BuyerDoesNotExistsException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, ErrorCode.BuyerDoesNotExist);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.BuyersApplicationsInspectAndCorrectRegiXData)]
        public IActionResult EditApplicationAndStartRegixChecks([FromForm] BuyerRegixDataDTO buyer)
        {
            return Ok(service.EditApplicationRegixData(buyer));
        }

        // Termination
        [HttpGet]
        [CustomAuthorize(Permissions.BuyersApplicationsReadAll, Permissions.BuyersApplicationsRead)]
        public IActionResult GetBuyerTerminationApplication([FromQuery] int id)
        {
            BuyerTerminationApplicationDTO result = service.GetBuyerTerminationApplication(id);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.BuyersApplicationsInspectAndCorrectRegiXData)]
        public IActionResult GetBuyerTerminationRegixData([FromQuery] int applicationId)
        {
            RegixChecksWrapperDTO<BuyerTerminationRegixDataDTO> data = service.GetBuyerTerminationRegixData(applicationId);
            return Ok(data);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.BuyersApplicationsAddRecords)]
        public async Task<IActionResult> AddBuyerTerminationApplication([FromForm] BuyerTerminationApplicationDTO application)
        {
            bool hasDelivery = deliveryService.HasApplicationDelivery(application.ApplicationId!.Value);
            if (hasDelivery != application.HasDelivery!.Value)
            {
                throw new Exception("Mismatch between HasDelivery in model and in database");
            }

            if (hasDelivery)
            {
                bool hasEDelivery = await deliveryService.HasSubmittedForEDelivery(application.DeliveryData.DeliveryTypeId,
                                                                                   application.SubmittedFor,
                                                                                   application.SubmittedBy);

                if (hasEDelivery == false)
                {
                    return ValidationFailedResult(new List<string> { nameof(ApplicationValidationErrorsEnum.NoEDeliveryRegistration) });
                }
            }

            try
            {
                int id = service.AddBuyerTerminationApplication(application, ApplicationStatusesEnum.EXT_CHK_STARTED);
                return Ok(id);
            }
            catch (BuyerDoesNotExistsException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, ErrorCode.BuyerDoesNotExist);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.BuyersApplicationsEditRecords)]
        public async Task<IActionResult> EditBuyerTerminationApplication([FromForm] BuyerTerminationApplicationDTO application, [FromQuery] bool saveAsDraft)
        {
            bool hasDelivery = deliveryService.HasApplicationDelivery(application.ApplicationId!.Value);
            if (hasDelivery != application.HasDelivery!.Value)
            {
                throw new Exception("Mismatch between HasDelivery in model and in database");
            }

            if (hasDelivery)
            {
                bool hasEDelivery = await deliveryService.HasSubmittedForEDelivery(application.DeliveryData.DeliveryTypeId,
                                                                                   application.SubmittedFor,
                                                                                   application.SubmittedBy);

                if (hasEDelivery == false)
                {
                    return ValidationFailedResult(new List<string> { nameof(ApplicationValidationErrorsEnum.NoEDeliveryRegistration) });
                }
            }

            try
            {
                if (saveAsDraft)
                {
                    service.EditBuyerTerminationApplication(application);
                }
                else
                {
                    service.EditBuyerTerminationApplication(application, ApplicationStatusesEnum.EXT_CHK_STARTED);
                }
                
                return Ok();
            }
            catch (BuyerDoesNotExistsException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, ErrorCode.BuyerDoesNotExist);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.BuyersApplicationsEditRecords)]
        public IActionResult EditBuyerTerminationApplicationAndStartRegixChecks([FromForm] BuyerTerminationRegixDataDTO application)
        {
            service.EditBuyerTerminationRegixData(application);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.BuyersApplicationsReadAll, Permissions.BuyersApplicationsRead)]
        public IActionResult GetBuyerFromTerminationApplication([FromQuery] int applicationId)
        {
            BuyerEditDTO result = service.GetBuyerFromTerminationApplication(applicationId);
            return Ok(result);
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.BuyersApplicationsDeleteRecords)]
        public IActionResult DeleteApplication([FromQuery] int id)
        {
            applicationsRegisterService.DeleteApplication(id);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.BuyersApplicationsRestoreRecords)]
        public IActionResult UndoDeleteApplication([FromQuery] int id)
        {
            applicationsRegisterService.UndoDeleteApplication(id);
            return Ok();
        }

        // nomenclatures
        [HttpGet]
        [CustomAuthorize(Permissions.BuyersReadAll, Permissions.BuyersRead)]
        public IActionResult GetBuyerTypes()
        {
            return Ok(service.GetEntityTypes());
        }

        [HttpGet]
        [CustomAuthorize(Permissions.BuyersReadAll, Permissions.BuyersRead)]
        public IActionResult GetAllBuyersNomenclatures()
        {
            return Ok(service.GetAllBuyersNomenclatures());
        }

        [HttpGet]
        [CustomAuthorize(Permissions.BuyersReadAll, Permissions.BuyersRead)]
        public IActionResult GetAllFirstSaleCentersNomenclatures()
        {
            return Ok(service.GetAllFirstSaleCentersNomenclatures());
        }

        [HttpGet]
        [CustomAuthorize(Permissions.BuyersApplicationsReadAll, Permissions.BuyersApplicationsRead)]
        public IActionResult GetApplicationStatuses()
        {
            List<NomenclatureDTO> statuses = applicationsRegisterService.GetApplicationStatuses(ApplicationHierarchyTypesEnum.Online, ApplicationHierarchyTypesEnum.OnPaper);
            return Ok(statuses);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.BuyersApplicationsReadAll, Permissions.BuyersApplicationsRead)]
        public IActionResult GetApplicationTypes()
        {
            List<NomenclatureDTO> types = applicationsRegisterService.GetApplicationTypes(new PageCodeEnum[]
                                                                                          {
                                                                                             PageCodeEnum.RegFirstSaleCenter,
                                                                                             PageCodeEnum.ChangeFirstSaleCenter,
                                                                                             PageCodeEnum.TermFirstSaleCenter,
                                                                                             PageCodeEnum.RegFirstSaleBuyer,
                                                                                             PageCodeEnum.ChangeFirstSaleBuyer,
                                                                                             PageCodeEnum.TermFirstSaleBuyer
                                                                                         });
            return Ok(types);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.BuyersApplicationsReadAll, Permissions.BuyersApplicationsRead)]
        public IActionResult GetApplicationSources()
        {
            List<NomenclatureDTO> sources = applicationsRegisterService.GetApplicationSources(ApplicationHierarchyTypesEnum.Online, ApplicationHierarchyTypesEnum.OnPaper);
            return Ok(sources);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.BuyersReadAll, Permissions.BuyersRead)]
        public IActionResult GetBuyerStatuses()
        {
            List<NomenclatureDTO> statuses = service.GetBuyerStatuses().ToList();
            return Ok(statuses);
        }
    }
}
