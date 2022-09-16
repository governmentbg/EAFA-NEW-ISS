using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.QualifiedFrishersRegister;
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
    public class QualifiedFishersPublicController : BaseController
    {
        private readonly IQualifiedFishersService service;
        private readonly IFileService fileService;
        private readonly IDeliveryService deliveryService;

        public QualifiedFishersPublicController(IQualifiedFishersService service,
                                          IFileService fileService,
                                          IPermissionsService permissionsService,
                                          IDeliveryService deliveryService)
            : base(permissionsService)
        {
            this.service = service;
            this.fileService = fileService;
            this.deliveryService = deliveryService;
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead)]
        public IActionResult GetApplication([FromQuery] int id)
        {
            QualifiedFisherApplicationEditDTO result = service.GetApplicationEntry(id);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetRegisterByApplicationId([FromQuery] int applicationId)
        {
            QualifiedFisherEditDTO fisher = service.GetRegisterByApplicationId(applicationId);
            return Ok(fisher);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsAddRecords)]
        public async Task<IActionResult> AddApplication([FromForm] QualifiedFisherApplicationEditDTO fisher)
        {
            EgnLncDTO fisherEgnLnc;
            RegixPersonDataDTO person;
            List<AddressRegistrationDTO> addresses;

            if (fisher.SubmittedByRole == SubmittedByRolesEnum.Personal)
            {
                fisherEgnLnc = fisher.SubmittedByRegixData.EgnLnc;
                person = fisher.SubmittedByRegixData;
                addresses = fisher.SubmittedByAddresses;
            }
            else
            {
                fisherEgnLnc = fisher.SubmittedForRegixData.EgnLnc;
                person = fisher.SubmittedForRegixData;
                addresses = fisher.SubmittedForAddresses;
            }

            if (!service.PersonIsQualifiedFisherCheck(fisherEgnLnc, fisher.Id))
            {
                bool hasEDelivery = await deliveryService.HasSubmittedForEDelivery(fisher.DeliveryData.DeliveryTypeId,
                                                                                   new ApplicationSubmittedForRegixDataDTO
                                                                                   {
                                                                                       Person = person,
                                                                                       Addresses = addresses,
                                                                                       SubmittedByRole = fisher.SubmittedByRole
                                                                                   },
                                                                                   new ApplicationSubmittedByRegixDataDTO
                                                                                   {
                                                                                       Person = fisher.SubmittedByRegixData,
                                                                                       Addresses = fisher.SubmittedByAddresses
                                                                                   });

                if (hasEDelivery)
                {
                    int id = service.AddApplicationEntry(fisher, null);
                    return Ok(id);
                }
                else
                {
                    return ValidationFailedResult(errorCode: ErrorCode.NoEDeliveryRegistration);
                }
            }
            else
            {
                return ValidationFailedResult(errorCode: ErrorCode.QualifiedFisherAlreadyExists);
            }
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsEditRecords)]
        public async Task<IActionResult> EditApplication([FromForm] QualifiedFisherApplicationEditDTO fisher)
        {
            EgnLncDTO fisherEgnLnc;
            RegixPersonDataDTO person;
            List<AddressRegistrationDTO> addresses;

            if (fisher.SubmittedByRole == SubmittedByRolesEnum.Personal)
            {
                fisherEgnLnc = fisher.SubmittedByRegixData.EgnLnc;
                person = fisher.SubmittedByRegixData;
                addresses = fisher.SubmittedByAddresses;
            }
            else
            {
                fisherEgnLnc = fisher.SubmittedForRegixData.EgnLnc;
                person = fisher.SubmittedForRegixData;
                addresses = fisher.SubmittedForAddresses;
            }

            if (!service.PersonIsQualifiedFisherCheck(fisherEgnLnc, fisher.Id))
            {
                bool hasEDelivery = await deliveryService.HasSubmittedForEDelivery(fisher.DeliveryData.DeliveryTypeId,
                                                                                   new ApplicationSubmittedForRegixDataDTO
                                                                                   {
                                                                                       SubmittedByRole = fisher.SubmittedByRole,
                                                                                       Person = person,
                                                                                       Addresses = addresses
                                                                                   },
                                                                                   new ApplicationSubmittedByRegixDataDTO
                                                                                   {
                                                                                       Person = fisher.SubmittedByRegixData,
                                                                                       Addresses = fisher.SubmittedByAddresses
                                                                                   });
                if (hasEDelivery)
                {
                    service.EditApplicationEntry(fisher);
                    return Ok();
                }
                else
                {
                    return ValidationFailedResult(errorCode: ErrorCode.NoEDeliveryRegistration);
                }
            }
            else
            {
                return ValidationFailedResult(errorCode: ErrorCode.QualifiedFisherAlreadyExists);
            }
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult DownloadFile(int id)
        {
            DownloadableFileDTO file = fileService.GetFileForDownload(id);
            return File(file.Bytes, file.MimeType, file.FileName);
        }
    }
}
