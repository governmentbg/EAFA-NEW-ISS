using System;
using System.Collections.Generic;
using IARA.Common.Enums;
using IARA.DomainModels.Nomenclatures;
using IARA.Interfaces.Nomenclatures;
using IARA.Security;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace IARA.Web.Controllers.Mobile.Public
{
    [AreaRoute(AreaType.MobilePublic)]
    public class NomenclaturesController : BaseController
    {
        private readonly IMobileNomenclaturesService mobileNomenclaturesService;

        public NomenclaturesController(IMobileNomenclaturesService mobileNomenclaturesService, IPermissionsService permissionsService)
            : base(permissionsService)
        {
            this.mobileNomenclaturesService = mobileNomenclaturesService;
        }

        [HttpGet]
        public IActionResult GetNomenclatureTables()
        {
            try
            {
                List<MobileNomenclatureDTO> result = mobileNomenclaturesService.GetNomenclatureTables(MobileTypeEnum.Pub);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public IActionResult GetCountries()
        {
            return Ok(mobileNomenclaturesService.GetCountries());
        }

        [HttpGet]
        public IActionResult GetDistricts()
        {
            return Ok(mobileNomenclaturesService.GetDistricts());
        }

        [HttpGet]
        public IActionResult GetDocumentTypes()
        {
            return Ok(mobileNomenclaturesService.GetDocumentTypes());
        }

        [HttpGet]
        public IActionResult GetFishes()
        {
            return Ok(mobileNomenclaturesService.GetFishes());
        }

        [HttpGet]
        public IActionResult GetPermitReasons()
        {
            return Ok(mobileNomenclaturesService.GetPermitReasons());
        }

        [HttpGet]
        public IActionResult GetFileTypes()
        {
            return Ok(mobileNomenclaturesService.GetFileTypes());
        }

        [HttpGet]
        public IActionResult GetMunicipalities()
        {
            return Ok(mobileNomenclaturesService.GetMunicipalities());
        }

        [HttpGet]
        public IActionResult GetPopulatedAreas()
        {
            return Ok(mobileNomenclaturesService.GetPopulatedAreas());
        }

        [HttpGet]
        public IActionResult GetTicketTypes()
        {
            return Ok(mobileNomenclaturesService.GetTicketTypes());
        }

        [HttpGet]
        public IActionResult GetTicketPeriods()
        {
            return Ok(mobileNomenclaturesService.GetTicketPeriods());
        }

        [HttpGet]
        public IActionResult GetTicketTariffs()
        {
            return Ok(mobileNomenclaturesService.GetTicketTariffs());
        }

        [HttpGet]
        public IActionResult GetViolationSignalTypes()
        {
            return Ok(mobileNomenclaturesService.GetViolationSignalTypes());
        }

        [HttpGet]
        public IActionResult GetMobileVersions()
        {
            return Ok(mobileNomenclaturesService.GetMobileVersions());
        }

        [HttpGet]
        public IActionResult GetGenders()
        {
            return Ok(mobileNomenclaturesService.GetGenders());
        }

        [HttpGet]
        public IActionResult GetSystemParameters()
        {
            return Ok(mobileNomenclaturesService.GetSystemParameters());
        }

        [HttpGet]
        public IActionResult GetPaymentTypes()
        {
            return Ok(mobileNomenclaturesService.GetPaymentTypes());
        }
    }
}
