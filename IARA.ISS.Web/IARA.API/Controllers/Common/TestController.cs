using System;
using IARA.Interfaces.Common;
using IARA.Logging.Abstractions.Interfaces;
using IARA.WebHelpers;
using Microsoft.AspNetCore.Mvc;
using TL.Caching.Interfaces;
using TL.SysToSysSecCom;
using TL.SysToSysSecCom.Interfaces;

namespace IARA.Web.Controllers.Public
{
    [AreaRoute(AreaType.Common)]
    public class TestController : ControllerBase
    {
        public TestController(IMemoryCacheService memoryCacheService)
        {
        }

        [HttpGet]
        public IActionResult WebNotifications([FromServices] IWebNotificationsService webNotificationsService, int userId = 3)
        {
            webNotificationsService.NotifyUser(userId, "NEWS_MOBILE", new { userId }, "http://iara.egov.bg/news");
            return Ok();
        }

        [HttpGet]
        public IActionResult WebNotificationsBroadcast([FromServices] IWebNotificationsService webNotificationsService)
        {
            webNotificationsService.NotifyAllConnectedUsers("NEWS_MOBILE", new { UserId = 3 }, "http://iara.egov.bg/news");
            return Ok();
        }

        [HttpGet]
        public IActionResult TeamsLog([FromServices] IExtendedLogger logger)
        {
            try
            {
                throw new NotImplementedException(nameof(TeamsLog));
            }
            catch (Exception ex)
            {
                logger.LogException(ex);
            }

            return Ok();
        }

        [HttpGet]
        public IActionResult Ping()
        {
            return Ok("Success");
        }

        [HttpPost]
        public IActionResult SecurePing([FromBody] SecurePayload payload, [FromServices] ICryptoHelper cryptoHelper)
        {
            if (cryptoHelper.TryUnwrapPayload(payload, out FluxShipModel model))
            {
                var response = cryptoHelper.ToSecurePayload(new FluxShipModel
                {
                    ID = 1914,
                    ShipName = "Titanic"
                });

                return Ok(response);
            }
            else
            {
                return BadRequest();
            }
        }
    }

    public class FluxShipModel
    {
        public int ID { get; set; }
        public string ShipName { get; set; }
    }
}
