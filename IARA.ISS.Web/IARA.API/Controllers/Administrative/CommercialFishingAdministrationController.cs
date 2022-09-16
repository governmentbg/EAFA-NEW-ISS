using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using IARA.Common.Enums;
using IARA.Common.Exceptions;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.ApplicationsRegister;
using IARA.DomainModels.DTOModels.CatchesAndSales;
using IARA.DomainModels.DTOModels.CommercialFishingRegister;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.QualifiedFrishersRegister;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces;
using IARA.Interfaces.CatchSales;
using IARA.Interfaces.CommercialFishing;
using IARA.Interfaces.Nomenclatures;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Enums;
using IARA.WebHelpers.Models;
using Microsoft.AspNetCore.Mvc;
using TL.Caching.Interfaces;

namespace IARA.Web.Controllers.Administrative
{
    [AreaRoute(AreaType.Administrative)]
    public class CommercialFishingAdministrationController : BaseAuditController
    {
        private static readonly PageCodeEnum[] PERMITS_PAGE_CODES = new PageCodeEnum[]
        {
            PageCodeEnum.CommFish,
            PageCodeEnum.RightToFishThirdCountry,
            PageCodeEnum.PoundnetCommFish,
            PageCodeEnum.DupCommFish,
            PageCodeEnum.DupRightToFishThirdCountry,
            PageCodeEnum.DupPoundnetCommFish
        };

        private static readonly PageCodeEnum[] PERMIT_LICENSES_PAGE_CODES = new PageCodeEnum[]
        {
            PageCodeEnum.RightToFishResource,
            PageCodeEnum.PoundnetCommFishLic,
            PageCodeEnum.CatchQuataSpecies,
            PageCodeEnum.DupRightToFishResource,
            PageCodeEnum.DupPoundnetCommFishLic,
            PageCodeEnum.DupCatchQuataSpecies
        };

        private readonly ICommercialFishingService service;
        private readonly ICommercialFishingNomenclaturesService nomenclaturesService;
        private readonly IApplicationService applicationService;
        private readonly IApplicationsRegisterService applicationsRegisterService;
        private readonly IFileService fileService;
        private readonly IFishingGearsService fishingGearsService;
        private readonly IShipsRegisterNomenclaturesService shipNomenclaturesService;
        private readonly ILogBooksService logBooksService;
        private readonly IUserService userService;
        private readonly IMemoryCacheService memoryCacheService;

        public CommercialFishingAdministrationController(IPermissionsService permissionsService,
                                                         ICommercialFishingService commercialFishingService,
                                                         ICommercialFishingNomenclaturesService commercialFishingNomenclaturesService,
                                                         IApplicationService applicationService,
                                                         IApplicationsRegisterService applicationsRegisterService,
                                                         IFileService fileService,
                                                         IFishingGearsService fishingGearsService,
                                                         IShipsRegisterNomenclaturesService shipNomenclaturesService,
                                                         ILogBooksService logBooksService,
                                                         IUserService userService,
                                                         IMemoryCacheService memoryCacheService)
            : base(permissionsService)
        {
            service = commercialFishingService;
            nomenclaturesService = commercialFishingNomenclaturesService;
            this.applicationService = applicationService;
            this.applicationsRegisterService = applicationsRegisterService;
            this.fileService = fileService;
            this.fishingGearsService = fishingGearsService;
            this.shipNomenclaturesService = shipNomenclaturesService;
            this.logBooksService = logBooksService;
            this.userService = userService;
            this.memoryCacheService = memoryCacheService;
        }

        // Register

        [HttpPost]
        [CustomAuthorize(Permissions.CommercialFishingPermitRegisterReadAll,
                         Permissions.CommercialFishingPermitRegisterRead,
                         Permissions.ShipsRegisterReadAll,
                         Permissions.ShipsRegisterRead)]
        public IActionResult GetAllPermits([FromBody] GridRequestModel<CommercialFishingRegisterFilters> request)
        {
            if (!CurrentUser.Permissions.Contains(Permissions.CommercialFishingPermitRegisterReadAll) && request.Filters?.ShipId == null)
            {
                int? territoryUnitId = userService.GetUserTerritoryUnitId(CurrentUser.ID);

                if (territoryUnitId.HasValue)
                {
                    if (request.Filters == null)
                    {
                        request.Filters = new CommercialFishingRegisterFilters
                        {
                            ShowInactiveRecords = false
                        };
                    }

                    request.Filters.PermitTerritoryUnitId = territoryUnitId.Value;

                    if (CurrentUser.Permissions.Contains(Permissions.CommercialFishingPermitLicenseRegisterRead)
                        || CurrentUser.Permissions.Contains(Permissions.CommercialFishingPermitLicenseRegisterReadAll))
                    {
                        request.Filters.PermitLicenseTerritoryUnitId = territoryUnitId.Value;
                    }
                }
                else
                {
                    return Ok(Enumerable.Empty<CommercialFishingPermitRegisterDTO>());
                }
            }

            IQueryable<CommercialFishingPermitRegisterDTO> permits = service.GetAllCommercialFishingPermits(request.Filters);
            return PageResult(permits, request, false);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.CommercialFishingPermitRegisterReadAll,
                         Permissions.CommercialFishingPermitRegisterRead,
                         Permissions.ShipsRegisterReadAll,
                         Permissions.ShipsRegisterRead)]
        public IActionResult GetPermitLicensesForTable([FromBody] PermitLicenseData request)
        {
            if (!CurrentUser.Permissions.Contains(Permissions.CommercialFishingPermitLicenseRegisterReadAll) && request.Filters?.ShipId == null)
            {
                int? territoryUnitId = userService.GetUserTerritoryUnitId(CurrentUser.ID);

                if (territoryUnitId.HasValue)
                {
                    if (request.Filters == null)
                    {
                        request.Filters = new CommercialFishingRegisterFilters
                        {
                            ShowInactiveRecords = false
                        };
                    }

                    request.Filters.PermitTerritoryUnitId = territoryUnitId.Value;
                    request.Filters.PermitLicenseTerritoryUnitId = territoryUnitId.Value;
                }
                else
                {
                    return Ok(Enumerable.Empty<CommercialFishingPermitLicenseRegisterDTO>());
                }
            }

            List<CommercialFishingPermitLicenseRegisterDTO> result = service.GetPermitLicensesForTable(request.PermitIds, request.Filters).ToList();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CommercialFishingPermitRegisterReadAll, Permissions.CommercialFishingPermitRegisterRead)]
        public IActionResult GetPermit([FromQuery] int id)
        {
            CommercialFishingEditDTO permit = service.GetPermit(id);
            return Ok(permit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CommercialFishingPermitLicenseRegisterReadAll, Permissions.CommercialFishingPermitLicenseRegisterRead)]
        public IActionResult GetPermitLicense([FromQuery] int id)
        {
            CommercialFishingEditDTO permit = service.GetPermitLicense(id);
            return Ok(permit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CommercialFishingPermitRegisterReadAll, Permissions.CommercialFishingPermitRegisterRead)]
        public IActionResult GetPermitRegisterByApplicationId([FromQuery] int applicationId)
        {
            CommercialFishingEditDTO permit = service.GetPermitByApplicationId(applicationId);
            return Ok(permit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CommercialFishingPermitLicenseRegisterReadAll, Permissions.CommercialFishingPermitLicenseRegisterRead)]
        public IActionResult GetPermitLicenseRegisterByApplicationId([FromQuery] int applicationId)
        {
            CommercialFishingEditDTO permit = service.GetPermitLicenseByApplicationId(applicationId);
            return Ok(permit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CommercialFishingPermitRegisterReadAll, Permissions.CommercialFishingPermitRegisterRead)]
        public override IActionResult GetAuditInfo(int id)
        {
            return Ok(service.GetSimpleAudit(id));
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CommercialFishingPermitLicenseRegisterReadAll, Permissions.CommercialFishingPermitLicenseRegisterRead)]
        public IActionResult GetPermitLicenseAuditInfo(int id)
        {
            return Ok(service.GetPermitLicenseSimpleAudit(id));
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CommercialFishingPermitRegisterReadAll, Permissions.CommercialFishingPermitRegisterRead)]
        public IActionResult GetPermitApplicationDataForRegister([FromQuery] int applicationId)
        {
            CommercialFishingEditDTO permit = service.GetPermitApplicationDataForRegister(applicationId);
            return Ok(permit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CommercialFishingPermitLicenseRegisterReadAll, Permissions.CommercialFishingPermitLicenseRegisterRead)]
        public IActionResult GetPermitLicenseApplicationDataForRegister([FromQuery] int applicationId)
        {
            CommercialFishingEditDTO permit = service.GetPermitLicenseApplicationDataForRegister(applicationId);
            return Ok(permit);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.CommercialFishingPermitLicenseApplicationsReadAll, Permissions.CommercialFishingPermitLicenseApplicationsRead)]
        public IActionResult CalculatePermitLicenseAppliedTariffs([FromBody] PermitLicenseTariffCalculationParameters tariffCalculationParameters)
        {
            List<PaymentTariffDTO> appliedTariffs = service.CalculatePermitLicenseAppliedTariffs(tariffCalculationParameters);
            return Ok(appliedTariffs);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.CommercialFishingPermitLicenseRegisterRead, Permissions.CommercialFishingPermitLicenseRegisterReadAll)]
        public IActionResult GetOverlappedLogBooks([FromBody] List<OverlappingLogBooksParameters> ranges)
        {
            List<RangeOverlappingLogBooksDTO> results = logBooksService.GetOverlappedLogBooks(ranges);
            return Ok(results);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.CommercialFishingPermitRegisterAddRecords)]
        public IActionResult AddPermit([FromForm] CommercialFishingEditDTO permit)
        {
            int id = service.AddPermit(permit);
            ForceRefreshShipsNomenclature();
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.CommercialFishingPermitRegisterAddRecords)]
        public async Task<IActionResult> AddPermitAndDownloadRegister([FromForm] CommercialFishingEditDTO permit)
        {
            int registerId = service.AddPermit(permit);
            ForceRefreshShipsNomenclature();
            DownloadableFileDTO file = await service.GetPermitRegisterFileForDownload(registerId, permit.Type!.Value);
            return File(file.Bytes, file.MimeType, file.FileName);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CommercialFishingPermitRegisterRead, Permissions.CommercialFishingPermitRegisterReadAll)]
        public async Task<IActionResult> DownloadPermitRegister([FromQuery] int registerId)
        {
            CommercialFishingTypesEnum permitType = service.GetPermitType(registerId);
            DownloadableFileDTO file = await service.GetPermitRegisterFileForDownload(registerId, permitType);
            return File(file.Bytes, file.MimeType, file.FileName);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.CommercialFishingPermitLicenseRegisterAddRecords)]
        [RequestFormLimits(ValueCountLimit = 5000)]
        public IActionResult AddPermitLicense([FromForm] CommercialFishingEditDTO permitLicense, [FromQuery] bool ignoreLogBookConflicts)
        {
            try
            {
                int id = service.AddPermitLicense(permitLicense, ignoreLogBookConflicts);
                ForceRefreshShipsNomenclature();
                return Ok(id);
            }
            catch (InvalidLogBookPagesRangeException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, errorCode: ErrorCode.InvalidLogBookPagesRange);
            }
            catch (InvalidLogBookLicensePagesRangeException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, errorCode: ErrorCode.InvalidLogBookLicensePagesRange);
            }
            catch (NoPermitRegisterForPermitLicenseException ex)
            {
                return ValidationFailedResult( new List<string> { ex.Message}, errorCode: ErrorCode.NoPermitRegisterForPermitLicense );
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.CommercialFishingPermitLicenseRegisterAddRecords)]
        [RequestFormLimits(ValueCountLimit = 5000)]
        public async Task<IActionResult> AddPermitLicenseAndDownloadRegister([FromForm] CommercialFishingEditDTO permitLicense, [FromQuery] bool ignoreLogBookConflicts)
        {
            try
            {
                int registerId = service.AddPermitLicense(permitLicense, ignoreLogBookConflicts);
                ForceRefreshShipsNomenclature();
                DownloadableFileDTO file = await service.GetPermitLicenseRegisterFileForDownload(registerId, permitLicense.Type!.Value);
                return File(file.Bytes, file.MimeType, file.FileName);
            }
            catch (InvalidLogBookPagesRangeException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, errorCode: ErrorCode.InvalidLogBookPagesRange);
            }
            catch (InvalidLogBookLicensePagesRangeException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, errorCode: ErrorCode.InvalidLogBookLicensePagesRange);
            }
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CommercialFishingPermitLicenseRegisterRead, Permissions.CommercialFishingPermitLicenseRegisterReadAll)]
        public async Task<IActionResult> DownloadPermitLicenseRegister([FromQuery] int registerId)
        {
            CommercialFishingTypesEnum permitType = service.GetPermitLicenseType(registerId);
            DownloadableFileDTO file = await service.GetPermitLicenseRegisterFileForDownload(registerId, permitType);
            return File(file.Bytes, file.MimeType, file.FileName);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.CommercialFishingPermitRegisterEditRecords)]
        public IActionResult EditPermit([FromForm] CommercialFishingEditDTO permit)
        {
            service.EditPermit(permit, CurrentUser.ID);
            ForceRefreshShipsNomenclature();
            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.CommercialFishingPermitRegisterEditRecords)]
        public async Task<IActionResult> EditPermitAndDownloadRegister([FromForm] CommercialFishingEditDTO permit)
        {
            service.EditPermit(permit, CurrentUser.ID);
            ForceRefreshShipsNomenclature();
            DownloadableFileDTO file = await service.GetPermitRegisterFileForDownload(permit.Id!.Value, permit.Type!.Value);
            return File(file.Bytes, file.MimeType, file.FileName);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.CommercialFishingPermitLicenseRegisterEditRecords)]
        [RequestFormLimits(ValueCountLimit = 5000)]
        public IActionResult EditPermitLicense([FromForm] CommercialFishingEditDTO permitLicense, [FromQuery] bool ignoreLogBookConflicts)
        {
            try
            {
                service.EditPermitLicense(permitLicense, CurrentUser.ID, ignoreLogBookConflicts);
                ForceRefreshShipsNomenclature();

                return Ok();
            }
            catch (InvalidLogBookPagesRangeException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, errorCode: ErrorCode.InvalidLogBookPagesRange);
            }
            catch (InvalidLogBookLicensePagesRangeException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, errorCode: ErrorCode.InvalidLogBookLicensePagesRange);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.CommercialFishingPermitLicenseRegisterEditRecords)]
        [RequestFormLimits(ValueCountLimit = 5000)]
        public async Task<IActionResult> EditPermitLicenseAndDownloadRegister([FromForm] CommercialFishingEditDTO permitLicense, [FromQuery] bool ignoreLogBookConflicts)
        {
            try
            {
                service.EditPermitLicense(permitLicense, CurrentUser.ID, ignoreLogBookConflicts);
                ForceRefreshShipsNomenclature();

                DownloadableFileDTO file = await service.GetPermitLicenseRegisterFileForDownload(permitLicense.Id!.Value, permitLicense.Type!.Value);
                return File(file.Bytes, file.MimeType, file.FileName);
            }
            catch (InvalidLogBookPagesRangeException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, errorCode: ErrorCode.InvalidLogBookPagesRange);
            }
            catch (InvalidLogBookLicensePagesRangeException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, errorCode: ErrorCode.InvalidLogBookLicensePagesRange);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.CommercialFishingPermitRegisterRead, Permissions.CommercialFishingPermitRegisterReadAll)]
        public IActionResult DownloadPermitFluxXml([FromBody] CommercialFishingEditDTO permit)
        {
            XmlSerializer serializer = new(typeof(CommercialFishingEditDTO));
            using StringWriter sww = new();
            using XmlWriter writer = XmlWriter.Create(sww);

            serializer.Serialize(writer, permit);

            string xml = sww.ToString();
            return File(Encoding.UTF8.GetBytes(xml), "application/xml", $"permit_{permit.PermitRegistrationNumber}");
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CommercialFishingPermitRegisterReadAll,
                         Permissions.CommercialFishingPermitRegisterRead,
                         Permissions.CommercialFishingPermitApplicationsReadAll,
                         Permissions.CommercialFishingPermitApplicationsRead,
                         Permissions.CommercialFishingPermitLicenseRegisterReadAll,
                         Permissions.CommercialFishingPermitLicenseRegisterRead,
                         Permissions.CommercialFishingPermitLicenseApplicationsReadAll,
                         Permissions.CommercialFishingPermitLicenseApplicationsRead)]
        public IActionResult DownloadFile(int id)
        {
            DownloadableFileDTO file = fileService.GetFileForDownload(id);
            return File(file.Bytes, file.MimeType, file.FileName);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CommercialFishingPermitLicenseRegisterReadAll, Permissions.CommercialFishingPermitLicenseRegisterRead)]
        public IActionResult GetLogBookSimpleAudit([FromQuery] int id)
        {
            return Ok(logBooksService.GetSimpleAudit(id));
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CommercialFishingPermitRegisterReadAll, Permissions.CommercialFishingPermitRegisterRead)]
        public IActionResult GetPermitSuspensionSimpleAudit([FromQuery] int id)
        {
            return Ok(service.GetPermitSuspensionSimpleAudit(id));
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CommercialFishingPermitLicenseRegisterReadAll, Permissions.CommercialFishingPermitLicenseRegisterRead)]
        public IActionResult GetPermitLicenseSuspensionSimpleAudit([FromQuery] int id)
        {
            return Ok(service.GetPermitLicenseSuspensionSimpleAudit(id));
        }

        // Fishing gears

        [HttpGet]
        [CustomAuthorize(Permissions.CommercialFishingPermitLicenseApplicationsReadAll,
                         Permissions.CommercialFishingPermitLicenseApplicationsRead,
                         Permissions.CommercialFishingPermitLicenseRegisterReadAll,
                         Permissions.CommercialFishingPermitLicenseRegisterRead,
                         Permissions.ShipsRegisterReadAll,
                         Permissions.ShipsRegisterRead)]
        public IActionResult GetFishingGearSimpleAudit([FromQuery] int id)
        {
            return Ok(fishingGearsService.GetSimpleAudit(id));
        }

        // Applications

        [HttpPost]
        [CustomAuthorize(Permissions.CommercialFishingPermitApplicationsReadAll,
                         Permissions.CommercialFishingPermitApplicationsRead,
                         Permissions.CommercialFishingPermitLicenseApplicationsReadAll,
                         Permissions.CommercialFishingPermitLicenseApplicationsRead)]
        public IActionResult GetAllApplications([FromBody] GridRequestModel<ApplicationsRegisterFilters> request)
        {
            List<PageCodeEnum> permittedPageCodesRead = null;

            if (!CurrentUser.Permissions.Contains(Permissions.CommercialFishingPermitApplicationsReadAll)
                || !CurrentUser.Permissions.Contains(Permissions.CommercialFishingPermitLicenseApplicationsReadAll))
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
                    permittedPageCodesRead = new List<PageCodeEnum>();

                    if (!CurrentUser.Permissions.Contains(Permissions.CommercialFishingPermitApplicationsReadAll)
                        && CurrentUser.Permissions.Contains(Permissions.CommercialFishingPermitApplicationsRead))
                    {
                        permittedPageCodesRead.AddRange(PERMITS_PAGE_CODES);
                    }

                    if (!CurrentUser.Permissions.Contains(Permissions.CommercialFishingPermitLicenseApplicationsReadAll)
                        && CurrentUser.Permissions.Contains(Permissions.CommercialFishingPermitLicenseApplicationsRead))
                    {
                        permittedPageCodesRead.AddRange(PERMIT_LICENSES_PAGE_CODES);
                    }
                }
                else
                {
                    return Ok(Enumerable.Empty<CommercialFishingPermitLicenseRegisterDTO>());
                }
            }

            PageCodeEnum[] permittedPageCodesReadAll = GetAllowedPageCodes().ToArray();

            IQueryable<ApplicationRegisterDTO> permits = applicationsRegisterService.GetAllApplications(request.Filters, null, permittedPageCodesReadAll, permittedPageCodesRead);
            return PageResult(permits, request, false);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CommercialFishingPermitApplicationsReadAll, Permissions.CommercialFishingPermitApplicationsRead)]
        public IActionResult GetPermitApplication([FromQuery] int id)
        {
            CommercialFishingApplicationEditDTO result = service.GetPermitApplication(id);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CommercialFishingPermitLicenseApplicationsReadAll, Permissions.CommercialFishingPermitLicenseApplicationsRead)]
        public IActionResult GetPermitLicenseApplication([FromQuery] int id)
        {
            CommercialFishingApplicationEditDTO result = service.GetPermitLicenseApplication(id);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CommercialFishingPermitLicenseApplicationsReadAll, Permissions.CommercialFishingPermitLicenseApplicationsRead)]
        public IActionResult GetPermitLicenseApplicationDataFromPermit([FromQuery] int permitId, [FromQuery] int applicationId)
        {
            CommercialFishingApplicationEditDTO result = service.GetPermitLicenseApplicationDataFromPermit(permitId, applicationId);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CommercialFishingPermitApplicationsInspectAndCorrectRegiXData)]
        public IActionResult GetPermitRegixData([FromQuery] int applicationId)
        {
            RegixChecksWrapperDTO<CommercialFishingRegixDataDTO> data = service.GetPermitRegixData(applicationId);
            return Ok(data);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CommercialFishingPermitLicenseApplicationsInspectAndCorrectRegiXData)]
        public IActionResult GetPermitLicenseRegixData([FromQuery] int applicationId)
        {
            RegixChecksWrapperDTO<CommercialFishingRegixDataDTO> data = service.GetPermitLicenseRegixData(applicationId);
            return Ok(data);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CommercialFishingPermitLicenseApplicationsAddRecords)]
        public IActionResult GetPermitLicensesForRenewal([FromQuery] int permitId, [FromQuery] PageCodeEnum pageCode)
        {
            if (PERMIT_LICENSES_PAGE_CODES.Contains(pageCode))
            {
                List<PermitLicenseForRenewalDTO> permitLicenses = service.GetPermitLicensesForRenewal(permitId, pageCode);
                return Ok(permitLicenses);
            }
            else
            {
                return BadRequest("Invalid pageCode");
            }
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CommercialFishingPermitLicenseApplicationsAddRecords)]
        public IActionResult GetPermitLicenseData([FromQuery] int permitLicenseId)
        {
            CommercialFishingApplicationEditDTO permitLicense = service.GetPermitLicenseForRenewal(permitLicenseId);
            return Ok(permitLicense);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.CommercialFishingPermitApplicationsEditRecords, Permissions.CommercialFishingPermitLicenseApplicationsEditRecords)]
        public IActionResult AssignApplicationViaAccessCode([FromQuery] string accessCode)
        {
            try
            {
                IEnumerable<PageCodeEnum> pageCodes = GetAllowedPageCodes();
                AssignedApplicationInfoDTO applicationData = applicationService.AssignApplicationViaAccessCode(accessCode, CurrentUser.ID, pageCodes.ToArray());
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

        [HttpPost]
        [CustomAuthorize(Permissions.CommercialFishingPermitApplicationsAddRecords)]
        public async Task<IActionResult> AddPermitApplication([FromForm] CommercialFishingApplicationEditDTO permit)
        {
            QualifiedFisherBasicDataDTO qualifiedFisher = new QualifiedFisherBasicDataDTO
            {
                Id = permit.QualifiedFisherId,
                Identifier = permit.QualifiedFisherIdentifier,
                FirstName = permit.QualifiedFisherFirstName,
                MiddleName = permit.QualifiedFisherMiddleName,
                LastName = permit.QualifiedFisherLastName
            };
            List<CommercialFishingValidationErrorsEnum> validationErrors = await service.ValidateApplicationData(permit.PageCode!.Value,
                                                                                                                 permit.SubmittedFor,
                                                                                                                 permit.SubmittedBy,
                                                                                                                 qualifiedFisher,
                                                                                                                 permit.ShipId!.Value,
                                                                                                                 permit.DeliveryData.DeliveryTypeId,
                                                                                                                 permit.WaterTypeId!.Value,
                                                                                                                 permit.IsHolderShipOwner);

            if (validationErrors.Count != 0)
            {
                return ValidationFailedResult(validationErrors.Select(x => x.ToString()).ToList(), errorCode: ErrorCode.InvalidData);
            }

            int id = service.AddPermitApplication(permit, ApplicationStatusesEnum.EXT_CHK_STARTED);
            ForceRefreshShipsNomenclature();

            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.CommercialFishingPermitApplicationsAddRecords, Permissions.CommercialFishingPermitLicenseApplicationsAddRecords)]
        public async Task<IActionResult> AddPermitApplicationAndStartPermitLicenseApplication([FromForm] CommercialFishingApplicationEditDTO permit)
        {
            QualifiedFisherBasicDataDTO qualifiedFisher = new QualifiedFisherBasicDataDTO
            {
                Id = permit.QualifiedFisherId,
                Identifier = permit.QualifiedFisherIdentifier,
                FirstName = permit.QualifiedFisherFirstName,
                MiddleName = permit.QualifiedFisherMiddleName,
                LastName = permit.QualifiedFisherLastName
            };
            List<CommercialFishingValidationErrorsEnum> validationErrors = await service.ValidateApplicationData(permit.PageCode!.Value,
                                                                                                                 permit.SubmittedFor,
                                                                                                                 permit.SubmittedBy,
                                                                                                                 qualifiedFisher,
                                                                                                                 permit.ShipId!.Value,
                                                                                                                 permit.DeliveryData.DeliveryTypeId,
                                                                                                                 permit.WaterTypeId!.Value,
                                                                                                                 permit.IsHolderShipOwner);

            if (validationErrors.Count != 0)
            {
                return ValidationFailedResult(validationErrors.Select(x => x.ToString()).ToList(), errorCode: ErrorCode.InvalidData);
            }

            CommercialFishingApplicationEditDTO permitLicense = service.AddPermitApplicationAndStartPermitLicenseApplication(permit, 
                                                                                                                             CurrentUser.ID, 
                                                                                                                             ApplicationStatusesEnum.EXT_CHK_STARTED);
            ForceRefreshShipsNomenclature();

            return Ok(permitLicense);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.CommercialFishingPermitLicenseApplicationsAddRecords)]
        [RequestFormLimits(ValueCountLimit = 5000)]
        public async Task<IActionResult> AddPermitLicenseApplication([FromForm] CommercialFishingApplicationEditDTO permitLicense)
        {
            QualifiedFisherBasicDataDTO qualifiedFisher = new QualifiedFisherBasicDataDTO
            {
                Id = permitLicense.QualifiedFisherId,
                Identifier = permitLicense.QualifiedFisherIdentifier,
                FirstName = permitLicense.QualifiedFisherFirstName,
                MiddleName = permitLicense.QualifiedFisherMiddleName,
                LastName = permitLicense.QualifiedFisherLastName
            };
            List<CommercialFishingValidationErrorsEnum> validationErrors = await service.ValidateApplicationData(permitLicense.PageCode!.Value,
                                                                                                                 permitLicense.SubmittedFor,
                                                                                                                 permitLicense.SubmittedBy,
                                                                                                                 qualifiedFisher,
                                                                                                                 permitLicense.ShipId!.Value,
                                                                                                                 permitLicense.DeliveryData.DeliveryTypeId,
                                                                                                                 permitLicense.WaterTypeId!.Value,
                                                                                                                 permitLicense.IsHolderShipOwner!.Value);

            if (validationErrors.Count != 0)
            {
                return ValidationFailedResult(validationErrors.Select(x => x.ToString()).ToList(), errorCode: ErrorCode.InvalidData);
            }

            int id = service.AddPermitLicenseApplication(permitLicense, false, ApplicationStatusesEnum.EXT_CHK_STARTED);
            ForceRefreshShipsNomenclature();

            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.CommercialFishingPermitApplicationsEditRecords)]
        public async Task<IActionResult> EditPermitApplication([FromQuery] bool saveAsDraft, [FromForm] CommercialFishingApplicationEditDTO permit)
        {
            QualifiedFisherBasicDataDTO qualifiedFisher = new QualifiedFisherBasicDataDTO
            {
                Id = permit.QualifiedFisherId,
                Identifier = permit.QualifiedFisherIdentifier,
                FirstName = permit.QualifiedFisherFirstName,
                MiddleName = permit.QualifiedFisherMiddleName,
                LastName = permit.QualifiedFisherLastName
            };
            List<CommercialFishingValidationErrorsEnum> validationErrors = await service.ValidateApplicationData(permit.PageCode!.Value,
                                                                                                                 permit.SubmittedFor,
                                                                                                                 permit.SubmittedBy,
                                                                                                                 qualifiedFisher,
                                                                                                                 permit.ShipId!.Value,
                                                                                                                 permit.DeliveryData.DeliveryTypeId,
                                                                                                                 permit.WaterTypeId!.Value,
                                                                                                                 permit.IsHolderShipOwner);

            if (validationErrors.Count != 0)
            {
                return ValidationFailedResult(validationErrors.Select(x => x.ToString()).ToList(), errorCode: ErrorCode.InvalidData);
            }

            if (saveAsDraft)
            {
                service.EditPermitApplication(permit);
            }
            else
            {
                service.EditPermitApplication(permit, ApplicationStatusesEnum.EXT_CHK_STARTED);
            }

            ForceRefreshShipsNomenclature();

            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.CommercialFishingPermitLicenseApplicationsEditRecords)]
        [RequestFormLimits(ValueCountLimit = 5000)]
        public async Task<IActionResult> EditPermitLicenseApplication([FromQuery] bool saveAsDraft, [FromForm] CommercialFishingApplicationEditDTO permitLicense)
        {
            QualifiedFisherBasicDataDTO qualifiedFisher = new QualifiedFisherBasicDataDTO
            {
                Id = permitLicense.QualifiedFisherId,
                Identifier = permitLicense.QualifiedFisherIdentifier,
                FirstName = permitLicense.QualifiedFisherFirstName,
                MiddleName = permitLicense.QualifiedFisherMiddleName,
                LastName = permitLicense.QualifiedFisherLastName
            };
            List<CommercialFishingValidationErrorsEnum> validationErrors = await service.ValidateApplicationData(permitLicense.PageCode!.Value,
                                                                                                                 permitLicense.SubmittedFor,
                                                                                                                 permitLicense.SubmittedBy,
                                                                                                                 qualifiedFisher,
                                                                                                                 permitLicense.ShipId!.Value,
                                                                                                                 permitLicense.DeliveryData.DeliveryTypeId,
                                                                                                                 permitLicense.WaterTypeId!.Value,
                                                                                                                 permitLicense.IsHolderShipOwner!.Value);

            if (validationErrors.Count != 0)
            {
                return ValidationFailedResult(validationErrors.Select(x => x.ToString()).ToList(), errorCode: ErrorCode.InvalidData);
            }

            if (saveAsDraft)
            {
                service.EditPermitLicenseApplication(permitLicense);
            }
            else
            {
                service.EditPermitLicenseApplication(permitLicense, false, ApplicationStatusesEnum.EXT_CHK_STARTED);
            }

            ForceRefreshShipsNomenclature();

            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.CommercialFishingPermitApplicationsInspectAndCorrectRegiXData)]
        public IActionResult EditPermitApplicationAndStartRegixChecks([FromForm] CommercialFishingRegixDataDTO permit)
        {
            List<CommercialFishingValidationErrorsEnum> validationErrors = service.ValidateApplicationRegiXData(permit.PageCode!.Value,
                                                                                                                permit.SubmittedFor,
                                                                                                                permit.SubmittedBy,
                                                                                                                permit.Id!.Value,
                                                                                                                true);
            if (validationErrors.Count != 0)
            {
                return ValidationFailedResult(validationErrors.Select(x => x.ToString()).ToList(), errorCode: ErrorCode.InvalidData);
            }

            service.EditPermitApplicationRegixData(permit);
            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.CommercialFishingPermitLicenseApplicationsInspectAndCorrectRegiXData)]
        public IActionResult EditPermitLicenseApplicationAndStartRegixChecks([FromForm] CommercialFishingRegixDataDTO permitLicense)
        {
            List<CommercialFishingValidationErrorsEnum> validationErrors = service.ValidateApplicationRegiXData(permitLicense.PageCode!.Value,
                                                                                                                permitLicense.SubmittedFor,
                                                                                                                permitLicense.SubmittedBy,
                                                                                                                permitLicense.Id!.Value,
                                                                                                                false);
            if (validationErrors.Count != 0)
            {
                return ValidationFailedResult(validationErrors.Select(x => x.ToString()).ToList(), errorCode: ErrorCode.InvalidData);
            }

            service.EditPermitLicenseApplicationRegixData(permitLicense);
            return Ok();
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.CommercialFishingPermitApplicationsDeleteRecords, Permissions.CommercialFishingPermitLicenseApplicationsDeleteRecords)]
        public IActionResult DeleteApplication([FromQuery] int id)
        {
            applicationsRegisterService.DeleteApplication(id);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.CommercialFishingPermitApplicationsRestoreRecords, Permissions.CommercialFishingPermitLicenseApplicationsRestoreRecords)]
        public IActionResult UndoDeleteApplication([FromQuery] int id)
        {
            applicationsRegisterService.UndoDeleteApplication(id);
            return Ok();
        }

        // Nomenclatures

        [HttpGet]
        [CustomAuthorize(Permissions.CommercialFishingPermitRegisterReadAll, Permissions.CommercialFishingPermitRegisterRead)]
        public IActionResult GetCommercialFishingPermitTypes()
        {
            List<NomenclatureDTO> types = nomenclaturesService.GetCommercialFishingPermitTypes();
            return Ok(types);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CommercialFishingPermitLicenseRegisterReadAll, Permissions.CommercialFishingPermitLicenseRegisterRead)]
        public IActionResult GetCommercialFishingPermitLicenseTypes()
        {
            List<NomenclatureDTO> types = nomenclaturesService.GetCommercialFishingPermitLicenseTypes();
            return Ok(types);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CommercialFishingPermitApplicationsReadAll,
                         Permissions.CommercialFishingPermitApplicationsRead,
                         Permissions.CommercialFishingPermitLicenseApplicationsReadAll,
                         Permissions.CommercialFishingPermitLicenseApplicationsRead)]
        public IActionResult GetApplicationStatuses()
        {
            List<NomenclatureDTO> statuses = applicationsRegisterService.GetApplicationStatuses(ApplicationHierarchyTypesEnum.Online, 
                                                                                                ApplicationHierarchyTypesEnum.OnPaper);
            return Ok(statuses);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CommercialFishingPermitApplicationsReadAll,
                         Permissions.CommercialFishingPermitApplicationsRead,
                         Permissions.CommercialFishingPermitLicenseApplicationsReadAll,
                         Permissions.CommercialFishingPermitLicenseApplicationsRead)]
        public IActionResult GetApplicationTypes()
        {
            IEnumerable<PageCodeEnum> pageCodes = GetAllowedPageCodes();
            List<NomenclatureDTO> types = applicationsRegisterService.GetApplicationTypes(pageCodes.ToArray());
            return Ok(types);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CommercialFishingPermitApplicationsReadAll,
                         Permissions.CommercialFishingPermitApplicationsRead,
                         Permissions.CommercialFishingPermitLicenseApplicationsReadAll,
                         Permissions.CommercialFishingPermitLicenseApplicationsRead)]
        public IActionResult GetApplicationSources()
        {
            List<NomenclatureDTO> sources = applicationsRegisterService.GetApplicationSources(ApplicationHierarchyTypesEnum.Online, 
                                                                                              ApplicationHierarchyTypesEnum.OnPaper);
            return Ok(sources);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CommercialFishingPermitRegisterReadAll,
                         Permissions.CommercialFishingPermitRegisterRead,
                         Permissions.CommercialFishingPermitLicenseRegisterReadAll,
                         Permissions.CommercialFishingPermitLicenseRegisterRead)]
        public IActionResult GetQualifiedFishers()
        {
            List<QualifiedFisherNomenclatureDTO> qualifiedFishers = nomenclaturesService.GetQualifiedFishers();
            return Ok(qualifiedFishers);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CommercialFishingPermitRegisterReadAll,
                         Permissions.CommercialFishingPermitRegisterRead,
                         Permissions.CommercialFishingPermitApplicationsReadAll,
                         Permissions.CommercialFishingPermitApplicationsRead,
                         Permissions.CommercialFishingPermitLicenseRegisterReadAll,
                         Permissions.CommercialFishingPermitLicenseRegisterRead,
                         Permissions.CommercialFishingPermitLicenseApplicationsReadAll,
                         Permissions.CommercialFishingPermitLicenseApplicationsRead)]
        public IActionResult GetWaterTypes()
        {
            List<NomenclatureDTO> waterTypes = nomenclaturesService.GetWaterTypes();
            return Ok(waterTypes);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CommercialFishingPermitRegisterReadAll,
                         Permissions.CommercialFishingPermitRegisterRead,
                         Permissions.CommercialFishingPermitApplicationsReadAll,
                         Permissions.CommercialFishingPermitApplicationsRead,
                         Permissions.CommercialFishingPermitLicenseRegisterReadAll,
                         Permissions.CommercialFishingPermitLicenseRegisterRead,
                         Permissions.CommercialFishingPermitLicenseApplicationsReadAll,
                         Permissions.CommercialFishingPermitLicenseApplicationsRead)]
        public IActionResult GetHolderGroundForUseTypes()
        {
            List<NomenclatureDTO> groundForUseTypes = nomenclaturesService.GetHolderGroundForUseTypes();
            return Ok(groundForUseTypes);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CommercialFishingPermitRegisterReadAll,
                         Permissions.CommercialFishingPermitRegisterRead,
                         Permissions.CommercialFishingPermitApplicationsReadAll,
                         Permissions.CommercialFishingPermitApplicationsRead,
                         Permissions.CommercialFishingPermitLicenseRegisterReadAll,
                         Permissions.CommercialFishingPermitLicenseRegisterRead,
                         Permissions.CommercialFishingPermitLicenseApplicationsReadAll,
                         Permissions.CommercialFishingPermitLicenseApplicationsRead)]
        public IActionResult GetPoundNets()
        {
            List<PoundNetNomenclatureDTO> poundNets = nomenclaturesService.GetPoundNets();
            return Ok(poundNets);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CommercialFishingPermitLicenseRegisterReadAll,
                         Permissions.CommercialFishingPermitLicenseRegisterRead,
                         Permissions.CommercialFishingPermitLicenseApplicationsReadAll,
                         Permissions.CommercialFishingPermitLicenseApplicationsRead)]
        public IActionResult GetPorts()
        {
            List<NomenclatureDTO> ports = shipNomenclaturesService.GetPorts();
            return Ok(ports);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CommercialFishingPermitLicenseRegisterReadAll,
                         Permissions.CommercialFishingPermitLicenseRegisterRead,
                         Permissions.CommercialFishingPermitRegisterReadAll,
                         Permissions.CommercialFishingPermitRegisterRead)]
        public IActionResult GetSuspensionTypes()
        {
            List<SuspensionTypeNomenclatureDTO> suspensionTypes = nomenclaturesService.GetSuspensionTypes();
            return Ok(suspensionTypes);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CommercialFishingPermitLicenseRegisterReadAll,
                         Permissions.CommercialFishingPermitLicenseRegisterRead,
                         Permissions.CommercialFishingPermitRegisterReadAll,
                         Permissions.CommercialFishingPermitRegisterRead)]
        public IActionResult GetSuspensionReasons()
        {
            List<SuspensionReasonNomenclatureDTO> suspensionReasons = nomenclaturesService.GetSuspensionReasons();
            return Ok(suspensionReasons);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CommercialFishingPermitLicenseApplicationsReadAll, Permissions.CommercialFishingPermitLicenseApplicationsRead)]
        public IActionResult GetShipPermits([FromQuery] int shipId, [FromQuery] bool onlyPoundNet)
        {
            List<PermitNomenclatureDTO> permits = nomenclaturesService.GetShipPermits(shipId, onlyPoundNet);
            return Ok(permits);
        }

        private IEnumerable<PageCodeEnum> GetAllowedPageCodes()
        {
            IEnumerable<PageCodeEnum> pageCodes = null;
            if (CurrentUser.Permissions.Contains(Permissions.CommercialFishingPermitApplicationsRead)
                || CurrentUser.Permissions.Contains(Permissions.CommercialFishingPermitApplicationsReadAll))
            {
                pageCodes = PERMITS_PAGE_CODES.ToList();
            }

            if (CurrentUser.Permissions.Contains(Permissions.CommercialFishingPermitLicenseApplicationsRead)
                || CurrentUser.Permissions.Contains(Permissions.CommercialFishingPermitLicenseApplicationsReadAll))
            {
                if (pageCodes == null)
                {
                    pageCodes = PERMIT_LICENSES_PAGE_CODES.ToList();
                }
                else
                {
                    pageCodes = pageCodes.Concat(PERMIT_LICENSES_PAGE_CODES).ToList();
                }
            }

            return pageCodes;
        }

        private void ForceRefreshShipsNomenclature()
        {
            memoryCacheService.ForceRefresh<ICommonNomenclaturesService, List<ShipNomenclatureDTO>>("GetShips", 
                                                                                                    (service) => service.GetShips().ToList());
        }
    }

    public class PermitLicenseData
    {
        public CommercialFishingRegisterFilters Filters { get; set; }

        public List<int> PermitIds { get; set; }
    }
}
