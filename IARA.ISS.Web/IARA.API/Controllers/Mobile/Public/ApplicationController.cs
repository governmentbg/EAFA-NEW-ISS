using IARA.Common.Enums;
using IARA.Interfaces;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace IARA.Web.Controllers.Mobile.Public
{
    [AreaRoute(AreaType.MobilePublic)]
    public class ApplicationController : BaseController
    {
        private readonly IApplicationService applicationService;

        public ApplicationController(IApplicationService applicationService, IPermissionsService permissionsService)
            : base(permissionsService)
        {
            this.applicationService = applicationService;
        }

        [HttpPatch]
        [CustomAuthorize(Permissions.TicketApplicationsEdit)]
        public IActionResult InitiateApplicationCorrections([FromQuery] int applicationId)
        {
            ApplicationStatusesEnum newStatus = this.applicationService.InitiateApplicationCorrections(applicationId);
            return this.Ok(new { status = newStatus.ToString() });
        }
    }
}
