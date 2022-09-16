using System.Collections.Generic;
using System.Linq;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.StatisticalForms;
using IARA.DomainModels.DTOModels.StatisticalForms.AquaFarms;
using IARA.DomainModels.DTOModels.StatisticalForms.FishVessels;
using IARA.DomainModels.DTOModels.StatisticalForms.Reworks;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces;
using IARA.Interfaces.CommercialFishing;
using IARA.Interfaces.Nomenclatures;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Models;
using Microsoft.AspNetCore.Mvc;

namespace IARA.WebAPI.Controllers.Public
{
    // TODO add security checks
    [AreaRoute(AreaType.Public)]
    public class StatisticalFormsPublicController : BaseController
    {
        private readonly IStatisticalFormsService service;
        private readonly IStatisticalFormsNomenclaturesService nomenclaturesService;
        private readonly IApplicationService applicationService;
        private readonly IFileService fileService;
        private readonly IFishingGearsService fishingGearsService;

        public StatisticalFormsPublicController(IStatisticalFormsService service,
                                                IStatisticalFormsNomenclaturesService nomenclaturesService,
                                                IApplicationService applicationService,
                                                IFileService fileService,
                                                IFishingGearsService fishingGearsService,
                                                IPermissionsService permissionsService)
            : base(permissionsService)
        {
            this.service = service;
            this.nomenclaturesService = nomenclaturesService;
            this.applicationService = applicationService;
            this.fishingGearsService = fishingGearsService;
            this.fileService = fileService;
        }

        // Register
        [HttpPost]
        [CustomAuthorize(Permissions.StatisticalFormsAquaFarmRead,
                         Permissions.StatisticalFormsReworkRead,
                         Permissions.StatisticalFormsFishVesselRead)]
        public IActionResult GetAllStatisticalForms([FromBody] GridRequestModel<StatisticalFormsFilters> request)
        {
            List<StatisticalFormTypesEnum> pageCodes = new();

            if (CurrentUser.Permissions.Contains(Permissions.StatisticalFormsAquaFarmRead))
            {
                pageCodes.Add(StatisticalFormTypesEnum.AquaFarm);
            }

            if (CurrentUser.Permissions.Contains(Permissions.StatisticalFormsReworkRead))
            {
                pageCodes.Add(StatisticalFormTypesEnum.Rework);
            }

            if (CurrentUser.Permissions.Contains(Permissions.StatisticalFormsFishVesselRead))
            {
                pageCodes.Add(StatisticalFormTypesEnum.FishVessel);
            }

            IQueryable<StatisticalFormDTO> result = service.GetAllStatisticalForms(request.Filters, pageCodes, CurrentUser.ID);
            return PageResult(result, request, false);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.StatisticalFormsAquaFarmRead)]
        public IActionResult GetStatisticalFormAquaFarm([FromQuery] int id)
        {
            StatisticalFormAquaFarmEditDTO form = service.GetStatisticalFormAquaFarm(id);
            return Ok(form);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.StatisticalFormsReworkRead)]
        public IActionResult GetStatisticalFormRework([FromQuery] int id)
        {
            StatisticalFormReworkEditDTO form = service.GetStatisticalFormRework(id);
            return Ok(form);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.StatisticalFormsFishVesselRead)]
        public IActionResult GetStatisticalFormFishVessel([FromQuery] int id)
        {
            StatisticalFormFishVesselEditDTO form = service.GetStatisticalFormFishVessel(id);
            return Ok(form);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetAquaFarmRegisterByApplicationId([FromQuery] int applicationId)
        {
            StatisticalFormAquaFarmEditDTO permit = service.GetAquaFarmRegisterByApplicationId(applicationId);
            return Ok(permit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetFishVesselRegisterByApplicationId([FromQuery] int applicationId)
        {
            StatisticalFormFishVesselEditDTO permit = service.GetFishVesselRegisterByApplicationId(applicationId);
            return Ok(permit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetReworkRegisterByApplicationId([FromQuery] int applicationId)
        {
            StatisticalFormReworkEditDTO permit = service.GetReworkRegisterByApplicationId(applicationId);
            return Ok(permit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.StatisticalFormsAquaFarmRead,
                         Permissions.StatisticalFormsReworkRead,
                         Permissions.StatisticalFormsFishVesselRead,
                         Permissions.OnlineSubmittedApplicationsRead, 
                         Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult DownloadFile([FromQuery] int id)
        {
            DownloadableFileDTO file = fileService.GetFileForDownload(id);
            return File(file.Bytes, file.MimeType, file.FileName);
        }

        [HttpGet]
        [CustomAuthorize()]
        public IActionResult GetCurrentUserAsSubmittedBy([FromQuery] StatisticalFormTypesEnum type)
        {
            ApplicationSubmittedByDTO result = service.GetUserAsSubmittedBy(CurrentUser.ID, type);
            return Ok(result);
        }

        // Aquaculture farms
        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead)]
        public IActionResult GetStatisticalFormAquaFarmApplication([FromQuery] int id)
        {
            if (applicationService.IsApplicationSubmittedByUserOrPerson(CurrentUser.ID, id))
            {
                StatisticalFormAquaFarmApplicationEditDTO form = service.GetStatisticalFormAquaFarmApplication(id);
                return Ok(form);
            }
            return NotFound();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsAddRecords)]
        public IActionResult AddStatisticalFormAquaFarmApplication([FromForm] StatisticalFormAquaFarmApplicationEditDTO form)
        {
            int id = service.AddStatisticalFormAquaFarmApplication(form);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsEditRecords)]
        public IActionResult EditStatisticalFormAquaFarmApplication([FromForm] StatisticalFormAquaFarmApplicationEditDTO form)
        {
            if (form.ApplicationId.HasValue && applicationService.IsApplicationSubmittedByUser(CurrentUser.ID, form.ApplicationId.Value))
            {
                service.EditStatisticalFormAquaFarmApplication(form);
                return Ok();
            }
            return NotFound();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, 
                         Permissions.OnlineSubmittedApplicationsReadRegister,
                         Permissions.StatisticalFormsAquaFarmRead,
                         Permissions.StatisticalFormsReworkRead,
                         Permissions.StatisticalFormsFishVesselRead)]
        public IActionResult GetStatisticalFormAquaculture([FromQuery] int aquacultureId)
        {
            StatisticalFormAquacultureDTO result = service.GetStatisticalFormAquaculture(aquacultureId);
            return Ok(result);
        }

        // Reworks
        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead)]
        public IActionResult GetStatisticalFormReworkApplication([FromQuery] int id)
        {
            if (applicationService.IsApplicationSubmittedByUserOrPerson(CurrentUser.ID, id))
            {
                StatisticalFormReworkApplicationEditDTO form = service.GetStatisticalFormReworkApplication(id);
                return Ok(form);
            }
            return NotFound();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsAddRecords)]
        public IActionResult AddStatisticalFormReworkApplication([FromForm] StatisticalFormReworkApplicationEditDTO form)
        {
            int id = service.AddStatisticalFormReworkApplication(form);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsEditRecords)]
        public IActionResult EditStatisticalFormReworkApplication([FromForm] StatisticalFormReworkApplicationEditDTO form)
        {
            if (form.ApplicationId.HasValue && applicationService.IsApplicationSubmittedByUser(CurrentUser.ID, form.ApplicationId.Value))
            {
                service.EditStatisticalFormReworkApplication(form);
                return Ok();
            }
            return NotFound();
        }

        // Fishing vessel
        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, 
                         Permissions.OnlineSubmittedApplicationsReadRegister,
                         Permissions.StatisticalFormsFishVesselRead)]
        public IActionResult GetStatisticalFormShip([FromQuery] int shipId)
        {
            StatisticalFormShipDTO result = service.GetStatisticalFormShip(shipId);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, 
                         Permissions.OnlineSubmittedApplicationsReadRegister,
                         Permissions.StatisticalFormsFishVesselRead)]
        public IActionResult GetShipFishingGearsForYear([FromQuery] int shipId, [FromQuery] int year)
        {
            List<NomenclatureDTO> result = fishingGearsService.GetShipFishingGearNomenclatures(shipId, year);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetStatisticalFormFishVesselApplication([FromQuery] int id)
        {
            if (applicationService.IsApplicationSubmittedByUserOrPerson(CurrentUser.ID, id))
            {
                StatisticalFormFishVesselApplicationEditDTO form = service.GetStatisticalFormFishVesselApplication(id);
                return Ok(form);
            }
            return NotFound();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsAddRecords)]
        public IActionResult AddStatisticalFormFishVesselApplication([FromForm] StatisticalFormFishVesselApplicationEditDTO form)
        {
            int id = service.AddStatisticalFormFishVesselApplication(form);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsEditRecords)]
        public IActionResult EditStatisticalFormFishVesselApplication([FromForm] StatisticalFormFishVesselApplicationEditDTO form)
        {
            if (form.ApplicationId.HasValue && applicationService.IsApplicationSubmittedByUser(CurrentUser.ID, form.ApplicationId.Value))
            {
                service.EditStatisticalFormFishVesselApplication(form);
                return Ok();
            }

            return NotFound();
        }

        //Nomenclatures
        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, 
                         Permissions.OnlineSubmittedApplicationsReadRegister,
                         Permissions.StatisticalFormsFishVesselRead)]
        public IActionResult GetGrossTonnageIntervals()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetGrossTonnageIntervals();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, 
                         Permissions.OnlineSubmittedApplicationsReadRegister,
                         Permissions.StatisticalFormsFishVesselRead)]
        public IActionResult GetVesselLengthIntervals()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetVesselLengthIntervals();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, 
                         Permissions.OnlineSubmittedApplicationsReadRegister,
                         Permissions.StatisticalFormsFishVesselRead)]
        public IActionResult GetFuelTypes()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetFuelTypes();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, 
                         Permissions.OnlineSubmittedApplicationsReadRegister,
                         Permissions.StatisticalFormsReworkRead)]
        public IActionResult GetReworkProductTypes()
        {
            List<NomenclatureDTO> result = nomenclaturesService.GetReworkProductTypes();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, 
                         Permissions.OnlineSubmittedApplicationsReadRegister,
                         Permissions.StatisticalFormsAquaFarmRead)]
        public IActionResult GetAllAquacultureNomenclatures()
        {
            List<StatisticalFormAquacultureNomenclatureDTO> result = nomenclaturesService.GetAllAquacultureNomenclatures();
            return Ok(result);
        }
    }
}
