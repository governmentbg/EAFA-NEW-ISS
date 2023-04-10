using System.Collections.Generic;
using System.Linq;
using IARA.Common.Exceptions;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.CatchesAndSales;
using IARA.DomainModels.RequestModels;
using IARA.Interfaces.CatchSales;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using IARA.WebHelpers.Models;
using Microsoft.AspNetCore.Mvc;

namespace IARA.WebAPI.Controllers.Administrative
{
    [AreaRoute(AreaType.Administrative)]
    public class LogBookPageEditExceptionsController : BaseAuditController
    {
        private readonly ILogBookPageEditExceptionsService service;

        public LogBookPageEditExceptionsController(IPermissionsService permissionsService, ILogBookPageEditExceptionsService service)
            : base(permissionsService)
        {
            this.service = service;
        }

        [HttpPost]
        [CustomAuthorize(Permissions.LogBookPageEditExceptionsRead)]
        public IActionResult GetAllLogBookPageEditExceptions([FromBody] GridRequestModel<LogBookPageEditExceptionFilters> request)
        {
            IQueryable<LogBookPageEditExceptionRegisterDTO> results = service.GetAllLogBookPageEditExceptions(request.Filters);
            return PageResult(results, request, false);
        }

        [HttpGet]
        [CustomAuthorize(Permissions.LogBookPageEditExceptionsRead)]
        public IActionResult GetLogBookPageEditException([FromQuery] int id)
        {
            LogBookPageEditExceptionEditDTO result = service.GetLogBookPageEditException(id);
            return Ok(result);
        }

        [HttpPost]
        [CustomAuthorize(Permissions.LogBookPageEditExceptionsAddRecords)]
        public IActionResult AddLogBookPageEditException([FromBody] LogBookPageEditExceptionEditDTO model)
        {
            try
            {
                service.AddOrEditLogBookPageEditException(model);
                return Ok();
            }
            catch(LogBookPageEditExceptionCombinationExistsException)
            {
                string msg = ErrorResources.logBookPageEditExceptionCombinationExistsMsg;
                return ValidationFailedResult(new List<string> { msg });
            }
        }

        [HttpPut]
        [CustomAuthorize(Permissions.LogBookPageEditExceptionsEditRecords)]
        public IActionResult EditLogBookPageEditException([FromBody] LogBookPageEditExceptionEditDTO model)
        {
            try
            {
                service.AddOrEditLogBookPageEditException(model);
                return Ok();
            }
            catch (LogBookPageEditExceptionCombinationExistsException)
            {
                string msg = ErrorResources.logBookPageEditExceptionCombinationExistsMsg;
                return ValidationFailedResult(new List<string> { msg });
            }
        }

        [HttpDelete]
        [CustomAuthorize(Permissions.LogBookPageEditExceptionsDeleteRecords)]
        public IActionResult DeleteLogBookPageEditException([FromQuery] int id)
        {
            service.DeleteLogBookPageEditException(id);
            return Ok();
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.LogBookPageEditExceptionsRestoreRecords)]
        public IActionResult RestoreLogBookPageEditException([FromQuery] int id)
        {
            service.RestoreLogBookPageEditException(id);
            return Ok();
        }

        // Nomenclatures

        [HttpGet]
        [CustomAuthorize(Permissions.LogBookPageEditExceptionsRead)]
        public IActionResult GetAllUsersNomenclature()
        {
            return Ok(service.GetAllUsersNomenclature());
        }

        [HttpGet]
        [CustomAuthorize(Permissions.LogBookPageEditExceptionsRead)]
        public IActionResult GetActiveLogBooksNomenclature([FromQuery] int? logBookPageEditExceptionId)
        {
            return Ok(service.GetActiveLogBooksNomenclature(logBookPageEditExceptionId));
        }

        // Audit

        [HttpGet]
        [CustomAuthorize(Permissions.LogBookPageEditExceptionsRead)]
        public override IActionResult GetAuditInfo(int id)
        {
            return Ok(this.service.GetSimpleAudit(id));
        }
    }
}
