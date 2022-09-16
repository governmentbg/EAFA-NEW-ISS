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
    public class ErrorLogController : BaseController
    {
        private IErrorLogService service;

        public ErrorLogController(IErrorLogService service, IPermissionsService permissionsService)
            : base(permissionsService)
        {
            this.service = service;
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ErrorLogRead)]
        public IActionResult GetAll([FromBody] GridRequestModel<ErrorLogFilters> request)
        {
            var result = service.GetAll(request.Filters);

            return PageResult(result, request, false);
        }
    }
}


