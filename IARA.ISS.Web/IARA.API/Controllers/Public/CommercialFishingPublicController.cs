using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Application;
using IARA.DomainModels.DTOModels.CommercialFishingRegister;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.DTOModels.QualifiedFrishersRegister;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces;
using IARA.Interfaces.CommercialFishing;
using IARA.Interfaces.Nomenclatures;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Enums;
using Microsoft.AspNetCore.Mvc;
using TL.Caching.Interfaces;

namespace IARA.WebAPI.Controllers.Public
{
    // TODO add security checks
    [AreaRoute(AreaType.Public)]
    public class CommercialFishingPublicController : BaseController
    {
        private readonly ICommercialFishingService service;
        private readonly ICommercialFishingNomenclaturesService nomenclaturesService;
        private readonly IFileService fileService;
        private readonly IShipsRegisterNomenclaturesService shipNomenclaturesService;
        private readonly IMemoryCacheService memoryCacheService;

        private static readonly PageCodeEnum[] PERMIT_LICENSES_PAGE_CODES = new PageCodeEnum[]
        {
            PageCodeEnum.RightToFishResource,
            PageCodeEnum.PoundnetCommFishLic,
            PageCodeEnum.CatchQuataSpecies
        };

        public CommercialFishingPublicController(IPermissionsService permissionsService,
                                                 ICommercialFishingService commercialFishingService,
                                                 ICommercialFishingNomenclaturesService commercialFishingNomenclaturesService,
                                                 IFileService fileService,
                                                 IShipsRegisterNomenclaturesService shipNomenclaturesService,
                                                 IMemoryCacheService memoryCacheService)
            : base(permissionsService)
        {
            this.service = commercialFishingService;
            this.nomenclaturesService = commercialFishingNomenclaturesService;
            this.fileService = fileService;
            this.memoryCacheService = memoryCacheService;
            this.shipNomenclaturesService = shipNomenclaturesService;
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetPermitRegisterByApplicationId([FromQuery] int applicationId)
        {
            CommercialFishingEditDTO permit = service.GetPermitByApplicationId(applicationId);

            return Ok(permit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetPermitLicenseRegisterByApplicationId([FromQuery] int applicationId)
        {
            CommercialFishingEditDTO permit = service.GetPermitLicenseByApplicationId(applicationId);

            return Ok(permit);
        }

        // Applications
        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead)]
        public IActionResult GetPermitApplication([FromQuery] int id)
        {
            CommercialFishingApplicationEditDTO result = service.GetPermitApplication(id);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead)]
        public IActionResult GetPermitLicenseApplication([FromQuery] int id)
        {
            CommercialFishingApplicationEditDTO result = service.GetPermitLicenseApplication(id);
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead)]
        public IActionResult GetPermitLicensesForRenewal([FromQuery] string permitNumber, [FromQuery] PageCodeEnum pageCode)
        {
            if (PERMIT_LICENSES_PAGE_CODES.Contains(pageCode))
            {
                List<PermitLicenseForRenewalDTO> permitLicenses = service.GetPermitLicensesForRenewal(permitNumber, pageCode);
                return Ok(permitLicenses);
            }
            else
            {
                return BadRequest("Invalid pageCode");
            }
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead)]
        public IActionResult GetPermitLicenseData([FromQuery] int permitLicenseId)
        {
            CommercialFishingApplicationEditDTO permitLicense = service.GetPermitLicenseForRenewal(permitLicenseId);
            return Ok(permitLicense);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead)]
        public IActionResult GetPermitLicenseApplicationDataFromPermit([FromQuery] string permitNumber, [FromQuery] int applicationId)
        {
            CommercialFishingApplicationEditDTO result = service.GetPermitLicenseApplicationDataFromPermit(permitNumber, applicationId);
            return Ok(result);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead)]
        public IActionResult CalculatePermitLicenseAppliedTariffs([FromBody] PermitLicenseTariffCalculationParameters tariffCalculationParameters)
        {
            List<PaymentTariffDTO> appliedTariffs = service.CalculatePermitLicenseAppliedTariffs(tariffCalculationParameters);
            return Ok(appliedTariffs);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsAddRecords)]
        public async Task<IActionResult> AddPermitApplication([FromForm] CommercialFishingApplicationEditDTO permit)
        {
            QualifiedFisherBasicDataDTO qualifiedFisher = new QualifiedFisherBasicDataDTO
            {
                Id = permit.QualifiedFisherId,
                Identifier = permit.QualifiedFisherIdentifier,
                FirstName = permit.QualifiedFisherFirstName,
                MiddleName = permit.QualifiedFisherMiddleName,
                LastName = permit.QualifiedFisherLastName
            };

            List<CommercialFishingValidationErrorsEnum> validationErrors = await service.ValidateApplicationData(permit.PageCode!.Value,
                                                                                                                 permit.SubmittedFor,
                                                                                                                 permit.SubmittedBy,
                                                                                                                 qualifiedFisher,
                                                                                                                 permit.ShipId!.Value,
                                                                                                                 permit.DeliveryData.DeliveryTypeId,
                                                                                                                 permit.WaterTypeId!.Value,
                                                                                                                 permit.IsHolderShipOwner);

            if (validationErrors.Count != 0)
            {
                return ValidationFailedResult(validationErrors.Select(x => x.ToString()).ToList(), errorCode: ErrorCode.InvalidData);
            }

            int id = service.AddPermitApplication(permit, null);
            ForceRefreshShipsNomenclature();

            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsAddRecords)]
        public async Task<IActionResult> AddPermitApplicationAndStartPermitLicenseApplication([FromForm] CommercialFishingApplicationEditDTO permit)
        {
            QualifiedFisherBasicDataDTO qualifiedFisher = new QualifiedFisherBasicDataDTO
            {
                Id = permit.QualifiedFisherId,
                Identifier = permit.QualifiedFisherIdentifier,
                FirstName = permit.QualifiedFisherFirstName,
                MiddleName = permit.QualifiedFisherMiddleName,
                LastName = permit.QualifiedFisherLastName
            };
            List<CommercialFishingValidationErrorsEnum> validationErrors = await service.ValidateApplicationData(permit.PageCode!.Value,
                                                                                                                 permit.SubmittedFor,
                                                                                                                 permit.SubmittedBy,
                                                                                                                 qualifiedFisher,
                                                                                                                 permit.ShipId!.Value,
                                                                                                                 permit.DeliveryData.DeliveryTypeId,
                                                                                                                 permit.WaterTypeId!.Value,
                                                                                                                 permit.IsHolderShipOwner);

            if (validationErrors.Count != 0)
            {
                return ValidationFailedResult(validationErrors.Select(x => x.ToString()).ToList(), errorCode: ErrorCode.InvalidData);
            }

            CommercialFishingApplicationEditDTO permitLicense = service.AddPermitApplicationAndStartPermitLicenseApplication(permit, CurrentUser.ID, null);
            ForceRefreshShipsNomenclature();

            return Ok(permitLicense);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsAddRecords)]
        [RequestFormLimits(ValueCountLimit = 5000)]
        public async Task<IActionResult> AddPermitLicenseApplication([FromForm] CommercialFishingApplicationEditDTO permitLicense)
        {
            QualifiedFisherBasicDataDTO qualifiedFisher = new QualifiedFisherBasicDataDTO
            {
                Id = permitLicense.QualifiedFisherId,
                Identifier = permitLicense.QualifiedFisherIdentifier,
                FirstName = permitLicense.QualifiedFisherFirstName,
                MiddleName = permitLicense.QualifiedFisherMiddleName,
                LastName = permitLicense.QualifiedFisherLastName
            };

            List<CommercialFishingValidationErrorsEnum> validationErrors = await service.ValidateApplicationData(permitLicense.PageCode!.Value,
                                                                                                                 permitLicense.SubmittedFor,
                                                                                                                 permitLicense.SubmittedBy,
                                                                                                                 qualifiedFisher,
                                                                                                                 permitLicense.ShipId!.Value,
                                                                                                                 permitLicense.DeliveryData.DeliveryTypeId,
                                                                                                                 permitLicense.WaterTypeId!.Value,
                                                                                                                 permitLicense.IsHolderShipOwner!.Value,
                                                                                                                 permitLicense.PermitLicensePermitNumber);

            if (validationErrors.Count != 0)
            {
                return ValidationFailedResult(validationErrors.Select(x => x.ToString()).ToList(), errorCode: ErrorCode.InvalidData);
            }

            int id = service.AddPermitLicenseApplication(permitLicense, true, null);
            ForceRefreshShipsNomenclature();

            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsEditRecords)]
        public async Task<IActionResult> EditPermitApplication([FromForm] CommercialFishingApplicationEditDTO permit)
        {
            QualifiedFisherBasicDataDTO qualifiedFisher = new QualifiedFisherBasicDataDTO
            {
                Id = permit.QualifiedFisherId,
                Identifier = permit.QualifiedFisherIdentifier,
                FirstName = permit.QualifiedFisherFirstName,
                MiddleName = permit.QualifiedFisherMiddleName,
                LastName = permit.QualifiedFisherLastName
            };
            List<CommercialFishingValidationErrorsEnum> validationErrors = await service.ValidateApplicationData(permit.PageCode!.Value,
                                                                                                                 permit.SubmittedFor,
                                                                                                                 permit.SubmittedBy,
                                                                                                                 qualifiedFisher,
                                                                                                                 permit.ShipId!.Value,
                                                                                                                 permit.DeliveryData.DeliveryTypeId,
                                                                                                                 permit.WaterTypeId!.Value,
                                                                                                                 permit.IsHolderShipOwner);

            if (validationErrors.Count != 0)
            {
                return ValidationFailedResult(validationErrors.Select(x => x.ToString()).ToList(), errorCode: ErrorCode.InvalidData);
            }

            service.EditPermitApplication(permit);

            ForceRefreshShipsNomenclature();

            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsEditRecords)]
        [RequestFormLimits(ValueCountLimit = 5000)]
        public async Task<IActionResult> EditPermitLicenseApplication([FromForm] CommercialFishingApplicationEditDTO permitLicense)
        {
            QualifiedFisherBasicDataDTO qualifiedFisher = new QualifiedFisherBasicDataDTO
            {
                Id = permitLicense.QualifiedFisherId,
                Identifier = permitLicense.QualifiedFisherIdentifier,
                FirstName = permitLicense.QualifiedFisherFirstName,
                MiddleName = permitLicense.QualifiedFisherMiddleName,
                LastName = permitLicense.QualifiedFisherLastName
            };

            List<CommercialFishingValidationErrorsEnum> validationErrors = await service.ValidateApplicationData(permitLicense.PageCode!.Value,
                                                                                                                 permitLicense.SubmittedFor,
                                                                                                                 permitLicense.SubmittedBy,
                                                                                                                 qualifiedFisher,
                                                                                                                 permitLicense.ShipId!.Value,
                                                                                                                 permitLicense.DeliveryData.DeliveryTypeId,
                                                                                                                 permitLicense.WaterTypeId!.Value,
                                                                                                                 permitLicense.IsHolderShipOwner!.Value,
                                                                                                                 permitLicense.PermitLicensePermitNumber);

            if (validationErrors.Count != 0)
            {
                return ValidationFailedResult(validationErrors.Select(x => x.ToString()).ToList(), errorCode: ErrorCode.InvalidData);
            }

            service.EditPermitLicenseApplication(permitLicense, true);

            ForceRefreshShipsNomenclature();

            return Ok();
        }

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
        public IActionResult GetCommercialFishingPermitTypes()
        {
            List<NomenclatureDTO> types = nomenclaturesService.GetCommercialFishingPermitTypes();
            return Ok(types);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetCommercialFishingPermitLicenseTypes()
        {
            List<NomenclatureDTO> types = nomenclaturesService.GetCommercialFishingPermitLicenseTypes();
            return Ok(types);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetWaterTypes()
        {
            List<NomenclatureDTO> waterTypes = nomenclaturesService.GetWaterTypes();
            return Ok(waterTypes);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetHolderGroundForUseTypes()
        {
            List<NomenclatureDTO> groundForUseTypes = nomenclaturesService.GetHolderGroundForUseTypes();
            return Ok(groundForUseTypes);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetPoundNets()
        {
            List<PoundNetNomenclatureDTO> poundNets = nomenclaturesService.GetPoundNets();
            return Ok(poundNets);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetPorts()
        {
            List<NomenclatureDTO> ports = shipNomenclaturesService.GetPorts();
            return Ok(ports);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetSuspensionTypes()
        {
            List<SuspensionTypeNomenclatureDTO> suspensionTypes = nomenclaturesService.GetSuspensionTypes();
            return Ok(suspensionTypes);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetSuspensionReasons()
        {
            List<SuspensionReasonNomenclatureDTO> suspensionReasons = nomenclaturesService.GetSuspensionReasons();
            return Ok(suspensionReasons);
        }

        private void ForceRefreshShipsNomenclature()
        {
            memoryCacheService.ForceRefresh<ICommonNomenclaturesService, List<ShipNomenclatureDTO>>("GetShips", (service) => service.GetShips().ToList());
        }
    }
}
