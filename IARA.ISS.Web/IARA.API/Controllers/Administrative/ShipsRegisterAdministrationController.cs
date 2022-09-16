using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.DomainModels.DTOInterfaces;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.ApplicationsRegister;
using IARA.DomainModels.DTOModels.CommercialFishingRegister;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.ShipsRegister;
using IARA.DomainModels.DTOModels.ShipsRegister.IncreaseCapacity;
using IARA.DomainModels.DTOModels.ShipsRegister.ReduceCapacity;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Excel.Tools.Models;
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
    public class ShipsRegisterAdministrationController : BaseAuditController
    {
        private static readonly PageCodeEnum[] PAGE_CODES = new PageCodeEnum[] { PageCodeEnum.ShipRegChange, PageCodeEnum.RegVessel, PageCodeEnum.DeregShip };

        private readonly IShipsRegisterService service;
        private readonly IShipsRegisterNomenclaturesService nomenclaturesService;
        private readonly IApplicationService applicationService;
        private readonly IApplicationsRegisterService applicationsRegisterService;
        private readonly IDeliveryService deliveryService;
        private readonly IFileService fileService;
        private readonly IFishingGearsService fishingGearsService;
        private readonly IUserService userService;
        private readonly IMemoryCacheService memoryCacheService;
        private readonly ILogBooksService logBooksService;

        public ShipsRegisterAdministrationController(IShipsRegisterService service,
                                                     IShipsRegisterNomenclaturesService nomenclaturesService,
                                                     IApplicationService applicationService,
                                                     IApplicationsRegisterService applicationsRegisterService,
                                                     IDeliveryService deliveryService,
                                                     IFileService fileService,
                                                     IFishingGearsService fishingGearsService,
                                                     IPermissionsService permissionsService,
                                                     IUserService userService,
                                                     IMemoryCacheService memoryCacheService,
                                                     ILogBooksService logBooksService)
            : base(permissionsService)
        {
            this.service = service;
            this.nomenclaturesService = nomenclaturesService;
            this.applicationService = applicationService;
            this.applicationsRegisterService = applicationsRegisterService;
            this.deliveryService = deliveryService;
            this.fileService = fileService;
            this.fishingGearsService = fishingGearsService;
            this.userService = userService;
            this.memoryCacheService = memoryCacheService;
            this.logBooksService = logBooksService;
        }

        // Register
        [HttpPost]
        [CustomAuthorize(Permissions.ShipsRegisterReadAll, Permissions.ShipsRegisterRead)]
        public IActionResult GetAllShips([FromBody] GridRequestModel<ShipsRegisterFilters> request)
        {
            if (!CurrentUser.Permissions.Contains(Permissions.QualifiedFishersReadAll))
            {
                int? territoryUnitId = userService.GetUserTerritoryUnitId(CurrentUser.ID);

                if (territoryUnitId.HasValue)
                {
                    if (request.Filters == null)
                    {
                        request.Filters = new ShipsRegisterFilters
                        {
                            ShowInactiveRecords = false
                        };
                    }

                    request.Filters.TerritoryUnitId = territoryUnitId.Value;
                }
                else
                {
                    return Ok(Enumerable.Empty<ShipRegisterDTO>());
                }
            }

            IQueryable<ShipRegisterDTO> permits = service.GetAllShips(request.Filters);
            return PageResult(permits, request, false);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ShipsRegisterReadAll, Permissions.ShipsRegisterRead)]
        public IActionResult GetShip([FromQuery] int id)
        {
            ShipRegisterEditDTO ship = service.GetShip(id);
            return Ok(ship);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ShipsRegisterReadAll, Permissions.ShipsRegisterRead)]
        public IActionResult GetShipFromChangeOfCircumstancesApplication([FromQuery] int applicationId)
        {
            ShipRegisterEditDTO ship = service.GetShipFromChangeOfCircumstancesApplication(applicationId);
            return Ok(ship);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ShipsRegisterReadAll, Permissions.ShipsRegisterRead)]
        public IActionResult GetShipFromDeregistrationApplication([FromQuery] int applicationId)
        {
            ShipRegisterEditDTO ship = service.GetShipFromDeregistrationApplication(applicationId);
            return Ok(ship);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ShipsRegisterReadAll,
                         Permissions.ShipsRegisterRead,
                         Permissions.FishingCapacityApplicationsReadAll,
                         Permissions.FishingCapacityApplicationsRead)]
        public IActionResult GetShipFromIncreaseCapacityApplication([FromQuery] int applicationId)
        {
            ShipRegisterEditDTO ship = service.GetShipFromIncreaseCapacityApplication(applicationId);
            return Ok(ship);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ShipsRegisterReadAll,
                         Permissions.ShipsRegisterRead,
                         Permissions.FishingCapacityApplicationsReadAll,
                         Permissions.FishingCapacityApplicationsRead)]
        public IActionResult GetShipFromReduceCapacityApplication([FromQuery] int applicationId)
        {
            ShipRegisterEditDTO ship = service.GetShipFromReduceCapacityApplication(applicationId);
            return Ok(ship);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ShipsRegisterApplicationsInspectAndCorrectRegiXData)]
        public IActionResult GetShipRegixData([FromQuery] int applicationId)
        {
            RegixChecksWrapperDTO<ShipRegisterRegixDataDTO> data = service.GetShipRegixData(applicationId);
            return Ok(data);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ShipsRegisterReadAll, Permissions.ShipsRegisterRead)]
        public IActionResult GetApplicationDataForRegister([FromQuery] int applicationId)
        {
            ShipRegisterEditDTO ship = service.GetApplicationDataForRegister(applicationId);
            return Ok(ship);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ShipsRegisterReadAll, Permissions.ShipsRegisterRead)]
        public IActionResult GetRegisterByApplicationId([FromQuery] int applicationId)
        {
            ShipRegisterEditDTO aquaculture = service.GetRegisterByApplicationId(applicationId);
            return Ok(aquaculture);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ShipsRegisterReadAll, Permissions.ShipsRegisterRead)]
        public IActionResult GetRegisterByChangeOfCircumstancesApplicationId([FromQuery] int applicationId)
        {
            ShipRegisterEditDTO aquaculture = service.GetRegisterByChangeOfCircumstancesApplicationId(applicationId);
            return Ok(aquaculture);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ShipsRegisterReadAll, Permissions.ShipsRegisterRead)]
        public IActionResult GetRegisterByChangeCapacityApplicationId([FromQuery] int applicationId)
        {
            ShipRegisterEditDTO aquaculture = service.GetRegisterByChangeCapacityApplicationId(applicationId);
            return Ok(aquaculture);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ShipsRegisterReadAll, Permissions.ShipsRegisterRead)]
        public IActionResult GetShipEventHistory([FromQuery] int shipId)
        {
            List<ShipRegisterEventDTO> events = service.GetShipEventHistory(shipId);
            return Ok(events);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ShipsRegisterReadAll, Permissions.ShipsRegisterRead)]
        public IActionResult DownloadShipRegisterExcel([FromBody] ExcelExporterRequestModel<ShipsRegisterFilters> request)
        {
            Stream stream = service.DownloadShipRegisterExcel(request);
            return ExcelFile(stream, request.Filename);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ShipsRegisterAddRecords)]
        public IActionResult AddShip([FromForm] ShipRegisterEditDTO ship)
        {
            if (ship.IsThirdPartyShip!.Value)
            {
                return BadRequest();
            }

            int id = service.AddShip(ship);

            memoryCacheService.ForceRefresh<ICommonNomenclaturesService, List<ShipNomenclatureDTO>>("GetShips", (service) => service.GetShips().ToList());
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ShipsRegisterEditRecords)]
        public IActionResult EditShip([FromForm] ShipRegisterEditDTO ship)
        {
            if (ship.IsThirdPartyShip!.Value)
            {
                return BadRequest();
            }

            service.EditShip(ship);

            memoryCacheService.ForceRefresh<ICommonNomenclaturesService, List<ShipNomenclatureDTO>>("GetShips", (service) => service.GetShips().ToList());
            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ShipsRegisterThirdPartyShips)]
        public IActionResult AddThirdPartyShip([FromForm] ShipRegisterEditDTO ship)
        {
            if (!ship.IsThirdPartyShip!.Value)
            {
                return BadRequest();
            }

            int id = service.AddShip(ship);

            memoryCacheService.ForceRefresh<ICommonNomenclaturesService, List<ShipNomenclatureDTO>>("GetShips", (service) => service.GetShips().ToList());
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ShipsRegisterThirdPartyShips)]
        public IActionResult EditThirdPartyShip([FromForm] ShipRegisterEditDTO ship)
        {
            if (!ship.IsThirdPartyShip!.Value)
            {
                return BadRequest();
            }

            service.EditShip(ship);

            memoryCacheService.ForceRefresh<ICommonNomenclaturesService, List<ShipNomenclatureDTO>>("GetShips", (service) => service.GetShips().ToList());
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ShipsRegisterReadAll, Permissions.ShipsRegisterRead)]
        public override IActionResult GetAuditInfo([FromQuery] int id)
        {
            SimpleAuditDTO audit = service.GetSimpleAudit(id);
            return Ok(audit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ShipsRegisterReadAll,
                         Permissions.ShipsRegisterRead,
                         Permissions.ShipsRegisterApplicationReadAll,
                         Permissions.ShipsRegisterApplicationsRead)]
        public IActionResult GetShipOwnerSimpleAudit([FromQuery] int id)
        {
            SimpleAuditDTO audit = service.GetShipOwnerSimpleAudit(id);
            return Ok(audit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ShipsRegisterReadAll, Permissions.ShipsRegisterRead)]
        public IActionResult GetOriginDeclarationFishSimpleAudit([FromQuery] int id)
        {
            SimpleAuditDTO audit = logBooksService.GetOriginDeclarationFishSimpleAudit(id);
            return Ok(audit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ShipsRegisterReadAll,
                         Permissions.ShipsRegisterRead,
                         Permissions.ShipsRegisterApplicationReadAll,
                         Permissions.ShipsRegisterApplicationsRead)]
        public IActionResult DownloadFile(int id)
        {
            DownloadableFileDTO file = fileService.GetFileForDownload(id);
            return File(file.Bytes, file.MimeType, file.FileName);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ShipsRegisterReadAll, Permissions.ShipsRegisterRead)]
        public IActionResult GetShipFishingGears([FromQuery] int shipUId)
        {
            List<FishingGearDTO> result = fishingGearsService.GetShipFishingGears(shipUId);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ShipsRegisterReadAll, Permissions.ShipsRegisterRead)]
        public IActionResult GetShipCatchQuotaNomenclatures([FromQuery] int shipUId)
        {
            List<NomenclatureDTO> result = service.GetShipCatchQuotaNomenclatures(shipUId);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ShipsRegisterReadAll, Permissions.ShipsRegisterRead)]
        public IActionResult GetShipYearlyQuota([FromQuery] int shipCatchQuotaId)
        {
            ShipRegisterYearlyQuotaDTO quota = service.GetShipYearlyQuota(shipCatchQuotaId);
            return Ok(quota);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ShipsRegisterReadAll, Permissions.ShipsRegisterRead)]
        public IActionResult GetShipOriginDeclarations([FromBody] GridRequestModel<ShipRegisterOriginDeclarationsFilters> request)
        {
            IQueryable<ShipRegisterOriginDeclarationFishDTO> result = service.GetShipOriginDeclarations(request.Filters);
            return PageResult(result, request, false);
        }

        // Nomenclatures
        [HttpGet]
        [CustomAuthorize(Permissions.ShipsRegisterReadAll,
                         Permissions.ShipsRegisterRead,
                         Permissions.ShipsRegisterApplicationReadAll,
                         Permissions.ShipsRegisterApplicationsRead)]
        public IActionResult GetEventTypes()
        {
            List<ShipEventTypeDTO> types = nomenclaturesService.GetEventTypes();
            return Ok(types);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ShipsRegisterReadAll,
                         Permissions.ShipsRegisterRead,
                         Permissions.ShipsRegisterApplicationReadAll,
                         Permissions.ShipsRegisterApplicationsRead)]
        public IActionResult GetFleetTypes()
        {
            List<FleetTypeNomenclatureDTO> types = nomenclaturesService.GetFleetTypes();
            return Ok(types);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ShipsRegisterReadAll,
                         Permissions.ShipsRegisterRead,
                         Permissions.ShipsRegisterApplicationReadAll,
                         Permissions.ShipsRegisterApplicationsRead)]
        public IActionResult GetPublicAidTypes()
        {
            List<NomenclatureDTO> types = nomenclaturesService.GetPublicAidTypes();
            return Ok(types);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ShipsRegisterReadAll,
                         Permissions.ShipsRegisterRead,
                         Permissions.ShipsRegisterApplicationReadAll,
                         Permissions.ShipsRegisterApplicationsRead)]
        public IActionResult GetPublicAidSegments()
        {
            List<NomenclatureDTO> types = nomenclaturesService.GetSegments();
            return Ok(types);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ShipsRegisterReadAll,
                         Permissions.ShipsRegisterRead,
                         Permissions.ShipsRegisterApplicationReadAll,
                         Permissions.ShipsRegisterApplicationsRead)]
        public IActionResult GetSailAreas()
        {
            List<SailAreaNomenclatureDTO> areas = nomenclaturesService.GetSailAreas();
            return Ok(areas);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ShipsRegisterReadAll,
                         Permissions.ShipsRegisterRead,
                         Permissions.ShipsRegisterApplicationReadAll,
                         Permissions.ShipsRegisterApplicationsRead)]
        public IActionResult GetVesselTypes()
        {
            List<VesselTypeNomenclatureDTO> types = nomenclaturesService.GetVesselTypes();
            return Ok(types);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ShipsRegisterReadAll,
                         Permissions.ShipsRegisterRead,
                         Permissions.ShipsRegisterApplicationReadAll,
                         Permissions.ShipsRegisterApplicationsRead)]
        public IActionResult GetPorts()
        {
            List<NomenclatureDTO> ports = nomenclaturesService.GetPorts();
            return Ok(ports);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ShipsRegisterReadAll,
                         Permissions.ShipsRegisterRead,
                         Permissions.ShipsRegisterApplicationReadAll,
                         Permissions.ShipsRegisterApplicationsRead)]
        public IActionResult GetHullMaterials()
        {
            List<NomenclatureDTO> materials = nomenclaturesService.GetHullMaterials();
            return Ok(materials);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ShipsRegisterReadAll,
                         Permissions.ShipsRegisterRead,
                         Permissions.ShipsRegisterApplicationReadAll,
                         Permissions.ShipsRegisterApplicationsRead)]
        public IActionResult GetFuelTypes()
        {
            List<NomenclatureDTO> types = nomenclaturesService.GetFuelTypes();
            return Ok(types);
        }

        // Applications
        [HttpPost]
        [CustomAuthorize(Permissions.ShipsRegisterApplicationReadAll, Permissions.ShipsRegisterApplicationsRead)]
        public IActionResult GetAllApplications([FromBody] GridRequestModel<ApplicationsRegisterFilters> request)
        {
            if (!CurrentUser.Permissions.Contains(Permissions.ShipsRegisterApplicationReadAll))
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
                    return Ok(Enumerable.Empty<ShipRegisterDTO>());
                }
            }

            IQueryable<ApplicationRegisterDTO> permits = applicationsRegisterService.GetAllApplications(request.Filters, null, PAGE_CODES);
            return PageResult(permits, request, false);
        }

        // register ship application
        [HttpGet]
        [CustomAuthorize(Permissions.ShipsRegisterApplicationReadAll, Permissions.ShipsRegisterApplicationsRead)]
        public IActionResult GetShipApplication([FromQuery] int id)
        {
            ShipRegisterApplicationEditDTO result = service.GetShipApplication(id);
            return Ok(result);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ShipsRegisterApplicationsAddRecords)]
        public async Task<IActionResult> AddShipApplication([FromForm] ShipRegisterApplicationEditDTO ship)
        {
            IActionResult result = await CheckModel(ship);

            if (result != null)
            {
                return result;
            }

            int id = service.AddShipApplication(ship, ApplicationStatusesEnum.EXT_CHK_STARTED);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ShipsRegisterApplicationsEditRecords)]
        public async Task<IActionResult> EditShipApplication([FromQuery] bool saveAsDraft, [FromForm] ShipRegisterApplicationEditDTO ship)
        {
            IActionResult result = await CheckModel(ship);

            if (result != null)
            {
                return result;
            }

            if (saveAsDraft)
            {
                service.EditShipApplication(ship);
            }
            else
            {
                service.EditShipApplication(ship, ApplicationStatusesEnum.EXT_CHK_STARTED);
            }

            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ShipsRegisterApplicationsEditRecords)]
        public IActionResult EditShipApplicationAndStartRegixChecks([FromForm] ShipRegisterRegixDataDTO ship)
        {
            service.EditShipApplicationRegixData(ship);
            return Ok();
        }

        // change of circumstances application
        [HttpGet]
        [CustomAuthorize(Permissions.ShipsRegisterApplicationReadAll, Permissions.ShipsRegisterApplicationsRead)]
        public IActionResult GetShipChangeOfCircumstancesApplication([FromQuery] int id)
        {
            ShipChangeOfCircumstancesApplicationDTO result = service.GetShipChangeOfCircumstancesApplication(id);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ShipsRegisterApplicationsInspectAndCorrectRegiXData)]
        public IActionResult GetShipChangeOfCircumstancesRegixData([FromQuery] int applicationId)
        {
            RegixChecksWrapperDTO<ShipChangeOfCircumstancesRegixDataDTO> data = service.GetShipChangeOfCircumstancesRegixData(applicationId);
            return Ok(data);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ShipsRegisterApplicationsAddRecords)]
        public async Task<IActionResult> AddShipChangeOfCircumstancesApplication([FromForm] ShipChangeOfCircumstancesApplicationDTO application)
        {
            IActionResult result = await CheckModel(application);

            if (result != null)
            {
                return result;
            }

            int id = service.AddShipChangeOfCircumstancesApplication(application, ApplicationStatusesEnum.EXT_CHK_STARTED);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ShipsRegisterApplicationsEditRecords)]
        public async Task<IActionResult> EditShipChangeOfCircumstancesApplication([FromQuery] bool saveAsDraft, [FromForm] ShipChangeOfCircumstancesApplicationDTO application)
        {
            IActionResult result = await CheckModel(application);

            if (result != null)
            {
                return result;
            }

            if (saveAsDraft)
            {
                service.EditShipChangeOfCircumstancesApplication(application);
            }
            else
            {
                service.EditShipChangeOfCircumstancesApplication(application, ApplicationStatusesEnum.EXT_CHK_STARTED);
            }

            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ShipsRegisterApplicationsEditRecords)]
        public IActionResult EditShipChangeOfCircumstancesApplicationAndStartRegixChecks([FromForm] ShipChangeOfCircumstancesRegixDataDTO application)
        {
            service.EditShipChangeOfCircumstancesRegixData(application);
            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ShipsRegisterEditRecords, Permissions.ShipsRegisterApplicationsEditRecords)]
        public IActionResult CompleteShipChangeOfCircumstancesApplication([FromForm] ShipRegisterChangeOfCircumstancesDTO changes)
        {
            service.CompleteShipChangeOfCircumstancesApplication(changes);

            memoryCacheService.ForceRefresh<ICommonNomenclaturesService, List<ShipNomenclatureDTO>>("GetShips", (service) => service.GetShips().ToList());
            return Ok();
        }

        // deregistration application
        [HttpGet]
        [CustomAuthorize(Permissions.ShipsRegisterApplicationReadAll, Permissions.ShipsRegisterApplicationsRead)]
        public IActionResult GetShipDeregistrationApplication([FromQuery] int id)
        {
            ShipDeregistrationApplicationDTO result = service.GetShipDeregistrationApplication(id);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ShipsRegisterApplicationsInspectAndCorrectRegiXData)]
        public IActionResult GetShipDeregistrationRegixData([FromQuery] int applicationId)
        {
            RegixChecksWrapperDTO<ShipDeregistrationRegixDataDTO> data = service.GetShipDeregistrationRegixData(applicationId);
            return Ok(data);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ShipsRegisterApplicationsAddRecords)]
        public async Task<IActionResult> AddShipDeregistrationApplication([FromForm] ShipDeregistrationApplicationDTO application)
        {
            IActionResult result = await CheckModel(application);

            if (result != null)
            {
                return result;
            }

            int id = service.AddShipDeregistrationApplication(application, ApplicationStatusesEnum.EXT_CHK_STARTED);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ShipsRegisterApplicationsEditRecords)]
        public async Task<IActionResult> EditShipDeregistrationApplication([FromQuery] bool saveAsDraft, [FromForm] ShipDeregistrationApplicationDTO application)
        {
            IActionResult result = await CheckModel(application);

            if (result != null)
            {
                return result;
            }

            if (saveAsDraft)
            {
                service.EditShipDeregistrationApplication(application);
            }
            else
            {
                service.EditShipDeregistrationApplication(application, ApplicationStatusesEnum.EXT_CHK_STARTED);
            }

            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ShipsRegisterApplicationsEditRecords)]
        public IActionResult EditShipDeregistrationApplicationAndStartRegixChecks([FromForm] ShipDeregistrationRegixDataDTO application)
        {
            service.EditShipDeregistrationRegixData(application);
            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ShipsRegisterEditRecords, Permissions.ShipsRegisterApplicationsEditRecords)]
        public IActionResult CompleteShipDeregistrationApplication([FromForm] ShipRegisterDeregistrationDTO changes)
        {
            service.CompleteShipDeregistrationApplication(changes);

            memoryCacheService.ForceRefresh<ICommonNomenclaturesService, List<ShipNomenclatureDTO>>("GetShips", (service) => service.GetShips().ToList());
            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ShipsRegisterEditRecords, Permissions.FishingCapacityApplicationsAddRecords)]
        public IActionResult CompleteShipIncreaseCapacityApplication([FromForm] ShipRegisterIncreaseCapacityDTO changes)
        {
            service.CompleteShipIncreaseCapacityApplication(changes);

            memoryCacheService.ForceRefresh<ICommonNomenclaturesService, List<ShipNomenclatureDTO>>("GetShips", (service) => service.GetShips().ToList());
            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ShipsRegisterEditRecords, Permissions.FishingCapacityApplicationsAddRecords)]
        public IActionResult CompleteShipReduceCapacityApplication([FromForm] ShipRegisterReduceCapacityDTO changes)
        {
            service.CompleteShipReduceCapacityApplication(changes);

            memoryCacheService.ForceRefresh<ICommonNomenclaturesService, List<ShipNomenclatureDTO>>("GetShips", (service) => service.GetShips().ToList());
            return Ok();
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.ShipsRegisterApplicationsDeleteRecords)]
        public IActionResult DeleteApplication([FromQuery] int id)
        {
            applicationsRegisterService.DeleteApplication(id);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.ShipsRegisterApplicationsRestoreRecords)]
        public IActionResult UndoDeleteApplication([FromQuery] int id)
        {
            applicationsRegisterService.UndoDeleteApplication(id);
            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ShipsRegisterApplicationsEditRecords)]
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
        [CustomAuthorize(Permissions.ShipsRegisterApplicationReadAll, Permissions.ShipsRegisterApplicationsRead)]
        public IActionResult GetApplicationStatuses()
        {
            List<NomenclatureDTO> statuses = applicationsRegisterService.GetApplicationStatuses(ApplicationHierarchyTypesEnum.Online, ApplicationHierarchyTypesEnum.OnPaper);
            return Ok(statuses);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ShipsRegisterApplicationReadAll, Permissions.ShipsRegisterApplicationsRead)]
        public IActionResult GetApplicationTypes()
        {
            List<NomenclatureDTO> types = applicationsRegisterService.GetApplicationTypes(PAGE_CODES);
            return Ok(types);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.ShipsRegisterApplicationReadAll, Permissions.ShipsRegisterApplicationsRead)]
        public IActionResult GetApplicationSources()
        {
            List<NomenclatureDTO> sources = applicationsRegisterService.GetApplicationSources(ApplicationHierarchyTypesEnum.Online, ApplicationHierarchyTypesEnum.OnPaper);
            return Ok(sources);
        }

        // Utils
        private async Task<IActionResult> CheckModel(ShipRegisterApplicationEditDTO ship)
        {
            if (ship.AcquiredFishingCapacity != null)
            {
                if (ship.RemainingCapacityAction != null && ship.RemainingCapacityAction.Action != FishingCapacityRemainderActionEnum.NoCertificate)
                {
                    return await CheckModel(ship as IDeliverableApplication);
                }
            }

            return null;
        }

        private async Task<IActionResult> CheckModel(ShipDeregistrationApplicationDTO application)
        {
            if (application.FreedCapacityAction != null && application.FreedCapacityAction.Action != FishingCapacityRemainderActionEnum.NoCertificate)
            {
                return await CheckModel(application as IDeliverableApplication);
            }

            return null;
        }

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
    }
}
