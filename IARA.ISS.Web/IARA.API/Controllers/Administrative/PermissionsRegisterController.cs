using System.Collections.Generic;
using System.Linq;
using IARA.DomainModels.DTOModels.PermissionsRegister;
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
    public class PermissionsRegisterController : BaseAuditController
    {
        private readonly IPermissionsRegisterService service;
        private readonly IRoleService roleService;

        public PermissionsRegisterController(IPermissionsRegisterService service, IRoleService roleService, IPermissionsService permissionsService)
            : base(permissionsService)
        {
            this.service = service;
            this.roleService = roleService;
        }

        [HttpPost]
        [CustomAuthorize(Permissions.PermissionsRegisterRead)]
        public IActionResult GetAllPermissions([FromBody] GridRequestModel<PermissionsRegisterFilters> request)
        {
            IQueryable<PermissionRegisterDTO> permissions = service.GetAllPermissions(request.Filters);
            return PageResult(permissions, request, false);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.PermissionsRegisterRead)]
        public IActionResult GetPermission([FromQuery] int id)
        {
            PermissionRegisterEditDTO permission = service.GetPermission(id);
            return Ok(permission);
        }

        [HttpPut]
        [CustomAuthorize(Permissions.PermissionsRegisterEditRecords)]
        public IActionResult EditPermission([FromBody] PermissionRegisterEditDTO permission)
        {
            service.EditPermission(permission);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.PermissionsRegisterRead)]
        public IActionResult GetAllPermissionTypes()
        {
            List<NomenclatureDTO> types = service.GetAllPermissionTypes();
            return Ok(types);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.PermissionsRegisterRead)]
        public IActionResult GetAllPermissionGroups()
        {
            List<NomenclatureDTO> types = service.GetAllPermissionGroups();
            return Ok(types);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.PermissionsRegisterRead)]
        public IActionResult GetAllRoles()
        {
            List<NomenclatureDTO> roles = roleService.GetAllRoles();
            return Ok(roles);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.PermissionsRegisterRead)]
        public override IActionResult GetAuditInfo(int id)
        {
            SimpleAuditDTO audit = service.GetSimpleAudit(id);
            return Ok(audit);
        }
    }
}
