using System.Linq;
using IARA.DomainModels.DTOModels.SystemLog;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces;
using IARA.Interfaces.Nomenclatures;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Models;
using Microsoft.AspNetCore.Mvc;

namespace IARA.Web.Controllers.Administrative
{
    [AreaRoute(AreaType.Administrative)]
    public class SystemLogController : BaseAuditController
    {
        private readonly ISystemLogService systemLogService;
        private readonly ISystemLogNomenclaturesService systemLogNomenclatureService;
        public SystemLogController(ISystemLogService systemLogService,
                                   ISystemLogNomenclaturesService systemLogNomenclatureService,
                                   IPermissionsService permissionsService)
            : base(permissionsService)
        {
            this.systemLogService = systemLogService;
            this.systemLogNomenclatureService = systemLogNomenclatureService;
        }

        [HttpPost]
        [CustomAuthorize(Permissions.SystemLogRead)]
        public IActionResult GetAll([FromBody] GridRequestModel<SystemLogFilters> request)
        {
            IQueryable<SystemLogDTO> result = systemLogService.GetAll(request.Filters);

            return PageResult(result, request, false);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.SystemLogRead)]
        public IActionResult Get([FromQuery] int id)
        {
            return Ok(systemLogService.Get(id));
        }

        [HttpGet]
        [CustomAuthorize(Permissions.SystemLogRead)]
        public IActionResult GetActionTypeCategories()
        {
            var result = systemLogNomenclatureService.GetActionTypeCategories();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.SystemLogRead)]
        public override IActionResult GetAuditInfo(int id)
        {
            return Ok(systemLogService.GetSimpleAudit(id));
        }
    }
}
