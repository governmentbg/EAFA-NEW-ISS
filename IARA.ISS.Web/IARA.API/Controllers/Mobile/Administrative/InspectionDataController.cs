using System;
using System.Collections.Generic;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.ControlActivity.Inspections;
using IARA.DomainModels.DTOModels.Mobile.Inspections;
using IARA.DomainModels.DTOModels.Mobile.Ships;
using IARA.DomainModels.Nomenclatures;
using IARA.Interfaces.Mobile;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace IARA.WebAPI.Controllers.Mobile.Administrative
{
    [AreaRoute(AreaType.MobileAdministrative)]
    public class InspectionDataController : BaseController
    {
        private readonly IMobileInspectionsService service;

        public InspectionDataController(IMobileInspectionsService service, IPermissionsService permissionsService)
            : base(permissionsService)
        {
            this.service = service;
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult IsDeviceAllowed([FromQuery] string imei)
        {
            bool result = this.service.IsDeviceAllowed(imei);
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult GetInspectors([FromQuery] DateTime? afterDate = null)
        {
            List<InspectorDTO> result = this.service.GetInspectors(afterDate);
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult GetPoundNets([FromQuery] DateTime? afterDate = null)
        {
            List<NomenclatureDTO> result = this.service.GetPoundNets(afterDate);
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult GetPoundNetFishingGears([FromQuery] DateTime? afterDate = null)
        {
            List<FishingGearInspectionDTO> result = this.service.GetPoundNetFishingGears(afterDate);
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult GetPatrolVehicles([FromQuery] DateTime? afterDate = null)
        {
            List<PatrolVehicleNomenclatureDTO> result = this.service.GetPatrolVehicles(afterDate);
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult GetShips([FromQuery] DateTime? afterDate = null)
        {
            List<ShipMobileDTO> result = this.service.GetShips(afterDate);
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult GetPermitLicenses([FromQuery] DateTime? afterDate = null)
        {
            List<PermitLicenseMobileDTO> result = this.service.GetPermitLicenses(afterDate);
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult GetShipsOwners([FromQuery] DateTime? afterDate = null)
        {
            List<ShipOwnerMobileDTO> result = this.service.GetShipsOwners(afterDate);
            return this.Ok(result);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult GetPersons([FromBody] List<int> personIds)
        {
            List<PersonMobileDTO> result = this.service.GetPersons(personIds);
            return this.Ok(result);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult GetLegals([FromBody] List<int> legalIds)
        {
            List<LegalMobileDTO> result = this.service.GetLegals(legalIds);
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult GetShipsFishingGears([FromQuery] DateTime? afterDate = null)
        {
            List<FishingGearInspectionDTO> result = this.service.GetShipsFishingGears(afterDate);
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult GetFishingGearsMarks([FromQuery] DateTime? afterDate = null)
        {
            List<FishingGearMarkInspectionDTO> result = this.service.GetFishingGearsMarks(afterDate);
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult GetLogBooks([FromQuery] DateTime? afterDate = null)
        {
            List<LogBookMobileDTO> result = this.service.GetLogBooks(afterDate);
            return this.Ok(result);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult GetLogBookPages([FromBody] List<int> ids)
        {
            List<LogBookPageMobileDTO> result = this.service.GetLogBookPages(ids);
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult GetBuyers([FromQuery] DateTime? afterDate = null)
        {
            List<BuyerMobileDTO> result = this.service.GetBuyers(afterDate);
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult GetDeclarationLogBookPages([FromQuery] DeclarationLogBookTypeEnum type, [FromQuery] int shipUid)
        {
            List<DeclarationLogBookPageDTO> result = this.service.GetDeclarationLogBookPages(type, shipUid);
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult GetAquacultures([FromQuery] DateTime? afterDate = null)
        {
            List<AquacultureMobileDTO> result = this.service.GetAquacultures(afterDate);
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult GetPoundNetPermitLicenses([FromQuery] DateTime? afterDate = null)
        {
            List<PoundNetPermitLicenseDTO> result = this.service.GetPoundNetPermitLicenses(afterDate);
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult GetPoundNetFishingGearsMarks([FromQuery] DateTime? afterDate = null)
        {
            List<FishingGearMarkInspectionDTO> result = this.service.GetPoundNetFishingGearsMarks(afterDate);
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult GetPoundNetFishingGearsPingers([FromQuery] DateTime? afterDate = null)
        {
            List<FishingGearPingerInspectionDTO> result = this.service.GetPoundNetFishingGearsPingers(afterDate);
            return this.Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.InspectionsRead)]
        public IActionResult GetFishingGearsPingers([FromQuery] DateTime? afterDate = null)
        {
            List<FishingGearPingerInspectionDTO> result = this.service.GetFishingGearsPingers(afterDate);
            return this.Ok(result);
        }
    }
}
