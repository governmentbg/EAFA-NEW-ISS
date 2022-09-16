using System.Collections.Generic;
using System.Linq;
using IARA.DomainModels.DTOModels.Translations;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Models;
using Microsoft.AspNetCore.Mvc;

namespace IARA.Web.Controllers.Administrative
{
    [AreaRoute(AreaType.Administrative)]
    public class TranslationsManagementController : BaseAuditController
    {
        private readonly ITranslationManagementService service;

        public TranslationsManagementController(ITranslationManagementService service,
                                                IPermissionsService permissionsService)
            : base(permissionsService)
        {
            this.service = service;
        }

        [HttpPost]
        [CustomAuthorize(Permissions.TranslationRead)]
        public IActionResult GetAll([FromQuery] bool helper, [FromBody] GridRequestModel<TranslationManagementFilters> request)
        {
            IQueryable<TranslationManagementDTO> result = service.GetAll(request.Filters, helper);
            return PageResult(result, request, false);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TranslationRead)]
        public IActionResult Get([FromQuery] int id)
        {
            TranslationManagementEditDTO entry = service.Get(id);
            return Ok(entry);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TranslationRead)]
        public IActionResult GetByKey([FromQuery] string key)
        {
            TranslationManagementEditDTO entry = service.GetByKey(key);
            return Ok(entry);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.TranslationAddRecords)]
        public IActionResult Add([FromBody] TranslationManagementEditDTO resource)
        {
            int id = service.AddEntry(resource);
            return Ok(id);
        }

        [HttpPut]
        [CustomAuthorize(Permissions.TranslationEditRecords)]
        public IActionResult Edit([FromBody] TranslationManagementEditDTO updatedResource)
        {
            service.EditEntry(updatedResource);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TranslationRead)]
        public IActionResult GetGroups()
        {
            List<NomenclatureDTO> groups = service.GetGroups();
            return Ok(groups);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.TranslationRead)]
        public override IActionResult GetAuditInfo([FromQuery] int id)
        {
            SimpleAuditDTO audit = service.GetSimpleAudit(id);
            return Ok(audit);
        }
    }
}
