using System.Collections.Generic;
using IARA.Logging.Abstractions.Interfaces;
using IARA.Logging.Abstractions.Models;
using IARA.Security;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace IARA.Web.Controllers.Mobile.Public
{
    [AreaRoute(AreaType.MobilePublic)]
    public class LoggerController : BaseController
    {
        private readonly IExtendedLogger logger;

        public LoggerController(IExtendedLogger logger, IPermissionsService permissionsService)
            : base(permissionsService)
        {
            this.logger = logger;
        }

        [HttpPost]
        public IActionResult Log(LogRecord records)
        {
            logger.Log(records);
            return Ok();
        }

        [HttpPost]
        public IActionResult LogErrors(List<LogRecord> records)
        {
            logger.Log(records);
            return Ok();
        }
    }
}
