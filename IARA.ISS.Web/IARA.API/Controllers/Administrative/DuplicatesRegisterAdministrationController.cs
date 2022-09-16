using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.DomainModels.DTOInterfaces;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.CatchesAndSales;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Duplicates;
using IARA.DomainModels.Nomenclatures;
using IARA.Interfaces;
using IARA.Interfaces.CatchSales;
using IARA.Interfaces.Nomenclatures;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Enums;
using Microsoft.AspNetCore.Mvc;

namespace IARA.WebAPI.Controllers.Administrative
{
    [AreaRoute(AreaType.Administrative)]
    public class DuplicatesRegisterAdministrationController : BaseAuditController
    {
        private readonly IDuplicatesRegisterService service;
        private readonly IDeliveryService deliveryService;
        private readonly ILogBookNomenclaturesService logBookNomenclaturesService;
        private readonly ICommercialFishingNomenclaturesService commercialFishingNomenclaturesService;

        public DuplicatesRegisterAdministrationController(IDuplicatesRegisterService service,
                                                          IDeliveryService deliveryService,
                                                          IPermissionsService permissionsService,
                                                          ILogBookNomenclaturesService logBookNomenclaturesService,
                                                          ICommercialFishingNomenclaturesService commercialFishingNomenclaturesService)
            : base(permissionsService)
        {
            this.service = service;
            this.deliveryService = deliveryService;
            this.logBookNomenclaturesService = logBookNomenclaturesService;
            this.commercialFishingNomenclaturesService = commercialFishingNomenclaturesService;
        }

        [HttpGet]
        [CustomAuthorize(Permissions.BuyersApplicationsReadAll,
                         Permissions.BuyersApplicationsRead,
                         Permissions.CommercialFishingPermitApplicationsReadAll,
                         Permissions.CommercialFishingPermitApplicationsRead,
                         Permissions.CommercialFishingPermitLicenseApplicationsReadAll,
                         Permissions.CommercialFishingPermitLicenseApplicationsRead,
                         Permissions.QualifiedFishersApplicationsReadAll,
                         Permissions.QualifiedFishersApplicationsRead)]
        public IActionResult GetDuplicatesApplication([FromQuery] int applicationId)
        {
            DuplicatesApplicationDTO duplicate = service.GetDuplicatesApplication(applicationId);
            return Ok(duplicate);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.BuyersApplicationsInspectAndCorrectRegiXData,
                         Permissions.CommercialFishingPermitApplicationsInspectAndCorrectRegiXData,
                         Permissions.CommercialFishingPermitLicenseApplicationsInspectAndCorrectRegiXData,
                         Permissions.QualifiedFishersApplicationsInspectAndCorrectRegiXData)]
        public IActionResult GetDuplicatesRegixData([FromQuery] int applicationId)
        {
            RegixChecksWrapperDTO<DuplicatesApplicationRegixDataDTO> result = service.GetDuplicatesRegixData(applicationId);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.BuyersApplicationsReadAll,
                         Permissions.BuyersApplicationsRead,
                         Permissions.CommercialFishingPermitApplicationsReadAll,
                         Permissions.CommercialFishingPermitApplicationsRead,
                         Permissions.CommercialFishingPermitLicenseApplicationsReadAll,
                         Permissions.CommercialFishingPermitLicenseApplicationsRead,
                         Permissions.QualifiedFishersApplicationsReadAll,
                         Permissions.QualifiedFishersApplicationsRead)]
        public IActionResult GetApplicationDataForRegister([FromQuery] int applicationId)
        {
            DuplicatesRegisterEditDTO duplicate = service.GetApplicationDataForRegister(applicationId);
            return Ok(duplicate);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.BuyersApplicationsReadAll,
                         Permissions.BuyersApplicationsRead,
                         Permissions.CommercialFishingPermitApplicationsReadAll,
                         Permissions.CommercialFishingPermitApplicationsRead,
                         Permissions.CommercialFishingPermitLicenseApplicationsReadAll,
                         Permissions.CommercialFishingPermitLicenseApplicationsRead,
                         Permissions.QualifiedFishersApplicationsReadAll,
                         Permissions.QualifiedFishersApplicationsRead)]
        public IActionResult GetRegisterByApplicationId([FromQuery] int applicationId)
        {
            DuplicatesRegisterEditDTO duplicate = service.GetRegisterByApplicationId(applicationId);
            return Ok(duplicate);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.BuyersApplicationsAddRecords,
                         Permissions.CommercialFishingPermitApplicationsAddRecords,
                         Permissions.CommercialFishingPermitLicenseApplicationsAddRecords,
                         Permissions.QualifiedFishersApplicationsAddRecords)]
        public async Task<IActionResult> AddDuplicateApplication([FromForm] DuplicatesApplicationDTO duplicate)
        {
            IActionResult result = await CheckModel(duplicate);

            if (result != null)
            {
                return result;
            }

            int id = service.AddDuplicatesApplication(duplicate, ApplicationStatusesEnum.EXT_CHK_STARTED);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.BuyersApplicationsEditRecords,
                         Permissions.CommercialFishingPermitApplicationsEditRecords,
                         Permissions.CommercialFishingPermitLicenseApplicationsEditRecords,
                         Permissions.QualifiedFishersApplicationsEditRecords)]
        public async Task<IActionResult> EditDuplicateApplication([FromQuery] bool saveAsDraft, [FromForm] DuplicatesApplicationDTO duplicate)
        {
            IActionResult result = await CheckModel(duplicate);

            if (result != null)
            {
                return result;
            }

            if (saveAsDraft)
            {
                service.EditDuplicatesApplication(duplicate);
            }
            else
            {
                service.EditDuplicatesApplication(duplicate, ApplicationStatusesEnum.EXT_CHK_STARTED);
            }

            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.BuyersApplicationsInspectAndCorrectRegiXData,
                         Permissions.CommercialFishingPermitApplicationsInspectAndCorrectRegiXData,
                         Permissions.CommercialFishingPermitLicenseApplicationsInspectAndCorrectRegiXData,
                         Permissions.QualifiedFishersApplicationsInspectAndCorrectRegiXData)]
        public IActionResult EditDuplicatesApplicationAndStartRegixChecks([FromForm] DuplicatesApplicationRegixDataDTO duplicate)
        {
            service.EditDuplicatesRegixData(duplicate);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.BuyersApplicationsReadAll,
                         Permissions.BuyersApplicationsRead,
                         Permissions.CommercialFishingPermitApplicationsReadAll,
                         Permissions.CommercialFishingPermitApplicationsRead,
                         Permissions.CommercialFishingPermitLicenseApplicationsReadAll,
                         Permissions.CommercialFishingPermitLicenseApplicationsRead,
                         Permissions.QualifiedFishersApplicationsReadAll,
                         Permissions.QualifiedFishersApplicationsRead)]
        public IActionResult GetDuplicateRegister([FromQuery] int id)
        {
            DuplicatesRegisterEditDTO duplicate = service.GetDuplicateRegister(id);
            return Ok(duplicate);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.BuyersApplicationsAddRecords,
                         Permissions.CommercialFishingPermitApplicationsAddRecords,
                         Permissions.CommercialFishingPermitLicenseApplicationsAddRecords,
                         Permissions.QualifiedFishersApplicationsAddRecords)]
        public IActionResult AddDuplicateRegister([FromForm] DuplicatesRegisterEditDTO duplicate)
        {
            int id = service.AddDuplicateRegister(duplicate);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.BuyersApplicationsAddRecords,
                         Permissions.CommercialFishingPermitApplicationsAddRecords,
                         Permissions.CommercialFishingPermitLicenseApplicationsAddRecords,
                         Permissions.QualifiedFishersApplicationsAddRecords)]
        public async Task<IActionResult> AddAndDownloadDuplicateRegister([FromForm] DuplicatesRegisterEditDTO duplicate)
        {
            int id = service.AddDuplicateRegister(duplicate);

            byte[] file = await service.DownloadDuplicateRegister(id);
            return File(file, "application/pdf");
        }

        [HttpGet]
        [CustomAuthorize(Permissions.BuyersApplicationsReadAll,
                         Permissions.BuyersApplicationsRead,
                         Permissions.CommercialFishingPermitApplicationsReadAll,
                         Permissions.CommercialFishingPermitApplicationsRead,
                         Permissions.CommercialFishingPermitLicenseApplicationsReadAll,
                         Permissions.CommercialFishingPermitLicenseApplicationsRead,
                         Permissions.QualifiedFishersApplicationsReadAll,
                         Permissions.QualifiedFishersApplicationsRead)]
        public async Task<IActionResult> DownloadDuplicateRegister([FromQuery] int id)
        {
            byte[] file = await service.DownloadDuplicateRegister(id);
            return File(file, "application/pdf");
        }

        [HttpGet]
        [CustomAuthorize(Permissions.BuyersApplicationsReadAll,
                         Permissions.BuyersApplicationsRead,
                         Permissions.CommercialFishingPermitApplicationsReadAll,
                         Permissions.CommercialFishingPermitApplicationsRead,
                         Permissions.CommercialFishingPermitLicenseApplicationsReadAll,
                         Permissions.CommercialFishingPermitLicenseApplicationsRead,
                         Permissions.QualifiedFishersApplicationsReadAll,
                         Permissions.QualifiedFishersApplicationsRead)]
        public IActionResult GetRegisteredBuyers()
        {
            List<NomenclatureDTO> buyers = logBookNomenclaturesService.GetRegisteredBuyers();
            return Ok(buyers);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.BuyersApplicationsReadAll,
                         Permissions.BuyersApplicationsRead,
                         Permissions.CommercialFishingPermitApplicationsReadAll,
                         Permissions.CommercialFishingPermitApplicationsRead,
                         Permissions.CommercialFishingPermitLicenseApplicationsReadAll,
                         Permissions.CommercialFishingPermitLicenseApplicationsRead,
                         Permissions.QualifiedFishersApplicationsReadAll,
                         Permissions.QualifiedFishersApplicationsRead)]
        public IActionResult GetPermits()
        {
            List<NomenclatureDTO> permitLicenses = logBookNomenclaturesService.GetPermits();
            return Ok(permitLicenses);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.BuyersApplicationsReadAll,
                         Permissions.BuyersApplicationsRead,
                         Permissions.CommercialFishingPermitApplicationsReadAll,
                         Permissions.CommercialFishingPermitApplicationsRead,
                         Permissions.CommercialFishingPermitLicenseApplicationsReadAll,
                         Permissions.CommercialFishingPermitLicenseApplicationsRead,
                         Permissions.QualifiedFishersApplicationsReadAll,
                         Permissions.QualifiedFishersApplicationsRead)]
        public IActionResult GetPermitLicenses()
        {
            List<PermitLicenseNomenclatureDTO> permitLicenses = logBookNomenclaturesService.GetPermitLicenses();
            return Ok(permitLicenses);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.BuyersApplicationsReadAll,
                         Permissions.BuyersApplicationsRead,
                         Permissions.CommercialFishingPermitApplicationsReadAll,
                         Permissions.CommercialFishingPermitApplicationsRead,
                         Permissions.CommercialFishingPermitLicenseApplicationsReadAll,
                         Permissions.CommercialFishingPermitLicenseApplicationsRead,
                         Permissions.QualifiedFishersApplicationsReadAll,
                         Permissions.QualifiedFishersApplicationsRead)]
        public IActionResult GetQualifiedFishers()
        {
            List<QualifiedFisherNomenclatureDTO> fishers = commercialFishingNomenclaturesService.GetQualifiedFishers();
            return Ok(fishers);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.BuyersApplicationsReadAll,
                         Permissions.BuyersApplicationsRead,
                         Permissions.CommercialFishingPermitApplicationsReadAll,
                         Permissions.CommercialFishingPermitApplicationsRead,
                         Permissions.CommercialFishingPermitLicenseApplicationsReadAll,
                         Permissions.CommercialFishingPermitLicenseApplicationsRead,
                         Permissions.QualifiedFishersApplicationsReadAll,
                         Permissions.QualifiedFishersApplicationsRead)]
        public override IActionResult GetAuditInfo([FromQuery] int id)
        {
            SimpleAuditDTO audit = service.GetSimpleAudit(id);
            return Ok(audit);
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
