using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.Common.Exceptions;
using IARA.DomainModels.DTOInterfaces;
using IARA.DomainModels.DTOModels.Duplicates;
using IARA.Interfaces;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Enums;
using Microsoft.AspNetCore.Mvc;

namespace IARA.WebAPI.Controllers.Public
{
    [AreaRoute(AreaType.Public)]
    public class DuplicatesRegisterPublicController : BaseController
    {
        private readonly IDuplicatesRegisterService service;
        private readonly IApplicationService applicationService;
        private readonly IDeliveryService deliveryService;

        public DuplicatesRegisterPublicController(IDuplicatesRegisterService service,
                                                  IApplicationService applicationService,
                                                  IDeliveryService deliveryService,
                                                  IPermissionsService permissionsService)
            : base(permissionsService)
        {
            this.service = service;
            this.applicationService = applicationService;
            this.deliveryService = deliveryService;
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead)]
        public IActionResult GetDuplicatesApplication([FromQuery] int applicationId)
        {
            if (applicationService.IsApplicationSubmittedByUserOrPerson(CurrentUser.ID, applicationId))
            {
                DuplicatesApplicationDTO result = service.GetDuplicatesApplication(applicationId);
                return Ok(result);
            }
            return NotFound();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetRegisterByApplicationId([FromQuery] int applicationId)
        {
            DuplicatesRegisterEditDTO duplicate = service.GetRegisterByApplicationId(applicationId);
            return Ok(duplicate);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsAddRecords)]
        public async Task<IActionResult> AddDuplicateApplication([FromForm] DuplicatesApplicationDTO duplicate)
        {
            IActionResult result = await CheckModel(duplicate);

            if (result != null)
            {
                return result;
            }

            try
            {
                int id = service.AddDuplicatesApplication(duplicate);
                return Ok(id);
            }
            catch (BuyerDoesNotExistsException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, ErrorCode.BuyerDoesNotExist);
            }
            catch (PermitDoesNotExistException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, ErrorCode.PermitDoesNotExist);
            }
            catch (PermitLicenceDoesNotExistException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, ErrorCode.PermitLicenceDoesNotExist);
            }
            catch (QualifiedFisherDoesNotExistException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, ErrorCode.QualifiedFisherDoesNotExist);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsEditRecords)]
        public async Task<IActionResult> EditDuplicateApplication([FromForm] DuplicatesApplicationDTO duplicate)
        {
            IActionResult result = await CheckModel(duplicate);

            if (result != null)
            {
                return result;
            }

            try
            {
                if (duplicate.ApplicationId.HasValue && applicationService.IsApplicationSubmittedByUser(CurrentUser.ID, duplicate.ApplicationId.Value))
                {
                    service.EditDuplicatesApplication(duplicate);
                }

                return NotFound();
            }
            catch (BuyerDoesNotExistsException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, ErrorCode.BuyerDoesNotExist);
            }
            catch (PermitDoesNotExistException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, ErrorCode.PermitDoesNotExist);
            }
            catch (PermitLicenceDoesNotExistException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, ErrorCode.PermitLicenceDoesNotExist);
            }
            catch (QualifiedFisherDoesNotExistException ex)
            {
                return ValidationFailedResult(new List<string> { ex.Message }, ErrorCode.QualifiedFisherDoesNotExist);
            }
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
