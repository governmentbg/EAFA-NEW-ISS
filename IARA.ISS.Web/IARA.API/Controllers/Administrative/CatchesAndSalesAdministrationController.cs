using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using IARA.Common.Enums;
using IARA.Common.Exceptions;
using IARA.DomainModels.DTOModels.CatchesAndSales;
using IARA.DomainModels.DTOModels.CommercialFishingRegister;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
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
    public class CatchesAndSalesAdministrationController : BaseAuditController
    {
        private readonly ILogBooksService service;
        private readonly ILogBookNomenclaturesService nomenclaturesService;
        private readonly IUserService userService;
        private readonly IAquacultureFacilitiesNomenclaturesService aquacultureNomenclatureService;

        public CatchesAndSalesAdministrationController(IPermissionsService permissionsService,
                                                       ILogBooksService service,
                                                       ILogBookNomenclaturesService nomenclaturesService,
                                                       IUserService userService,
                                                       IAquacultureFacilitiesNomenclaturesService aquacultureNomenclatureService)
            : base(permissionsService)
        {
            this.service = service;
            this.nomenclaturesService = nomenclaturesService;
            this.userService = userService;
            this.aquacultureNomenclatureService = aquacultureNomenclatureService;
        }

        [HttpPost]
        [CustomAuthorize(Permissions.FishLogBooksReadAll,
                         Permissions.FishLogBookRead,
                         Permissions.FirstSaleLogBooksReadAll,
                         Permissions.FirstSaleLogBookRead,
                         Permissions.AdmissionLogBooksReadAll,
                         Permissions.AdmissionLogBookRead,
                         Permissions.TransportationLogBooksReadAll,
                         Permissions.TransportationLogBookRead,
                         Permissions.AquacultureLogBooksReadAll,
                         Permissions.AquacultureLogBookRead)]
        public IActionResult GetAllLogBooks([FromBody] GridRequestModel<CatchesAndSalesAdministrationFilters> request)
        {
            if (!CurrentUser.Permissions.Contains(Permissions.FishLogBooksReadAll)
                || !CurrentUser.Permissions.Contains(Permissions.FirstSaleLogBooksReadAll)
                || !CurrentUser.Permissions.Contains(Permissions.AdmissionLogBooksReadAll)
                || !CurrentUser.Permissions.Contains(Permissions.TransportationLogBooksReadAll)
                || !CurrentUser.Permissions.Contains(Permissions.AquacultureLogBooksReadAll))
            {
                int? territoryUnitId = userService.GetUserTerritoryUnitId(CurrentUser.ID);

                if (territoryUnitId.HasValue)
                {
                    if (request.Filters == null)
                    {
                        request.Filters = new CatchesAndSalesAdministrationFilters
                        {
                            ShowInactiveRecords = false
                        };
                    }

                    request.Filters.TerritoryUnitId = territoryUnitId.Value;

                    if (!CurrentUser.Permissions.Contains(Permissions.FishLogBooksReadAll))
                    {
                        request.Filters.FilterFishLogBookTeritorryUnitId = true;
                    }

                    if (!CurrentUser.Permissions.Contains(Permissions.FirstSaleLogBooksReadAll))
                    {
                        request.Filters.FilterFirstSaleLogBookTeritorryUnitId = true;
                    }

                    if (!CurrentUser.Permissions.Contains(Permissions.AdmissionLogBooksReadAll))
                    {
                        request.Filters.FilterAdmissionLogBookTeritorryUnitId = true;
                    }

                    if (!CurrentUser.Permissions.Contains(Permissions.TransportationLogBooksReadAll))
                    {
                        request.Filters.FilterTransportationLogBookTeritorryUnitId = true;
                    }

                    if (!CurrentUser.Permissions.Contains(Permissions.AquacultureLogBooksReadAll))
                    {
                        request.Filters.FilterAquacultureLogBookTeritorryUnitId = true;
                    }
                }
                else
                {
                    return Ok(Enumerable.Empty<LogBookRegisterDTO>());
                }
            }

            List<LogBookTypesEnum> permittedLogBookTypes = GetPermittedLogBookTypes();
            IQueryable<LogBookRegisterDTO> logBooks = service.GetAllLogBooks(request.Filters, permittedLogBookTypes);
            return PageResult(logBooks, request, false);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.FishLogBookPageReadAll,
                         Permissions.FishLogBookPageRead,
                         Permissions.FirstSaleLogBookPageReadAll,
                         Permissions.FirstSaleLogBookPageRead,
                         Permissions.AdmissionLogBookPageReadAll,
                         Permissions.AdmissionLogBookPageRead,
                         Permissions.TransportationLogBookPageReadAll,
                         Permissions.TransportationLogBookPageRead,
                         Permissions.AquacultureLogBookPageReadAll,
                         Permissions.AquacultureLogBookPageRead)]
        public IActionResult GetLogBookPagesForTable([FromBody] LogBookAdministrationData logBookData)
        {
            List<LogBookTypesEnum> permittedLogBookTypes = GetPermittedLogBookTypesPages();
            LogBookPagesDTO logBookPages = service.GetLogBookPagesForTable(logBookData.LogBookIds, logBookData.Filters, permittedLogBookTypes);
            return Ok(logBookPages);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FishLogBookRead, Permissions.FishLogBooksReadAll)]
        public IActionResult GetCommercialFishingLogBook([FromQuery] int id)
        {
            CommercialFishingLogBookEditDTO result = service.GetCommercialFishingLogBook(id);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FirstSaleLogBookPageReadAll,
                         Permissions.FirstSaleLogBookPageRead,
                         Permissions.AdmissionLogBookPageReadAll,
                         Permissions.AdmissionLogBookPageRead,
                         Permissions.TransportationLogBookPageReadAll,
                         Permissions.TransportationLogBookPageRead,
                         Permissions.AquacultureLogBookPageReadAll,
                         Permissions.AquacultureLogBookPageRead)]
        public IActionResult GetLogBook([FromQuery] int id)
        {
            LogBookEditDTO result = service.GetLogBook(id);
            return Ok(result);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.FirstSaleLogBookPageAdd, Permissions.AdmissionLogBookPageAdd, Permissions.TransportationLogBookPageAdd)]
        public IActionResult GetLogBookPageDocumentData([FromBody] BasicLogBookPageDocumentParameters parameters)
        {
            try
            {
                BasicLogBookPageDocumentDataDTO result = service.GetLogBookPageDocumentData(parameters);
                return Ok(result);
            }
            catch (RecordNotFoundException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, errorCode: ErrorCode.LogBookNotFound);
            }
            catch (PageNumberNotInLogbookException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, errorCode: ErrorCode.PageNumberNotInLogbook);
            }
            catch (PageNumberNotInLogBookLicenseException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, errorCode: ErrorCode.PageNumberNotInLogBookLicense);
            }
            catch (LogBookPageAlreadySubmittedException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, errorCode: ErrorCode.LogBookPageAlreadySubmitted);
            }
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FirstSaleLogBookPageAdd, Permissions.AdmissionLogBookPageAdd, Permissions.TransportationLogBookPageAdd)]
        public IActionResult GetLogBookPageDocumentOwnerData([FromQuery] decimal documentNumber, [FromQuery] LogBookPageDocumentTypesEnum documentType)
        {
            List<LogBookNomenclatureDTO> result = service.GetLogBookPageDocumentOwnerData(documentNumber, documentType);
            return Ok(result);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.FirstSaleLogBookPageAdd, Permissions.AdmissionLogBookPageAdd, Permissions.TransportationLogBookPageAdd)]
        public IActionResult GetCommonLogBookPageData([FromBody] CommonLogBookPageDataParameters parameters)
        {
            LogBookPageStatusesEnum? pageStatus = service.CheckDocumentPageToAddStatus(parameters.PageNumberToAdd!.Value, parameters.LogBookType!.Value);
            if (!service.CheckDocumentPageToAddExistance(parameters.PageNumberToAdd!.Value, parameters.LogBookId!.Value, parameters.LogBookType!.Value))
            {
                return ValidationFailedResult(new List<string> { parameters.PageNumberToAdd.ToString() }, ErrorCode.PageNumberNotInLogbook);
            }
            else if (pageStatus != null && pageStatus != LogBookPageStatusesEnum.Missing)
            {
                return ValidationFailedResult(new List<string> { parameters.PageNumberToAdd.ToString() }, ErrorCode.LogBookPageAlreadySubmitted);
            }
            if (!string.IsNullOrEmpty(parameters.OriginDeclarationNumber)
                && !service.CheckOriginDeclarationExistance(parameters.OriginDeclarationNumber))
            {
                return ValidationFailedResult(new List<string> { parameters.OriginDeclarationNumber }, ErrorCode.InvalidOriginDeclarationNumber);
            }
            else if (parameters.TransportationDocumentNumber.HasValue
                && !service.CheckTransportationDocumentExistance(parameters.TransportationDocumentNumber.Value))
            {
                return ValidationFailedResult(new List<string> { parameters.TransportationDocumentNumber.Value.ToString() }, ErrorCode.InvalidTransportationDocNumber);
            }
            else if (parameters.AdmissionDocumentNumber.HasValue
                && !service.CheckAdmissionDocumentExistance(parameters.AdmissionDocumentNumber.Value))
            {
                return ValidationFailedResult(new List<string> { parameters.AdmissionDocumentNumber.Value.ToString() }, ErrorCode.InvalidAdmissionDocNumber);
            }
            else
            {
                CommonLogBookPageDataDTO commonLogBookPageData = service.GetCommonLogBookPageDataDTO(parameters);
                return Ok(commonLogBookPageData);
            }
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FishLogBookPageAdd, Permissions.FishLogBookPageEdit)]
        public IActionResult GetPreviousTripsOnBoardCatchRecords([FromQuery] int shipId)
        {
            List<OnBoardCatchRecordFishDTO> results = service.GetPreviousTripsOnBoardCatchRecords(shipId);
            return Ok(results);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FirstSaleLogBookPageAdd, Permissions.AdmissionLogBookPageAdd, Permissions.TransportationLogBookPageAdd)]
        public IActionResult GetPossibleProducts([FromQuery] int shipLogBookPageId)
        {
            List<LogBookPageProductDTO> results = service.GetPossibleProducts(shipLogBookPageId);
            return Ok(results);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FishLogBookPageReadAll, Permissions.FishLogBookPageRead)]
        public IActionResult GetShipLogBookPage([FromQuery] int id)
        {
            ShipLogBookPageEditDTO page = service.GetShipLogBookPage(id);
            return Ok(page);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FishLogBookPageAdd)]
        public IActionResult GetNewShipLogBookPages([FromQuery] long pageNumber, [FromQuery] int logBookId)
        {
            try
            {
                List<ShipLogBookPageEditDTO> pages = service.GetNewShipLogBookPages(pageNumber, logBookId);
                return Ok(pages);
            }
            catch (PageNumberNotInLogbookException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, errorCode: ErrorCode.PageNumberNotInLogbook);
            }
            catch (PageNumberNotInLogBookLicenseException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, errorCode: ErrorCode.PageNumberNotInLogBookLicense);
            }
            catch (LogBookPageAlreadySubmittedException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, errorCode: ErrorCode.LogBookPageAlreadySubmitted);
            }
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FirstSaleLogBookPageReadAll, Permissions.FirstSaleLogBookPageRead)]
        public IActionResult GetFirstSaleLogBookPage([FromQuery] int id)
        {
            FirstSaleLogBookPageEditDTO page = service.GetFirstSaleLogBookPage(id);
            return Ok(page);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FirstSaleLogBookPageAdd)]
        public IActionResult GetNewFirstSaleLogBookPage([FromQuery] int logBookId,
                                                        [FromQuery] int? originDeclarationId,
                                                        [FromQuery] int? transportationDocumentId,
                                                        [FromQuery] int? admissionDocumentId)
        {
            FirstSaleLogBookPageEditDTO page = service.GetNewFirstSaleLogBookPage(logBookId, originDeclarationId, transportationDocumentId, admissionDocumentId);
            return Ok(page);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AdmissionLogBookPageReadAll, Permissions.AdmissionLogBookPageRead)]
        public IActionResult GetAdmissionLogBookPage([FromQuery] int id)
        {
            AdmissionLogBookPageEditDTO page = service.GetAdmissionLogBookPage(id);
            return Ok(page);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AdmissionLogBookPageAdd)]
        public IActionResult GetNewAdmissionLogBookPage([FromQuery] int logBookId, [FromQuery] int? originDeclarationId, [FromQuery] int? transportationDocumentId)
        {
            AdmissionLogBookPageEditDTO page = service.GetNewAdmissionLogBookPage(logBookId, originDeclarationId, transportationDocumentId);
            return Ok(page);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TransportationLogBookPageReadAll, Permissions.TransportationLogBookPageRead)]
        public IActionResult GetTransportationLogBookPage([FromQuery] int id)
        {
            TransportationLogBookPageEditDTO page = service.GetTransportationLogBookPage(id);
            return Ok(page);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TransportationLogBookPageAdd)]
        public IActionResult GetNewTransportationLogBookPage([FromQuery] int logBookId, [FromQuery] int? originDeclarationId)
        {
            TransportationLogBookPageEditDTO page = service.GetNewTransportationLogBookPage(logBookId, originDeclarationId);
            return Ok(page);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureLogBookPageReadAll, Permissions.AquacultureLogBookPageRead)]
        public IActionResult GetAquacultureLogBookPage([FromQuery] int id)
        {
            AquacultureLogBookPageEditDTO page = service.GetAquacultureLogBookPage(id);
            return Ok(page);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureLogBookPageReadAll, Permissions.AquacultureLogBookPageRead)]
        public IActionResult GetNewAquacultureLogBookPage([FromQuery] int logBookId)
        {
            AquacultureLogBookPageEditDTO page = service.GetNewAquacultureLogBookPage(logBookId);
            return Ok(page);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.FishLogBookPageAdd)]
        public IActionResult AddShipLogBookPage([FromForm] ShipLogBookPageEditDTO page)
        {
            try
            {
                int id = service.AddShipLogBookPage(page);
                return Ok(id);
            }
            catch (PageNumberNotInLogbookException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, errorCode: ErrorCode.PageNumberNotInLogbook);
            }
            catch (PageNumberNotInLogBookLicenseException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, errorCode: ErrorCode.PageNumberNotInLogBookLicense);
            }
            catch (LogBookPageAlreadySubmittedException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, errorCode: ErrorCode.LogBookPageAlreadySubmitted);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.FishLogBookPageEdit)]
        public IActionResult EditShipLogBookPage([FromForm] ShipLogBookPageEditDTO page)
        {
            service.EditShipLogBookPage(page);
            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.FirstSaleLogBookPageAdd)]
        public IActionResult AddFirstSaleLogBookPage([FromForm] FirstSaleLogBookPageEditDTO page)
        {
            try
            {
                return Ok(service.AddFirstSaleLogBookPage(page));
            }
            catch (PageNumberNotInLogbookException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, errorCode: ErrorCode.PageNumberNotInLogbook);
            }
            catch (LogBookPageAlreadySubmittedException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, errorCode: ErrorCode.LogBookPageAlreadySubmitted);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.FirstSaleLogBookPageEdit)]
        public IActionResult EditFirstSaleLogBookPage([FromForm] FirstSaleLogBookPageEditDTO page)
        {
            service.EditFirstSaleLogBookPage(page);
            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.AdmissionLogBookPageAdd)]
        public IActionResult AddAdmissionLogBookPage([FromForm] AdmissionLogBookPageEditDTO page)
        {
            try
            {
                return Ok(service.AddAdmissionLogBookPage(page));
            }
            catch (PageNumberNotInLogbookException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, errorCode: ErrorCode.PageNumberNotInLogbook);
            }
            catch (PageNumberNotInLogBookLicenseException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, errorCode: ErrorCode.PageNumberNotInLogBookLicense);
            }
            catch (LogBookPageAlreadySubmittedException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, errorCode: ErrorCode.LogBookPageAlreadySubmitted);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.AdmissionLogBookPageEdit)]
        public IActionResult EditAdmissionLogBookPage([FromForm] AdmissionLogBookPageEditDTO page)
        {
            service.EditAdmissionLogBookPage(page);
            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.TransportationLogBookPageAdd)]
        public IActionResult AddTransportationLogBookPage([FromForm] TransportationLogBookPageEditDTO page)
        {
            try
            {
                return Ok(service.AddTransportationLogBookPage(page));
            }
            catch (PageNumberNotInLogbookException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, errorCode: ErrorCode.PageNumberNotInLogbook);
            }
            catch (PageNumberNotInLogBookLicenseException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, errorCode: ErrorCode.PageNumberNotInLogBookLicense);
            }
            catch (LogBookPageAlreadySubmittedException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, errorCode: ErrorCode.LogBookPageAlreadySubmitted);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.TransportationLogBookPageEdit)]
        public IActionResult EditTransportationLogBookPage([FromForm] TransportationLogBookPageEditDTO page)
        {
            service.EditTransportationLogBookPage(page);
            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.AquacultureLogBookPageAdd)]
        public IActionResult AddAquacultureLogBookPage([FromForm] AquacultureLogBookPageEditDTO page)
        {
            try
            {
                return Ok(service.AddAquacultureLogBookPage(page));
            }
            catch (PageNumberNotInLogbookException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, errorCode: ErrorCode.PageNumberNotInLogbook);
            }
            catch (LogBookPageAlreadySubmittedException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, errorCode: ErrorCode.LogBookPageAlreadySubmitted);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.AquacultureLogBookPageEdit)]
        public IActionResult EditAquacultureLogBookPage([FromForm] AquacultureLogBookPageEditDTO page)
        {
            service.EditAquacultureLogBookPage(page);
            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.FishLogBookPageCancel,
                         Permissions.FirstSaleLogBookPageCancel,
                         Permissions.AdmissionLogBookPageCancel,
                         Permissions.TransportationLogBookPageCancel,
                         Permissions.AquacultureLogBookPageCancel)]
        public IActionResult AnnulLogBookPage([FromBody] LogBookPageCancellationReasonDTO reasonData)
        {
            service.AnnulLogBookPage(reasonData);
            return Ok();
        }

        //Nomenclatures
        [HttpGet]
        [CustomAuthorize(Permissions.FirstSaleLogBookPageReadAll,
                         Permissions.FirstSaleLogBookPageRead,
                         Permissions.AdmissionLogBookPageReadAll,
                         Permissions.AdmissionLogBookPageRead,
                         Permissions.TransportationLogBookPageReadAll,
                         Permissions.TransportationLogBookPageRead,
                         Permissions.AquacultureLogBookPageReadAll,
                         Permissions.AquacultureLogBookPageRead)]
        public IActionResult GetLogBookTypes()
        {
            return Ok(nomenclaturesService.GetLogBookTypes());
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FirstSaleLogBookPageReadAll,
                         Permissions.FirstSaleLogBookPageRead,
                         Permissions.AdmissionLogBookPageReadAll,
                         Permissions.AdmissionLogBookPageRead,
                         Permissions.TransportationLogBookPageReadAll,
                         Permissions.TransportationLogBookPageRead,
                         Permissions.AquacultureLogBookPageReadAll,
                         Permissions.AquacultureLogBookPageRead)]
        public IActionResult GetRegisteredBuyersNomenclature()
        {
            return Ok(nomenclaturesService.GetRegisteredBuyers());
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureLogBookPageReadAll,
                         Permissions.AquacultureLogBookPageRead)]
        public IActionResult GetAquacultureFacilitiesNomenclature()
        {
            return Ok(aquacultureNomenclatureService.GetAllAquacultureNomenclatures());
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FishLogBookPageReadAll, Permissions.FishLogBookPageRead)]
        public IActionResult GetPermitLicenseNomenclatures()
        {
            return Ok(nomenclaturesService.GetPermitLicenses());
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FirstSaleLogBookPageReadAll,
                         Permissions.FirstSaleLogBookPageRead,
                         Permissions.AdmissionLogBookPageReadAll,
                         Permissions.AdmissionLogBookPageRead,
                         Permissions.TransportationLogBookPageReadAll,
                         Permissions.TransportationLogBookPageRead,
                         Permissions.AquacultureLogBookPageReadAll,
                         Permissions.AquacultureLogBookPageRead)]
        public IActionResult GetTurbotSizeGroups()
        {
            return Ok(nomenclaturesService.GetTurbotSizeGroups());
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FirstSaleLogBookPageReadAll,
                         Permissions.FirstSaleLogBookPageRead,
                         Permissions.AdmissionLogBookPageReadAll,
                         Permissions.AdmissionLogBookPageRead,
                         Permissions.TransportationLogBookPageReadAll,
                         Permissions.TransportationLogBookPageRead,
                         Permissions.AquacultureLogBookPageReadAll,
                         Permissions.AquacultureLogBookPageRead)]
        public IActionResult GetFishSizeCategories()
        {
            return Ok(nomenclaturesService.GetFishSizeCategories());
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FirstSaleLogBookPageReadAll,
                         Permissions.FirstSaleLogBookPageRead,
                         Permissions.AdmissionLogBookPageReadAll,
                         Permissions.AdmissionLogBookPageRead,
                         Permissions.TransportationLogBookPageReadAll,
                         Permissions.TransportationLogBookPageRead,
                         Permissions.AquacultureLogBookPageReadAll,
                         Permissions.AquacultureLogBookPageRead,
                         Permissions.FishLogBookPageReadAll,
                         Permissions.FishLogBookPageRead)]
        public IActionResult GetCatchStates()
        {
            return Ok(nomenclaturesService.GetCatchStates());
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FirstSaleLogBookPageReadAll,
                         Permissions.FirstSaleLogBookPageRead,
                         Permissions.AdmissionLogBookPageReadAll,
                         Permissions.AdmissionLogBookPageRead,
                         Permissions.TransportationLogBookPageReadAll,
                         Permissions.TransportationLogBookPageRead,
                         Permissions.AquacultureLogBookPageReadAll,
                         Permissions.AquacultureLogBookPageRead,
                         Permissions.FishLogBookPageReadAll,
                         Permissions.FishLogBookPageRead)]
        public IActionResult GetUnloadTypes()
        {
            return Ok(nomenclaturesService.GetUnloadTypes());
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FishLogBookPageReadAll, Permissions.FishLogBookPageRead)]
        public IActionResult GetFishSizes()
        {
            return Ok(nomenclaturesService.GetFishSizes());
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FishLogBookPageReadAll, Permissions.FishLogBookPageRead)]
        public IActionResult GetCatchTypes()
        {
            return Ok(nomenclaturesService.GetCatchTypes());
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FirstSaleLogBookPageReadAll,
                         Permissions.FirstSaleLogBookPageRead,
                         Permissions.AdmissionLogBookPageReadAll,
                         Permissions.AdmissionLogBookPageRead,
                         Permissions.TransportationLogBookPageReadAll,
                         Permissions.TransportationLogBookPageRead,
                         Permissions.AquacultureLogBookPageReadAll,
                         Permissions.AquacultureLogBookPageRead)]
        public IActionResult GetFishPurposes()
        {
            return Ok(nomenclaturesService.GetFishPurposes());
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FishLogBookPageReadAll, Permissions.FishLogBookPageRead)]
        public IActionResult GetFishingGearsRegister([FromQuery] int permitLicenseId)
        {
            return Ok(nomenclaturesService.GetFishingGearsRegister(permitLicenseId));
        }

        // Simple audit
        [HttpGet]
        [CustomAuthorize(Permissions.FirstSaleLogBookPageReadAll,
                         Permissions.FirstSaleLogBookPageRead,
                         Permissions.AdmissionLogBookPageReadAll,
                         Permissions.AdmissionLogBookPageRead,
                         Permissions.TransportationLogBookPageReadAll,
                         Permissions.TransportationLogBookPageRead,
                         Permissions.AquacultureLogBookPageReadAll,
                         Permissions.AquacultureLogBookPageRead)]
        public override IActionResult GetAuditInfo([FromQuery] int id)
        {
            SimpleAuditDTO audit = service.GetSimpleAudit(id);
            return Ok(audit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FishLogBookPageReadAll, Permissions.FishLogBookPageRead)]
        public IActionResult GetShipLogBookPageSimpleAudit([FromQuery] int id)
        {
            SimpleAuditDTO audit = service.GetShipLogBookPageSimpleAudit(id);
            return Ok(audit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FishLogBookPageReadAll, Permissions.FishLogBookPageRead)]
        public IActionResult GetCatchRecordSimpleAudit([FromQuery] int id)
        {
            SimpleAuditDTO audit = service.GetCatchRecordSimpleAudit(id);
            return Ok(audit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FishLogBookPageReadAll, Permissions.FishLogBookPageRead)]
        public IActionResult GetOriginDeclarationFishSimpleAudit([FromQuery] int id)
        {
            SimpleAuditDTO audit = service.GetOriginDeclarationFishSimpleAudit(id);
            return Ok(audit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FirstSaleLogBookPageReadAll, Permissions.FirstSaleLogBookPageRead)]
        public IActionResult GetFirstSaleLogBookPageSimpleAudit([FromQuery] int id)
        {
            SimpleAuditDTO audit = service.GetFirstSaleLogBookPageSimpleAudit(id);
            return Ok(audit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FirstSaleLogBookPageReadAll,
                         Permissions.FirstSaleLogBookPageRead,
                         Permissions.AdmissionLogBookPageReadAll,
                         Permissions.AdmissionLogBookPageRead,
                         Permissions.TransportationLogBookPageReadAll,
                         Permissions.TransportationLogBookPageRead,
                         Permissions.AquacultureLogBookPageReadAll,
                         Permissions.AquacultureLogBookPageRead)]
        public IActionResult GetLogBookPageProductAudit([FromQuery] int id)
        {
            SimpleAuditDTO audit = service.GetLogBookPageProductAudit(id);
            return Ok(audit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AdmissionLogBookPageReadAll, Permissions.AdmissionLogBookPageRead)]
        public IActionResult GetAdmissionLogBookPageSimpleAudit([FromQuery] int id)
        {
            SimpleAuditDTO audit = service.GetAdmissionLogBookPageSimpleAudit(id);
            return Ok(audit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TransportationLogBookPageReadAll, Permissions.TransportationLogBookPageRead)]
        public IActionResult GetTransportationLogBookPageSimpleAudit([FromQuery] int id)
        {
            SimpleAuditDTO audit = service.GetTransportationLogBookPageSimpleAudit(id);
            return Ok(audit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.AquacultureLogBookPageReadAll, Permissions.AquacultureLogBookPageRead)]
        public IActionResult GetAquacultureLogBookPageSimpleAudit([FromQuery] int id)
        {
            SimpleAuditDTO audit = service.GetAquacultureLogBookPageSimpleAudit(id);
            return Ok(audit);
        }

        private List<LogBookTypesEnum> GetPermittedLogBookTypes()
        {
            List<LogBookTypesEnum> permittedLogBookTypes = new();

            if (CurrentUser.Permissions.Contains(Permissions.FishLogBookRead) || CurrentUser.Permissions.Contains(Permissions.FishLogBooksReadAll))
            {
                permittedLogBookTypes.Add(LogBookTypesEnum.Ship);
            }

            if (CurrentUser.Permissions.Contains(Permissions.FirstSaleLogBookRead) || CurrentUser.Permissions.Contains(Permissions.FirstSaleLogBooksReadAll))
            {
                permittedLogBookTypes.Add(LogBookTypesEnum.FirstSale);
            }

            if (CurrentUser.Permissions.Contains(Permissions.AdmissionLogBookRead) || CurrentUser.Permissions.Contains(Permissions.AdmissionLogBooksReadAll))
            {
                permittedLogBookTypes.Add(LogBookTypesEnum.Admission);
            }

            if (CurrentUser.Permissions.Contains(Permissions.TransportationLogBookRead) || CurrentUser.Permissions.Contains(Permissions.TransportationLogBooksReadAll))
            {
                permittedLogBookTypes.Add(LogBookTypesEnum.Transportation);
            }

            if (CurrentUser.Permissions.Contains(Permissions.AquacultureLogBookRead) || CurrentUser.Permissions.Contains(Permissions.AquacultureLogBooksReadAll))
            {
                permittedLogBookTypes.Add(LogBookTypesEnum.Aquaculture);
            }

            return permittedLogBookTypes;
        }

        private List<LogBookTypesEnum> GetPermittedLogBookTypesPages()
        {
            List<LogBookTypesEnum> permittedLogBookTypes = new();

            if (CurrentUser.Permissions.Contains(Permissions.FishLogBookPageRead) || CurrentUser.Permissions.Contains(Permissions.FishLogBookPageReadAll))
            {
                permittedLogBookTypes.Add(LogBookTypesEnum.Ship);
            }

            if (CurrentUser.Permissions.Contains(Permissions.FirstSaleLogBookPageRead) || CurrentUser.Permissions.Contains(Permissions.FirstSaleLogBookPageReadAll))
            {
                permittedLogBookTypes.Add(LogBookTypesEnum.FirstSale);
            }

            if (CurrentUser.Permissions.Contains(Permissions.AdmissionLogBookPageRead) || CurrentUser.Permissions.Contains(Permissions.AdmissionLogBookPageReadAll))
            {
                permittedLogBookTypes.Add(LogBookTypesEnum.Admission);
            }

            if (CurrentUser.Permissions.Contains(Permissions.TransportationLogBookPageRead) || CurrentUser.Permissions.Contains(Permissions.TransportationLogBookPageReadAll))
            {
                permittedLogBookTypes.Add(LogBookTypesEnum.Transportation);
            }

            if (CurrentUser.Permissions.Contains(Permissions.AquacultureLogBookPageRead) || CurrentUser.Permissions.Contains(Permissions.AquacultureLogBookPageReadAll))
            {
                permittedLogBookTypes.Add(LogBookTypesEnum.Aquaculture);
            }

            return permittedLogBookTypes;
        }
    }
}
