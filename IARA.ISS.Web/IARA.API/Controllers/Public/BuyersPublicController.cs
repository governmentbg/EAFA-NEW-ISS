using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.Common.Exceptions;
using IARA.DomainModels.DTOModels.Buyers;
using IARA.DomainModels.DTOModels.Buyers.Termination;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.Buyers.ChangeOfCircumstances;
using IARA.DomainModels.Nomenclatures;
using IARA.Interfaces;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Enums;
using Microsoft.AspNetCore.Mvc;

namespace IARA.Web.Controllers.Public
{
    // TODO add security checks
    [AreaRoute(AreaType.Public)]
    public class BuyersPublicController : BaseController
    {
        private readonly IBuyersService service;
        private readonly IFileService fileService;
        private readonly IApplicationService applicationService;
        private readonly IDeliveryService deliveryService;

        public BuyersPublicController(IBuyersService service,
                                      IFileService fileService,
                                      IPermissionsService permissionsService,
                                      IApplicationService applicationService,
                                      IDeliveryService deliveryService)
            : base(permissionsService)
        {
            this.service = service;
            this.fileService = fileService;
            this.applicationService = applicationService;
            this.deliveryService = deliveryService;
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead)]
        public IActionResult GetApplication([FromQuery] int id)
        {
            BuyerApplicationEditDTO result = service.GetApplicationEntry(id);
            return Ok(result);
        }


        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetRegisterByApplicationId([FromQuery] int applicationId)
        {
            BuyerEditDTO result = service.GetRegisterByApplicationId(applicationId);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetRegisterByChangeOfCircumstancesApplicationId([FromQuery] int applicationId)
        {
            BuyerEditDTO result = service.GetRegisterByChangeOfCircumstancesApplicationId(applicationId);
            return Ok(result);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsAddRecords)]
        public IActionResult AddApplication([FromForm] BuyerApplicationEditDTO application)
        {
            int id = service.AddApplicationEntry(application, null);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsEditRecords)]
        public IActionResult EditApplication([FromForm] BuyerApplicationEditDTO application)
        {
            service.EditApplicationEntry(application);
            return Ok();
        }

        // Change of circumstances
        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead)]
        public IActionResult GetBuyerChangeOfCircumstancesApplication([FromQuery] int id)
        {
            if (applicationService.IsApplicationSubmittedByUserOrPerson(CurrentUser.ID, id))
            {
                BuyerChangeOfCircumstancesApplicationDTO result = service.GetBuyerChangeOfCircumstancesApplication(id);
                return Ok(result);
            }

            return NotFound();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsAddRecords)]
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
                int id = service.AddBuyerChangeOfCircumstancesApplication(application);
                return Ok(id);
            }
            catch (BuyerDoesNotExistsException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, ErrorCode.BuyerDoesNotExist);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsEditRecords)]
        public async Task<IActionResult> EditBuyerChangeOfCircumstancesApplication([FromForm] BuyerChangeOfCircumstancesApplicationDTO application)
        {
            if (application.ApplicationId.HasValue && applicationService.IsApplicationSubmittedByUser(CurrentUser.ID, application.ApplicationId.Value))
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
                    service.EditBuyerChangeOfCircumstancesApplication(application);
                    return Ok();
                }
                catch (BuyerDoesNotExistsException ex)
                {
                    return ValidationFailedResult(new List<string> { ex.Message }, ErrorCode.BuyerDoesNotExist);
                }
            }

            return NotFound();
        }

        // Termination
        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead)]
        public IActionResult GetBuyerTerminationApplication([FromQuery] int id)
        {
            if (applicationService.IsApplicationSubmittedByUserOrPerson(CurrentUser.ID, id))
            {
                BuyerTerminationApplicationDTO result = service.GetBuyerTerminationApplication(id);
                return Ok(result);
            }

            return NotFound();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsAddRecords)]
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
                int id = service.AddBuyerTerminationApplication(application);
                return Ok(id);
            }
            catch (BuyerDoesNotExistsException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, ErrorCode.BuyerDoesNotExist);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsEditRecords)]
        public async Task<IActionResult> EditBuyerTerminationApplication([FromForm] BuyerTerminationApplicationDTO application)
        {
            if (application.ApplicationId.HasValue && applicationService.IsApplicationSubmittedByUser(CurrentUser.ID, application.ApplicationId.Value))
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
                    service.EditBuyerTerminationApplication(application);
                    return Ok();
                }
                catch (BuyerDoesNotExistsException ex)
                {
                    return ValidationFailedResult(new List<string> { ex.Message }, ErrorCode.BuyerDoesNotExist);
                }
            }

            return NotFound();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult DownloadFile(int id)
        {
            DownloadableFileDTO file = fileService.GetFileForDownload(id);
            return File(file.Bytes, file.MimeType, file.FileName);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetBuyerStatuses()
        {
            List<NomenclatureDTO> statuses = service.GetBuyerStatuses().ToList();
            return Ok(statuses);
        }
    }
}
