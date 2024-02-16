using IARA.Common.Enums;
using IARA.Interfaces.Common;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using Microsoft.AspNetCore.Mvc;
using TL.WebNotifications.Abstractions.Interfaces;
using TL.WebNotifications.Abstractions.Models;

namespace IARA.WebAPI.Controllers.Common
{
    [AreaRoute(AreaType.Common)]
    public class VersionController : BaseController
    {
        private IFileVersionTrackerService trackerService;

        public VersionController(IFileVersionTrackerService trackerService, IPermissionsService permissionsService)
            : base(permissionsService)
        {
            this.trackerService = trackerService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            string version = trackerService.GetVersion();
            return Ok(new { version = version });
        }

        [HttpGet]
        [CustomAuthorize]
        public IActionResult PushNewVersionToOthers([FromServices] IClientWebNotificationsSender<WebNotificationTypes> notificationsHub)
        {
            notificationsHub.AddNotificationToOthers(new Notification<string, WebNotificationTypes>()
            {
                Type = WebNotificationTypes.Version,
                Message = trackerService.GetVersion()
            });

            return Ok();
        }
    }
}
