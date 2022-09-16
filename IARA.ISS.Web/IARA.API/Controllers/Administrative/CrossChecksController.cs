using System.Collections.Generic;
using System.Linq;
using IARA.DomainModels.DTOModels.CrossChecks;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces;
using IARA.Interfaces.CrossChecks;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Models;
using Microsoft.AspNetCore.Mvc;

namespace IARA.WebAPI.Controllers.Administrative
{
    [AreaRoute(AreaType.Administrative)]
    public class CrossChecksController : BaseAuditController
    {
        private readonly ICrossChecksAdministrationService service;
        private readonly ICrossChecksExecutionService executionService;
        private readonly IRoleService roleService;

        public CrossChecksController(ICrossChecksAdministrationService service,
                                     ICrossChecksExecutionService executionService,
                                     IRoleService roleService,
                                     IPermissionsService permissionsService)
            : base(permissionsService)
        {
            this.service = service;
            this.executionService = executionService;
            this.roleService = roleService;
        }

        // Cross checks
        [HttpPost]
        [CustomAuthorize(Permissions.CrossChecksRead)]
        public IActionResult GetAllCrossChecks([FromBody] GridRequestModel<CrossChecksFilters> request)
        {
            IQueryable<CrossCheckDTO> result = service.GetAllCrossChecks(request.Filters);
            return PageResult(result, request, false);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CrossChecksRead)]
        public IActionResult GetCrossCheck([FromQuery] int id)
        {
            CrossCheckEditDTO result = service.GetCrossCheck(id);
            return Ok(result);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.CrossChecksAddRecords)]
        public IActionResult AddCrossCheck([FromBody] CrossCheckEditDTO crossCheck)
        {
            int id = service.AddCrossCheck(crossCheck);
            return Ok(id);
        }

        [HttpPut]
        [CustomAuthorize(Permissions.CrossChecksEditRecords)]
        public IActionResult EditCrossCheck([FromBody] CrossCheckEditDTO crossCheck)
        {
            service.EditCrossCheck(crossCheck);
            return Ok();
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.CrossChecksDeleteRecords)]
        public IActionResult DeleteCrossCheck([FromQuery] int id)
        {
            service.DeleteCrossCheck(id);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.CrossChecksRestoreRecords)]
        public IActionResult UndoDeleteCrossCheck([FromQuery] int id)
        {
            service.UndoDeleteCrossCheck(id);
            return Ok();
        }

        [HttpPost]
        [CustomAuthorize(Permissions.CrossChecksExecuteRecords)]
        public IActionResult ExecuteCrossCheck([FromBody] int crossCheckId)
        {
            int id = executionService.ExecuteCrossCheck(crossCheckId);
            return Ok(id);
        }

        // Cross check results
        [HttpPost]
        [CustomAuthorize(Permissions.CrossCheckResultsRead)]
        public IActionResult GetAllCrossCheckResults([FromBody] GridRequestModel<CrossCheckResultsFilters> request)
        {
            IQueryable<CrossCheckResultDTO> result = service.GetAllCrossCheckResults(request.Filters);
            return PageResult(result, request, false);
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.CrossCheckResultsDeleteRecords)]
        public IActionResult DeleteCrossCheckResult([FromQuery] int id)
        {
            service.DeleteCrossCheckResult(id);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.CrossCheckResultsRestoreRecords)]
        public IActionResult UndoDeleteCrossCheckResult([FromQuery] int id)
        {
            service.UndoDeleteCrossCheckResult(id);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.CrossCheckResultsAssign)]
        public IActionResult AssignCrossCheckResult([FromQuery] int resultId, [FromQuery] int userId)
        {
            service.AssignCrossCheckResult(resultId, userId);
            return Ok();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CrossCheckResultsResolve)]
        public IActionResult GetCrossCheckResolution([FromQuery] int resultId)
        {
            if (service.HasUserAccessToCrossCheckResolution(resultId, CurrentUser.ID))
            {
                CrossCheckResolutionEditDTO result = service.GetCrossCheckResolution(resultId);
                return Ok(result);
            }
            return NotFound();
        }

        [HttpPut]
        [CustomAuthorize(Permissions.CrossCheckResultsResolve)]
        public IActionResult EditCrossCheckResolution([FromBody] CrossCheckResolutionEditDTO resolution)
        {
            if (service.HasUserAccessToCrossCheckResolution(resolution.CheckResultId!.Value, CurrentUser.ID))
            {
                service.EditCrossCheckResolution(resolution);
                return Ok();
            }
            return NotFound();
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CrossCheckResultsResolve)]
        public IActionResult GetCrossCheckResultSimpleAudit([FromQuery] int id)
        {
            if (service.HasUserAccessToCrossCheckResolution(id, CurrentUser.ID))
            {
                SimpleAuditDTO result = service.GetCrossCheckResultSimpleAudit(id);
                return Ok(result);
            }
            return NotFound();
        }

        // Nomenclatures
        [HttpGet]
        [CustomAuthorize(Permissions.CrossChecksRead)]
        public IActionResult GetAllReportGroups()
        {
            List<NomenclatureDTO> result = service.GetAllReportGroups();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CrossCheckResultsRead)]
        public IActionResult GetCheckResolutionTypes()
        {
            List<NomenclatureDTO> result = service.GetCheckResolutionTypes();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CrossChecksRead)]
        public IActionResult GetAllActiveRoles()
        {
            List<NomenclatureDTO> result = roleService.GetAllActiveRoles();
            return Ok(result);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.CrossChecksRead)]
        public override IActionResult GetAuditInfo(int id)
        {
            SimpleAuditDTO audit = service.GetSimpleAudit(id);
            return Ok(audit);
        }
    }
}
