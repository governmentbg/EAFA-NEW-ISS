using System;
using IARA.DomainModels.DTOModels.ReportViolations;
using IARA.Interfaces;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace IARA.Web.Controllers.Mobile.Public
{
    [AreaRoute(AreaType.MobilePublic)]
    public class ReportViolationController : BaseController
    {
        private readonly IViolationSignalService violationService;

        public ReportViolationController(IViolationSignalService violationService, IPermissionsService permissionsService)
            : base(permissionsService)
        {
            this.violationService = violationService ?? throw new ArgumentNullException(nameof(violationService));
        }

        [HttpPost]
        [CustomAuthorize(Permissions.ReportViolationSend)]
        public IActionResult Report([FromBody] ReportViolationDTO violation)
        {
            this.violationService.EnqueueSignalForViolationEmail(violation, CurrentUser.ID);
            return this.Ok();
        }
    }
}
