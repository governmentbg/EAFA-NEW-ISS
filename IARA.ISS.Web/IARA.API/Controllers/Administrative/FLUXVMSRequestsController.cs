using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IARA.DomainModels.DTOModels.FLUXVMSRequests;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Models;
using Microsoft.AspNetCore.Mvc;

namespace IARA.WebAPI.Controllers.Administrative
{
    [AreaRoute(AreaType.Administrative)]
    public class FLUXVMSRequestsController : BaseController
    {
        private readonly IFLUXVMSRequestsService service;

        public FLUXVMSRequestsController(IFLUXVMSRequestsService service,
                                         IPermissionsService permissionsService)
            : base(permissionsService)
        {
            this.service = service;
        }

        [HttpPost]
        [CustomAuthorize(Permissions.FLUXVMSRequestsRead)]
        public IActionResult GetAll([FromBody] GridRequestModel<FLUXVMSRequestFilters> request)
        {
            IQueryable<FLUXVMSRequestDTO> result = service.GetAll(request.Filters);
            return PageResult(result, request, false);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FLUXVMSRequestsRead)]
        public IActionResult Get([FromQuery] int id)
        {
            FLUXVMSRequestEditDTO result = service.Get(id);
            return Ok(result);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.FLUXVMSRequestsRead)]
        public IActionResult GetAllFlapRequests([FromBody] GridRequestModel<FluxFlapRequestFilters> request)
        {
            IQueryable<FluxFlapRequestDTO> result = service.GetAllFlapRequests(request.Filters);
            return PageResult(result, request, false);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FLUXVMSRequestsRead)]
        public IActionResult GetFlapRequest([FromQuery] int id)
        {
            FluxFlapRequestEditDTO request = service.GetFlapRequest(id);
            return Ok(request);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.FLUXVMSRequestsAddRecords)]
        public IActionResult AddFlapRequest([FromBody] FluxFlapRequestEditDTO request)
        {
            service.AddFlapRequest(request);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FLUXVMSRequestsRead)]
        public IActionResult GetFlapRequestAudit([FromQuery] int id)
        {
            SimpleAuditDTO audit = service.GetFlapRequestAudit(id);
            return Ok(audit);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FLUXVMSRequestsRead)]
        public IActionResult GetAgreementTypes()
        {
            List<NomenclatureDTO> result = service.GetAgreementTypes();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FLUXVMSRequestsRead)]
        public IActionResult GetCoastalParties()
        {
            List<NomenclatureDTO> result = service.GetCoastalParties();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FLUXVMSRequestsRead)]
        public IActionResult GetRequestPurposes()
        {
            List<NomenclatureDTO> result = service.GetRequestPurposes();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FLUXVMSRequestsRead)]
        public IActionResult GetFishingCategories()
        {
            List<NomenclatureDTO> result = service.GetFishingCategories();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.FLUXVMSRequestsRead)]
        public IActionResult GetFlapQuotaTypes()
        {
            List<NomenclatureDTO> result = service.GetFlapQuotaTypes();
            return Ok(result);
        }
    }
}
