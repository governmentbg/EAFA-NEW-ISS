using System.Collections.Generic;
using IARA.Logging.Abstractions.Interfaces;
using IARA.Logging.Abstractions.Models;
using IARA.Security;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace IARA.Web.Controllers.Mobile.Administrative
{
    [AreaRoute(AreaType.MobileAdministrative)]
    public class LoggerController : BaseController
    {
        private readonly IExtendedLogger logger;

        public LoggerController(IExtendedLogger logger, IPermissionsService permissionsService)
            : base(permissionsService)
        {
            this.logger = logger;
        }

        [HttpPost]
        public IActionResult Log([FromBody] LogRecord records)
        {
            this.logger.Log(records);
            return this.Ok();
        }

        [HttpPost]
        public IActionResult LogErrors([FromBody] List<LogRecord> records)
        {
            this.logger.Log(records);
            return this.Ok();
        }
    }
}
