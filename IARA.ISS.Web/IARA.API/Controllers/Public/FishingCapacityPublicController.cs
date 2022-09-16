using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.DomainModels.DTOInterfaces;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.FishingCapacity;
using IARA.DomainModels.DTOModels.FishingCapacity.Duplicates;
using IARA.DomainModels.DTOModels.FishingCapacity.IncreaseCapacity;
using IARA.DomainModels.DTOModels.FishingCapacity.ReduceCapacity;
using IARA.DomainModels.DTOModels.FishingCapacity.TransferCapacity;
using IARA.Interfaces;
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
    public class FishingCapacityPublicController : BaseController
    {
        private readonly IFishingCapacityService service;
        private readonly IFileService fileService;
        private readonly IApplicationService applicationService;
        private readonly IDeliveryService deliveryService;

        public FishingCapacityPublicController(IFishingCapacityService service,
                                               IFileService fileService,
                                               IApplicationService applicationService,
                                               IPermissionsService permissionsService,
                                               IDeliveryService deliveryService)
            : base(permissionsService)
        {
            this.service = service;
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

        // Nomenclatures
        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetAllCapacityCertificateNomenclatures()
        {
            List<FishingCapacityCertificateNomenclatureDTO> licences = service.GetAllCapacityCertificateNomenclatures(CurrentUser.ID);
            return Ok(licences);
        }

        // Applications
        // Increase capacity applications
        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead)]
        public IActionResult GetIncreaseFishingCapacityApplication([FromQuery] int id)
        {
            if (applicationService.IsApplicationSubmittedByUserOrPerson(CurrentUser.ID, id))
            {
                IncreaseFishingCapacityApplicationDTO result = service.GetIncreaseFishingCapacityApplication(id);
                return Ok(result);
            }
            return NotFound();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsAddRecords)]
        public async Task<IActionResult> AddIncreaseFishingCapacityApplication([FromForm] IncreaseFishingCapacityApplicationDTO application)
        {
            IActionResult result = await CheckModel(application);

            if (result != null)
            {
                return result;
            }

            int id = service.AddIncreaseFishingCapacityApplication(application, null);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsEditRecords)]
        public async Task<IActionResult> EditIncreaseFishingCapacityApplication([FromForm] IncreaseFishingCapacityApplicationDTO application)
        {
            if (application.ApplicationId.HasValue && applicationService.IsApplicationSubmittedByUser(CurrentUser.ID, application.ApplicationId.Value))
            {
                IActionResult result = await CheckModel(application);

                if (result != null)
                {
                    return result;
                }

                service.EditIncreaseFishingCapacityApplication(application);
                return Ok();
            }

            return NotFound();
        }

        // Reduce capacity applications
        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead)]
        public IActionResult GetReduceFishingCapacityApplication([FromQuery] int id)
        {
            if (applicationService.IsApplicationSubmittedByUserOrPerson(CurrentUser.ID, id))
            {
                ReduceFishingCapacityApplicationDTO result = service.GetReduceFishingCapacityApplication(id);
                return Ok(result);
            }
            return NotFound();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsAddRecords)]
        public async Task<IActionResult> AddReduceFishingCapacityApplication([FromForm] ReduceFishingCapacityApplicationDTO application)
        {
            IActionResult result = await CheckModel(application);

            if (result != null)
            {
                return result;
            }

            int id = service.AddReduceFishingCapacityApplication(application, null);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsEditRecords)]
        public async Task<IActionResult> EditReduceFishingCapacityApplication([FromForm] ReduceFishingCapacityApplicationDTO application)
        {
            if (application.ApplicationId.HasValue && applicationService.IsApplicationSubmittedByUser(CurrentUser.ID, application.ApplicationId.Value))
            {
                IActionResult result = await CheckModel(application);

                if (result != null)
                {
                    return result;
                }

                service.EditReduceFishingCapacityApplication(application);
                return Ok();
            }

            return NotFound();
        }

        // Transfer capacity applications
        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead)]
        public IActionResult GetTransferFishingCapacityApplication([FromQuery] int id)
        {
            if (applicationService.IsApplicationSubmittedByUserOrPerson(CurrentUser.ID, id))
            {
                TransferFishingCapacityApplicationDTO result = service.GetTransferFishingCapacityApplication(id);
                return Ok(result);
            }
            return NotFound();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsAddRecords)]
        public async Task<IActionResult> AddTransferFishingCapacityApplication([FromForm] TransferFishingCapacityApplicationDTO application)
        {
            IActionResult result = await CheckModel(application);

            if (result != null)
            {
                return result;
            }

            int id = service.AddTransferFishingCapacityApplication(application, null);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsEditRecords)]
        public async Task<IActionResult> EditTransferFishingCapacityApplication([FromForm] TransferFishingCapacityApplicationDTO application)
        {
            if (application.ApplicationId.HasValue && applicationService.IsApplicationSubmittedByUser(CurrentUser.ID, application.ApplicationId.Value))
            {
                IActionResult result = await CheckModel(application);

                if (result != null)
                {
                    return result;
                }

                service.EditTransferFishingCapacityApplication(application);
                return Ok();
            }
            return NotFound();
        }

        // Capacity certificate duplicate applications
        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead)]
        public IActionResult GetCapacityCertificateDuplicateApplication([FromQuery] int id)
        {
            if (applicationService.IsApplicationSubmittedByUserOrPerson(CurrentUser.ID, id))
            {
                CapacityCertificateDuplicateApplicationDTO result = service.GetCapacityCertificateDuplicateApplication(id);
                return Ok(result);
            }
            return NotFound();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsAddRecords)]
        public async Task<IActionResult> AddCapacityCertificateDuplicateApplication([FromForm] CapacityCertificateDuplicateApplicationDTO application)
        {
            IActionResult result = await CheckModel(application);

            if (result != null)
            {
                return result;
            }

            int id = service.AddCapacityCertificateDuplicateApplication(application, null);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsEditRecords)]
        public async Task<IActionResult> EditCapacityCertificateDuplicateApplication([FromForm] CapacityCertificateDuplicateApplicationDTO application)
        {
            if (application.ApplicationId.HasValue && applicationService.IsApplicationSubmittedByUser(CurrentUser.ID, application.ApplicationId.Value))
            {
                IActionResult result = await CheckModel(application);

                if (result != null)
                {
                    return result;
                }

                service.EditCapacityCertificateDuplicateApplication(application);
                return Ok();
            }

            return NotFound();
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
