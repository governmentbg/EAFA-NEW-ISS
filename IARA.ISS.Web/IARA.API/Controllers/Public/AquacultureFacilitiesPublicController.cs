using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.DomainModels.DTOInterfaces;
using IARA.DomainModels.DTOModels.AquacultureFacilities;
using IARA.DomainModels.DTOModels.AquacultureFacilities.ChangeOfCircumstances;
using IARA.DomainModels.DTOModels.AquacultureFacilities.Deregistration;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.Nomenclatures;
using IARA.Interfaces;
using IARA.Interfaces.Nomenclatures;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Enums;
using Microsoft.AspNetCore.Mvc;

namespace IARA.WebAPI.Controllers.Public
{
    // TODO add security checks
    [AreaRoute(AreaType.Public)]
    public class AquacultureFacilitiesPublicController : BaseController
    {
        private readonly IAquacultureFacilitiesService service;
        private readonly IAquacultureFacilitiesNomenclaturesService nomenclaturesService;
        private readonly IFileService fileService;
        private readonly IApplicationService applicationService;
        private readonly IDeliveryService deliveryService;

        public AquacultureFacilitiesPublicController(IAquacultureFacilitiesService service,
                                                     IAquacultureFacilitiesNomenclaturesService nomenclaturesService,
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

        // Applications
        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead)]
        public IActionResult GetAquacultureApplication([FromQuery] int id)
        {
            if (applicationService.IsApplicationSubmittedByUserOrPerson(CurrentUser.ID, id))
            {
                AquacultureApplicationEditDTO result = service.GetAquacultureApplication(id);
                return Ok(result);
            }
            return NotFound();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetRegisterByApplicationId([FromQuery] int applicationId)
        {
            AquacultureFacilityEditDTO aquaculture = service.GetRegisterByApplicationId(applicationId);
            return Ok(aquaculture);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetRegisterByChangeOfCircumstancesApplicationId([FromQuery] int applicationId)
        {
            AquacultureFacilityEditDTO aquaculture = service.GetRegisterByChangeOfCircumstancesApplicationId(applicationId);
            return Ok(aquaculture);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsAddRecords)]
        [RequestFormLimits(ValueCountLimit = 5000)]
        public async Task<IActionResult> AddAquacultureApplication([FromForm] AquacultureApplicationEditDTO aquaculture)
        {
            IActionResult result = await CheckModel(aquaculture);

            if (result != null)
            {
                return result;
            }

            int id = service.AddAquacultureApplication(aquaculture);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsEditRecords)]
        [RequestFormLimits(ValueCountLimit = 5000)]
        public async Task<IActionResult> EditAquacultureApplication([FromForm] AquacultureApplicationEditDTO aquaculture)
        {
            if (aquaculture.ApplicationId.HasValue && applicationService.IsApplicationSubmittedByUser(CurrentUser.ID, aquaculture.ApplicationId.Value))
            {
                IActionResult result = await CheckModel(aquaculture);

                if (result != null)
                {
                    return result;
                }

                service.EditAquacultureApplication(aquaculture);
            }

            return NotFound();
        }

        // Change of circumstances
        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead)]
        public IActionResult GetAquacultureChangeOfCircumstancesApplication([FromQuery] int id)
        {
            if (applicationService.IsApplicationSubmittedByUserOrPerson(CurrentUser.ID, id))
            {
                AquacultureChangeOfCircumstancesApplicationDTO result = service.GetAquacultureChangeOfCircumstancesApplication(id);
                return Ok(result);
            }
            return NotFound();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsAddRecords)]
        public async Task<IActionResult> AddAquacultureChangeOfCircumstancesApplication([FromForm] AquacultureChangeOfCircumstancesApplicationDTO application)
        {
            IActionResult result = await CheckModel(application);

            if (result != null)
            {
                return result;
            }

            int id = service.AddAquacultureChangeOfCircumstancesApplication(application);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsEditRecords)]
        public async Task<IActionResult> EditAquacultureChangeOfCircumstancesApplication([FromForm] AquacultureChangeOfCircumstancesApplicationDTO application)
        {
            if (application.ApplicationId.HasValue && applicationService.IsApplicationSubmittedByUser(CurrentUser.ID, application.ApplicationId.Value))
            {
                IActionResult result = await CheckModel(application);

                if (result != null)
                {
                    return result;
                }

                service.EditAquacultureChangeOfCircumstancesApplication(application);
                return Ok();
            }

            return NotFound();
        }

        // Deregistration
        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead)]
        public IActionResult GetAquacultureDeregistrationApplication([FromQuery] int id)
        {
            if (applicationService.IsApplicationSubmittedByUserOrPerson(CurrentUser.ID, id))
            {
                AquacultureDeregistrationApplicationDTO result = service.GetAquacultureDeregistrationApplication(id);
                return Ok(result);
            }
            return NotFound();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsAddRecords)]
        public async Task<IActionResult> AddAquacultureDeregistrationApplication([FromForm] AquacultureDeregistrationApplicationDTO application)
        {
            IActionResult result = await CheckModel(application);

            if (result != null)
            {
                return result;
            }

            int id = service.AddAquacultureDeregistrationApplication(application);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsEditRecords)]
        public async Task<IActionResult> EditAquacultureDeregistrationApplication([FromForm] AquacultureDeregistrationApplicationDTO application)
        {
            if (application.ApplicationId.HasValue && applicationService.IsApplicationSubmittedByUser(CurrentUser.ID, application.ApplicationId.Value))
            {
                IActionResult result = await CheckModel(application);

                if (result != null)
                {
                    return result;
                }

                service.EditAquacultureDeregistrationApplication(application);
                return Ok();
            }

            return NotFound();
        }

        // Nomenclatures
        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetAllAquacultureNomenclatures()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetAllAquacultureNomenclatures();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetAquaculturePowerSupplyTypes()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetAquaculturePowerSupplyTypes();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetAquacultureWaterAreaTypes()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetAquacultureWaterAreaTypes();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetWaterLawCertificateTypes()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetWaterLawCertificateTypes();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetAquacultureInstallationTypes()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetAquacultureInstallationTypes();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetInstallationBasinPurposeTypes()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetInstallationBasinPurposeTypes();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetInstallationBasinMaterialTypes()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetInstallationBasinMaterialTypes();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetHatcheryEquipmentTypes()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetHatcheryEquipmentTypes();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetInstallationNetCageTypes()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetInstallationNetCageTypes();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetInstallationCollectorTypes()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetInstallationCollectorTypes();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
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
