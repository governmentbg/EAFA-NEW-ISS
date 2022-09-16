using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using IARA.Flux.Models;
using IARA.Interfaces.FluxIntegrations;
using IARA.Logging.Abstractions.Interfaces;
using IARA.WebHelpers.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TL.SysToSysSecCom.Controller;
using TL.SysToSysSecCom.Interfaces;
using TL.SysToSysSecCom.Utils;

namespace IARA.WebAPI.Controllers.Integration
{
    public abstract class BaseFluxController : BaseSecureController
    {
        private IBaseFluxService responseService;
        protected readonly IExtendedLogger logger;

        protected BaseFluxController(IBaseFluxService responseService, ICryptoHelper cryptoHelper, IExtendedLogger logger)
            : base(cryptoHelper)
        {
            this.logger = logger;
            this.responseService = responseService;
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public virtual async Task<IActionResult> UpdateReportStatus([FromSecureBody] FLUXResponseMessageType response)
        {
            bool result = await responseService.ReceiveResponse(response);
            return result ? Ok() : InternalServerError();
        }

        protected override void ActionExecuting(ActionExecutingContext context)
        {
            string message = $"{string.Join(",", context.ActionArguments.Select(x => $"{x.Key}:{x.Value}"))}";
            logger.LogInfo(context.HttpContext.Request.Path + "  " + Environment.NewLine + message);
            context.SetCurrentPrincipal(true, "FLUX");
            base.ActionExecuting(context);
        }

        protected IActionResult InternalServerError()
        {
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }
}
