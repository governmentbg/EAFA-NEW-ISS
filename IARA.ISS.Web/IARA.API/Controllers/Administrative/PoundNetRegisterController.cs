using System.Collections.Generic;
using System.Linq;
using IARA.DomainModels.DTOModels;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces;
using IARA.Interfaces.Nomenclatures;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Models;
using Microsoft.AspNetCore.Mvc;

namespace IARA.Web.Controllers
{
    [AreaRoute(AreaType.Administrative)]
    public class PoundNetRegisterController : BaseAuditController
    {
        private readonly IPoundNetRegisterService service;
        private readonly IPoundnetNomenclaturesService poundnetNomenclatures;

        public PoundNetRegisterController(IPoundNetRegisterService service,
                                          IPoundnetNomenclaturesService poundnetNomenclatures,
                                          IPermissionsService permissionsService)
            : base(permissionsService)
        {
            this.poundnetNomenclatures = poundnetNomenclatures;
            this.service = service;
        }

        [HttpPost]
        [CustomAuthorize(Permissions.PoundnetsRead)]
        public IActionResult GetAll([FromBody] GridRequestModel<PoundNetRegisterFilters> request)
        {
            IQueryable<PoundNetDTO> result = service.GetAll(request.Filters);
            return PageResult(result, request, false);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.PoundnetsRead)]
        public IActionResult Get([FromQuery] int id)
        {
            PoundnetRegisterDTO poundnet = service.Get(id);
            return Ok(poundnet);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.PoundnetsAddRecords)]
        public IActionResult Add([FromBody] PoundnetRegisterDTO poundnet)
        {
            int id = service.Add(poundnet);
            return Ok(id);
        }

        [HttpPut]
        [CustomAuthorize(Permissions.PoundnetsEditRecords)]
        public IActionResult Edit([FromBody] PoundnetRegisterDTO poundnet)
        {
            service.Edit(poundnet);
            return Ok();
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.PoundnetsDeleteRecords)]
        public IActionResult Delete([FromQuery] int id)
        {
            service.Delete(id);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.PoundnetsRestoreRecords)]
        public IActionResult UndoDelete([FromQuery] int id)
        {
            service.UndoDelete(id);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.PoundnetsRead)]
        public IActionResult GetCategories()
        {
            List<NomenclatureDTO> result = poundnetNomenclatures.GetPoundnetCategories();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.PoundnetsRead)]
        public IActionResult GetSeasonalTypes()
        {
            List<NomenclatureDTO> result = poundnetNomenclatures.GetSeasonalTypes();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.PoundnetsRead)]
        public IActionResult GetPoundnetStatuses()
        {
            List<NomenclatureDTO> result = poundnetNomenclatures.GetPoundnetStatuses();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.PoundnetsRead)]
        public override IActionResult GetAuditInfo([FromQuery] int id)
        {
            SimpleAuditDTO audit = service.GetSimpleAudit(id);
            return Ok(audit);
        }
    }
}
