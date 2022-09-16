using System.Collections.Generic;
using IARA.DomainModels.Nomenclatures;
using IARA.Interfaces;
using IARA.Security;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace IARA.Web.Controllers.Administrative
{
    [AreaRoute(AreaType.Administrative)]
    public class RoleController : BaseAuditController
    {
        private readonly IRoleService service;

        public RoleController(IRoleService service, IPermissionsService permissionsService)
            : base(permissionsService)
        {
            this.service = service;
        }

        [HttpGet]
        public IActionResult GetAllActiveRoles()
        {
            List<NomenclatureDTO> result = service.GetAllActiveRoles();
            return Ok(result);
        }

        [HttpGet]
        public IActionResult GetInternalActiveRoles()
        {
            List<NomenclatureDTO> result = service.GetInternalActiveRoles();
            return Ok(result);
        }

        [HttpGet]
        public IActionResult GetPublicActiveRoles()
        {
            List<NomenclatureDTO> result = service.GetPublicActiveRoles();
            return Ok(result);
        }

        [HttpGet]
        public override IActionResult GetAuditInfo(int id)
        {
            return Ok(service.GetSimpleAudit(id));
        }
    }
}
