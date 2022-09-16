using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.ScientificFishing;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces;
using IARA.Interfaces.Nomenclatures;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Enums;
using IARA.WebHelpers.Models;
using Microsoft.AspNetCore.Mvc;

namespace IARA.Web.Controllers.Public
{
    [AreaRoute(AreaType.Public)]
    public class ScientificFishingPublicController : BaseController
    {
        private readonly IScientificFishingService service;
        private readonly IScientificFishingNomenclaturesService nomenclatures;
        private readonly IFileService fileService;
        private readonly IApplicationService applicationService;
        private readonly IDeliveryService deliveryService;

        public ScientificFishingPublicController(IScientificFishingService service,
                                                 IScientificFishingNomenclaturesService nomenclatures,
                                                 IFileService fileService,
                                                 IApplicationService applicationService,
                                                 IPermissionsService permissionsService,
                                                 IDeliveryService deliveryService)
            : base(permissionsService)
        {
            this.service = service;
            this.nomenclatures = nomenclatures;
            this.fileService = fileService;
            this.applicationService = applicationService;
            this.deliveryService = deliveryService;
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetAllPermits([FromBody] GridRequestModel<ScientificFishingPublicFilters> request)
        {
            IQueryable<ScientificFishingPermitDTO> permits = service.GetAllPermits(request.Filters, CurrentUser.ID);
            GridResultModel<ScientificFishingPermitDTO> result = new(permits, request, false);
            service.SetPermitHoldersForTable(result.Records);

            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetPermit([FromQuery] int id)
        {
            if (service.HasUserAccessToPermits(CurrentUser.ID, new List<int> { id }))
            {
                ScientificFishingPermitEditDTO permit = service.GetPermit(id, CurrentUser.ID);
                return Ok(permit);
            }
            return NotFound();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetPermitHolderPhoto([FromQuery] int holderId)
        {
            if (service.HasUserAccessToPermitHolder(CurrentUser.ID, holderId))
            {
                string photo = service.GetPermitHolderPhoto(holderId);
                return Ok(photo);
            }
            return NotFound();
        }

        [HttpGet]
        [CustomAuthorize()]
        public IActionResult GetCurrentUserAsSubmittedBy()
        {
            ApplicationSubmittedByDTO result = service.GetUserAsSubmittedBy(CurrentUser.ID);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetRegisterByApplicationId([FromQuery] int applicationId)
        {
            ScientificFishingPermitEditDTO permit = service.GetRegisterByApplicationId(applicationId);
            return Ok(permit);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ScientificFishingAddOutings)]
        public IActionResult AddOuting([FromBody] ScientificFishingOutingDTO outing)
        {
            if (service.HasUserAccessToPermits(CurrentUser.ID, new List<int> { outing.PermitId!.Value }))
            {
                int id = service.AddOuting(outing);
                return Ok(id);
            }
            return NotFound();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult DownloadFile(int id)
        {
            if (service.HasUserAccessToPermitFile(CurrentUser.ID, id))
            {
                DownloadableFileDTO file = fileService.GetFileForDownload(id);
                return File(file.Bytes, file.MimeType, file.FileName);
            }
            return NotFound();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetPermitReasons()
        {
            List<ScientificFishingReasonNomenclatureDTO> reasons = nomenclatures.GetPermitReasons();
            return Ok(reasons);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetPermitStatuses()
        {
            List<NomenclatureDTO> statuses = nomenclatures.GetPermitStatuses();
            return Ok(statuses);
        }

        // Applications
        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead)]
        public IActionResult GetPermitApplication([FromQuery] int id)
        {
            if (applicationService.IsApplicationSubmittedByUserOrPerson(CurrentUser.ID, id))
            {
                ScientificFishingApplicationEditDTO result = service.GetPermitApplication(id);
                return Ok(result);
            }
            return NotFound();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsAddRecords)]
        public async Task<IActionResult> AddPermitApplication([FromForm] ScientificFishingApplicationEditDTO permit)
        {
            bool hasDelivery = deliveryService.HasApplicationDelivery(permit.ApplicationId!.Value);
            if (hasDelivery != permit.HasDelivery!.Value)
            {
                throw new Exception("Mismatch between HasDelivery in model and in database");
            }

            if (hasDelivery)
            {
                ApplicationSubmittedForRegixDataDTO submittedFor = new ApplicationSubmittedForRegixDataDTO
                {
                    Legal = permit.Receiver,
                    Addresses = new List<AddressRegistrationDTO>(),
                    SubmittedByRole = permit.RequesterLetterOfAttorney != null
                        ? SubmittedByRolesEnum.LegalRepresentative
                        : SubmittedByRolesEnum.LegalOwner
                };

                ApplicationSubmittedByRegixDataDTO submittedBy = new ApplicationSubmittedByRegixDataDTO
                {
                    Person = permit.Requester,
                    Addresses = new List<AddressRegistrationDTO>()
                };

                bool hasEDelivery = await deliveryService.HasSubmittedForEDelivery(permit.DeliveryData.DeliveryTypeId,
                                                                                   submittedFor,
                                                                                   submittedBy);

                if (hasEDelivery == false)
                {
                    return ValidationFailedResult(new List<string> { nameof(ApplicationValidationErrorsEnum.NoEDeliveryRegistration) }, ErrorCode.NoEDeliveryRegistration);
                }
            }

            int id = service.AddPermitApplication(permit, null);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsEditRecords)]
        public async Task<IActionResult> EditPermitApplication([FromForm] ScientificFishingApplicationEditDTO permit)
        {
            bool hasDelivery = deliveryService.HasApplicationDelivery(permit.ApplicationId!.Value);
            if (hasDelivery != permit.HasDelivery!.Value)
            {
                throw new Exception("Mismatch between HasDelivery in model and in database");
            }

            if (hasDelivery)
            {
                ApplicationSubmittedForRegixDataDTO submittedFor = new ApplicationSubmittedForRegixDataDTO
                {
                    Legal = permit.Receiver,
                    Addresses = new List<AddressRegistrationDTO>(),
                    SubmittedByRole = permit.RequesterLetterOfAttorney != null
                        ? SubmittedByRolesEnum.LegalRepresentative
                        : SubmittedByRolesEnum.LegalOwner
                };

                ApplicationSubmittedByRegixDataDTO submittedBy = new ApplicationSubmittedByRegixDataDTO
                {
                    Person = permit.Requester,
                    Addresses = new List<AddressRegistrationDTO>()
                };

                bool hasEDelivery = await deliveryService.HasSubmittedForEDelivery(permit.DeliveryData.DeliveryTypeId,
                                                                                   submittedFor,
                                                                                   submittedBy);

                if (hasEDelivery == false)
                {
                    return ValidationFailedResult(new List<string> { nameof(ApplicationValidationErrorsEnum.NoEDeliveryRegistration) }, ErrorCode.NoEDeliveryRegistration);
                }
            }

            if (permit.ApplicationId.HasValue && applicationService.IsApplicationSubmittedByUser(CurrentUser.ID, permit.ApplicationId.Value))
            {
                service.EditPermitApplication(permit);
                return Ok();
            }

            return NotFound();
        }
    }
}
