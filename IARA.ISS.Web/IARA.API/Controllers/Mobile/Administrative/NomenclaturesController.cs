using System;
using System.Collections.Generic;
using IARA.Common.Enums;
using IARA.DomainModels.Nomenclatures;
using IARA.Interfaces.Nomenclatures;
using IARA.Security;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace IARA.Web.Controllers.Mobile.Administrative
{
    [AreaRoute(AreaType.MobileAdministrative)]
    public class NomenclaturesController : BaseController
    {
        private readonly IMobileNomenclaturesService service;

        public NomenclaturesController(IMobileNomenclaturesService service, IPermissionsService permissionsService)
            : base(permissionsService)
        {
            this.service = service;
        }

        [HttpGet]
        public IActionResult GetNomenclatureTables()
        {
            try
            {
                List<MobileNomenclatureDTO> result = this.service.GetNomenclatureTables(MobileTypeEnum.Insp);
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetFileTypes()
        {
            return this.Ok(this.service.GetFileTypes());
        }

        [HttpGet]
        public IActionResult GetCountries()
        {
            return this.Ok(this.service.GetCountries());
        }

        [HttpGet]
        public IActionResult GetDistricts()
        {
            return this.Ok(this.service.GetDistricts());
        }

        [HttpGet]
        public IActionResult GetDocumentTypes()
        {
            return this.Ok(this.service.GetDocumentTypes());
        }

        [HttpGet]
        public IActionResult GetMunicipalities()
        {
            return this.Ok(this.service.GetMunicipalities());
        }

        [HttpGet]
        public IActionResult GetObservationTools()
        {
            return this.Ok(this.service.GetObservationTools());
        }

        [HttpGet]
        public IActionResult GetPopulatedAreas()
        {
            return this.Ok(this.service.GetPopulatedAreas());
        }

        [HttpGet]
        public IActionResult GetInspectionStates()
        {
            return this.Ok(this.service.GetInspectionStates());
        }

        [HttpGet]
        public IActionResult GetInspectionTypes()
        {
            return this.Ok(this.service.GetInspectionTypes());
        }

        [HttpGet]
        public IActionResult GetInstitutions()
        {
            return this.Ok(this.service.GetInstitutions());
        }

        [HttpGet]
        public IActionResult GetPatrolVehicleTypes()
        {
            return this.Ok(this.service.GetPatrolVehicleTypes());
        }

        [HttpGet]
        public IActionResult GetVesselActivities()
        {
            return this.Ok(this.service.GetVesselActivities());
        }

        [HttpGet]
        public IActionResult GetVesselTypes()
        {
            return this.Ok(this.service.GetVesselTypes());
        }

        [HttpGet]
        public IActionResult GetRequiredFileTypes()
        {
            return this.Ok(this.service.GetRequiredFileTypes());
        }

        [HttpGet]
        public IActionResult GetShipAssociations()
        {
            return this.Ok(this.service.GetShipAssociations());
        }

        [HttpGet]
        public IActionResult GetInspectionCheckTypes()
        {
            return this.Ok(this.service.GetInspectionCheckTypes());
        }

        [HttpGet]
        public IActionResult GetCatchInspectionTypes()
        {
            return this.Ok(this.service.GetCatchInspectionTypes());
        }

        [HttpGet]
        public IActionResult GetFishes()
        {
            return this.Ok(this.service.GetFishes());
        }

        [HttpGet]
        public IActionResult GetFishingGears()
        {
            return this.Ok(this.service.GetFishingGears());
        }

        [HttpGet]
        public IActionResult GetInspectedPersonTypes()
        {
            return this.Ok(this.service.GetInspectedPersonTypes());
        }

        [HttpGet]
        public IActionResult GetCatchZones()
        {
            return this.Ok(this.service.GetCatchZones());
        }

        [HttpGet]
        public IActionResult GetWaterBodyTypes()
        {
            return this.Ok(this.service.GetWaterBodyTypes());
        }

        [HttpGet]
        public IActionResult GetPorts()
        {
            return this.Ok(this.service.GetPorts());
        }

        [HttpGet]
        public IActionResult GetFishingGearMarkStatuses()
        {
            return this.Ok(this.service.GetFishingGearMarkStatuses());
        }

        [HttpGet]
        public IActionResult GetFishingGearPingerStatuses()
        {
            return this.Ok(this.service.GetFishingGearPingerStatuses());
        }

        [HttpGet]
        public IActionResult GetTransportVehicleTypes()
        {
            return this.Ok(this.service.GetTransportVehicleTypes());
        }

        [HttpGet]
        public IActionResult GetGenders()
        {
            return this.Ok(this.service.GetGenders());
        }

        [HttpGet]
        public IActionResult GetPermitLicenseTypes()
        {
            return this.Ok(this.service.GetPermitLicenseTypes());
        }

        [HttpGet]
        public IActionResult GetFleetTypes()
        {
            return this.Ok(this.service.GetFleetTypes());
        }

        [HttpGet]
        public IActionResult GetFishPresentations()
        {
            return this.Ok(this.service.GetFishPresentations());
        }

        [HttpGet]
        public IActionResult GetFishSex()
        {
            return this.Ok(this.service.GetFishSex());
        }

        [HttpGet]
        public IActionResult GetFishingGearCheckReasons()
        {
            return this.Ok(this.service.GetFishingGearCheckReasons());
        }

        [HttpGet]
        public IActionResult GetFishingGearRecheckReasons()
        {
            return this.Ok(this.service.GetFishingGearRecheckReasons());
        }
    }
}
