using System.Collections.Generic;
using System.Linq;
using IARA.DomainModels.DTOModels.RolesRegister;
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
    public class RolesRegisterController : BaseAuditController
    {
        private readonly IRolesRegisterService service;

        public RolesRegisterController(IRolesRegisterService service, IPermissionsService permissionsService)
            : base(permissionsService)
        {
            this.service = service;
        }

        [HttpPost]
        [CustomAuthorize(Permissions.RolesRegisterRead)]
        public IActionResult GetAllRoles([FromBody] GridRequestModel<RolesRegisterFilters> request)
        {
            IQueryable<RoleRegisterDTO> roles = service.GetAllRoles(request.Filters);
            return PageResult(roles, request, false);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.RolesRegisterRead)]
        public IActionResult GetRole([FromQuery] int id)
        {
            RoleRegisterEditDTO role = service.GetRole(id);
            return Ok(role);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.RolesRegisterAddRecords)]
        public IActionResult AddRole([FromBody] RoleRegisterEditDTO role)
        {
            int id = service.AddRole(role);
            return Ok(id);
        }

        [HttpPut]
        [CustomAuthorize(Permissions.RolesRegisterEditRecords)]
        public IActionResult EditRole([FromBody] RoleRegisterEditDTO role)
        {
            service.EditRole(role);
            return Ok();
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.RolesRegisterDeleteRecords)]
        public IActionResult DeleteRole([FromQuery] int id)
        {
            service.DeleteRole(id);
            return Ok();
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.RolesRegisterDeleteRecords)]
        public IActionResult DeleteAndReplaceRole([FromQuery] int id, [FromQuery] int newRoleId)
        {
            service.DeleteAndReplaceRole(id, newRoleId);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.RolesRegisterRestoreRecords)]
        public IActionResult UndoDeleteRole([FromQuery] int id)
        {
            service.UndoDeleteRole(id);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.RolesRegisterRead)]
        public IActionResult GetPermissionGroups()
        {
            List<PermissionGroupDTO> groups = service.GetPermissionGroups();
            return Ok(groups);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.RolesRegisterRead)]
        public IActionResult GetUsersWithRole([FromQuery] int roleId)
        {
            List<NomenclatureDTO> users = service.GetUsersWithRole(roleId);
            return Ok(users);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.RolesRegisterRead)]
        public override IActionResult GetAuditInfo(int id)
        {
            SimpleAuditDTO audit = service.GetSimpleAudit(id);
            return Ok(audit);
        }
    }
}
