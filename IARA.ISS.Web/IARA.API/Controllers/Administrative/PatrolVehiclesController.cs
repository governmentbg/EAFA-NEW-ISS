using System.Linq;
using IARA.DomainModels.DTOModels.Vehicles;
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
    public class PatrolVehiclesController : BaseAuditController
    {
        private readonly IPatrolVehiclesService service;

        public PatrolVehiclesController(IPatrolVehiclesService service,
            IPermissionsService permissionsService)
            : base(permissionsService)
        {
            this.service = service;
        }

        [HttpPost]
        [CustomAuthorize(Permissions.PatrolVehiclesRead)]
        public IActionResult GetAll([FromBody] GridRequestModel<PatrolVehiclesFilters> request)
        {
            IQueryable<PatrolVehiclesDTO> patrolVehicles = service.GetAll(request.Filters);
            return PageResult(patrolVehicles, request, false);
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.PatrolVehiclesDeleteRecords)]
        public IActionResult DeletePatrolVehicle([FromQuery] int id)
        {
            service.DeletePatrolVehicle(id);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.PatrolVehiclesRestoreRecords)]
        public IActionResult UndoDeletePatrolVehicle([FromQuery] int id)
        {
            service.UndoDeletePatrolVehicle(id);
            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.PatrolVehiclesAddRecords)]
        public IActionResult AddPatrolVehicle([FromBody] PatrolVehiclesEditDTO patrolVehicle)
        {
            int id = service.AddPatrolVehicle(patrolVehicle);
            return Ok(id);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.PatrolVehiclesEditRecords)]
        public IActionResult EditPatrolVehicle([FromBody] PatrolVehiclesEditDTO patrolVehicle)
        {
            service.EditPatrolVehicle(patrolVehicle);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.PatrolVehiclesRead)]
        public IActionResult GetPatrolVehicle([FromQuery] int id)
        {
            return Ok(service.GetPatrolVehicle(id));
        }

        [HttpGet]
        [CustomAuthorize(Permissions.PatrolVehiclesRead)]
        public override IActionResult GetAuditInfo(int id)
        {
            return Ok(service.GetSimpleAudit(id));
        }
    }
}
