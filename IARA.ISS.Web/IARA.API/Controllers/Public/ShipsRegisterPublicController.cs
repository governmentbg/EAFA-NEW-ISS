using System.Collections.Generic;
using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.ShipsRegister;
using IARA.DomainModels.Nomenclatures;
using IARA.Interfaces;
using IARA.Interfaces.Nomenclatures;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace IARA.Web.Controllers.Public
{
    // TODO add security checks
    [AreaRoute(AreaType.Public)]
    public class ShipsRegisterPublicController : BaseController
    {
        private readonly IShipsRegisterService service;
        private readonly IShipsRegisterNomenclaturesService nomenclaturesService;
        private readonly IFileService fileService;
        private readonly IApplicationService applicationService;
        private readonly IDeliveryService deliveryService;

        public ShipsRegisterPublicController(IShipsRegisterService service,
                                             IShipsRegisterNomenclaturesService nomenclaturesService,
                                             IFileService fileService,
                                             IApplicationService applicationService,
                                             IDeliveryService deliveryService,
                                             IPermissionsService permissionsService)
            : base(permissionsService)
        {
            this.service = service;
            this.nomenclaturesService = nomenclaturesService;
            this.fileService = fileService;
            this.applicationService = applicationService;
            this.deliveryService = deliveryService;
        }

        // Register
        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult DownloadFile(int id)
        {
            DownloadableFileDTO file = fileService.GetFileForDownload(id);
            return File(file.Bytes, file.MimeType, file.FileName);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetRegisterByApplicationId([FromQuery] int applicationId)
        {
            ShipRegisterEditDTO aquaculture = service.GetRegisterByApplicationId(applicationId);
            return Ok(aquaculture);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetRegisterByChangeOfCircumstancesApplicationId([FromQuery] int applicationId)
        {
            ShipRegisterEditDTO aquaculture = service.GetRegisterByChangeOfCircumstancesApplicationId(applicationId);
            return Ok(aquaculture);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetRegisterByChangeCapacityApplicationId([FromQuery] int applicationId)
        {
            ShipRegisterEditDTO aquaculture = service.GetRegisterByChangeCapacityApplicationId(applicationId);
            return Ok(aquaculture);
        }

        // Nomenclatures
        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetFleetTypes()
        {
            List<FleetTypeNomenclatureDTO> types = nomenclaturesService.GetFleetTypes();
            return Ok(types);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetPublicAidTypes()
        {
            List<NomenclatureDTO> types = nomenclaturesService.GetPublicAidTypes();
            return Ok(types);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetPublicAidSegments()
        {
            List<NomenclatureDTO> types = nomenclaturesService.GetSegments();
            return Ok(types);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetSailAreas()
        {
            List<SailAreaNomenclatureDTO> areas = nomenclaturesService.GetSailAreas();
            return Ok(areas);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetVesselTypes()
        {
            List<VesselTypeNomenclatureDTO> types = nomenclaturesService.GetVesselTypes();
            return Ok(types);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetPorts()
        {
            List<NomenclatureDTO> ports = nomenclaturesService.GetPorts();
            return Ok(ports);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetHullMaterials()
        {
            List<NomenclatureDTO> materials = nomenclaturesService.GetHullMaterials();
            return Ok(materials);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetFuelTypes()
        {
            List<NomenclatureDTO> types = nomenclaturesService.GetFuelTypes();
            return Ok(types);
        }

        // Applications
        // register ship applications
        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead)]
        public IActionResult GetShipApplication([FromQuery] int id)
        {
            if (applicationService.IsApplicationSubmittedByUserOrPerson(CurrentUser.ID, id))
            {
                ShipRegisterApplicationEditDTO result = service.GetShipApplication(id);
                return Ok(result);
            }
            return NotFound();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsAddRecords)]
        public async Task<IActionResult> AddShipApplication([FromForm] ShipRegisterApplicationEditDTO ship)
        {
            IActionResult result = await CheckModel(ship);

            if (result != null)
            {
                return result;
            }

            int id = service.AddShipApplication(ship, null);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsEditRecords)]
        public async Task<IActionResult> EditShipApplication([FromForm] ShipRegisterApplicationEditDTO ship)
        {
            if (ship.ApplicationId.HasValue && applicationService.IsApplicationSubmittedByUser(CurrentUser.ID, ship.ApplicationId.Value))
            {
                IActionResult result = await CheckModel(ship);

                if (result != null)
                {
                    return result;
                }

                service.EditShipApplication(ship);
                return Ok();
            }

            return NotFound();
        }

        // change of circumstances application
        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead)]
        public IActionResult GetShipChangeOfCircumstancesApplication([FromQuery] int id)
        {
            if (applicationService.IsApplicationSubmittedByUserOrPerson(CurrentUser.ID, id))
            {
                ShipChangeOfCircumstancesApplicationDTO result = service.GetShipChangeOfCircumstancesApplication(id);
                return Ok(result);
            }
            return NotFound();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsAddRecords)]
        public async Task<IActionResult> AddShipChangeOfCircumstancesApplication([FromForm] ShipChangeOfCircumstancesApplicationDTO application)
        {
            IActionResult result = await CheckModel(application);

            if (result != null)
            {
                return result;
            }

            int id = service.AddShipChangeOfCircumstancesApplication(application);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsEditRecords)]
        public async Task<IActionResult> EditShipChangeOfCircumstancesApplication([FromForm] ShipChangeOfCircumstancesApplicationDTO application)
        {
            if (application.ApplicationId.HasValue && applicationService.IsApplicationSubmittedByUser(CurrentUser.ID, application.ApplicationId.Value))
            {
                IActionResult result = await CheckModel(application);

                if (result != null)
                {
                    return result;
                }

                service.EditShipChangeOfCircumstancesApplication(application);
                return Ok();
            }

            return NotFound();
        }

        // deregistration application
        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead)]
        public IActionResult GetShipDeregistrationApplication([FromQuery] int id)
        {
            if (applicationService.IsApplicationSubmittedByUserOrPerson(CurrentUser.ID, id))
            {
                ShipDeregistrationApplicationDTO result = service.GetShipDeregistrationApplication(id);
                return Ok(result);
            }
            return NotFound();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsAddRecords)]
        public async Task<IActionResult> AddShipDeregistrationApplication([FromForm] ShipDeregistrationApplicationDTO application)
        {
            IActionResult result = await CheckModel(application);

            if (result != null)
            {
                return result;
            }

            int id = service.AddShipDeregistrationApplication(application);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsEditRecords)]
        public async Task<IActionResult> EditShipDeregistrationApplication([FromForm] ShipDeregistrationApplicationDTO application)
        {
            if (application.ApplicationId.HasValue && applicationService.IsApplicationSubmittedByUser(CurrentUser.ID, application.ApplicationId.Value))
            {
                IActionResult result = await CheckModel(application);

                if (result != null)
                {
                    return result;
                }

                service.EditShipDeregistrationApplication(application);
                return Ok();
            }

            return NotFound();
        }

        // Utils
        private async Task<IActionResult> CheckModel(ShipRegisterApplicationEditDTO ship)
        {
            if (ship.AcquiredFishingCapacity != null)
            {
                if (ship.RemainingCapacityAction != null && ship.RemainingCapacityAction.Action != FishingCapacityRemainderActionEnum.NoCertificate)
                {
                    bool hasDelivery = deliveryService.HasApplicationDelivery(ship.ApplicationId!.Value);

                    if (hasDelivery)
                    {
                        if (ship.DeliveryData == null)
                        {
                            return BadRequest("No delivery data provided for new free capacity certificate");
                        }
                        else
                        {
                            bool hasEDelivery = await deliveryService.HasSubmittedForEDelivery(ship.DeliveryData.DeliveryTypeId,
                                                                                               ship.SubmittedFor,
                                                                                               ship.SubmittedBy);

                            if (hasEDelivery == false)
                            {
                                return ValidationFailedResult(new List<string> { nameof(ApplicationValidationErrorsEnum.NoEDeliveryRegistration) });
                            }
                        }
                    }
                }
            }

            return null;
        }

        private async Task<IActionResult> CheckModel(ShipChangeOfCircumstancesApplicationDTO application)
        {
            if (application.DeliveryData != null)
            {
                bool hasEDelivery = await deliveryService.HasSubmittedForEDelivery(application.DeliveryData.DeliveryTypeId,
                                                                                   application.SubmittedFor,
                                                                                   application.SubmittedBy);

                if (hasEDelivery == false)
                {
                    return ValidationFailedResult(new List<string> { nameof(ApplicationValidationErrorsEnum.NoEDeliveryRegistration) });
                }
            }

            return null;
        }

        private async Task<IActionResult> CheckModel(ShipDeregistrationApplicationDTO application)
        {
            if (application.FreedCapacityAction != null && application.FreedCapacityAction.Action != FishingCapacityRemainderActionEnum.NoCertificate)
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
            }

            return null;
        }
    }
}
