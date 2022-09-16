using System.Threading.Tasks;
using IARA.Flux.Models;
using IARA.Interfaces.Flux.PermitsAndCertificates;
using IARA.Logging.Abstractions.Interfaces;
using IARA.WebAPI.Controllers.Integration;
using IARA.WebHelpers;
using Microsoft.AspNetCore.Mvc;
using TL.SysToSysSecCom.Interfaces;
using TL.SysToSysSecCom.Utils;

namespace IARA.Web.Controllers.Integration
{
    [AreaRoute(AreaType.Integration)]
    public class PermitsDomainController : BaseFluxController
    {
        private readonly IFluxPermitsDomainInitiatorService service;

        public PermitsDomainController(ICryptoHelper cryptoHelper, IFluxPermitsDomainInitiatorService service, IExtendedLogger logger)
            : base(service, cryptoHelper, logger)
        {
            this.service = service;
        }

        /// <summary>
        /// Получаване на отговор с данни
        /// </summary>
        [HttpPost]
        public IActionResult ReceiveFlapResponse([FromSecureBody] FLUXFLAPResponseMessageType response)
        {
            service.ReceiveFlapResponse(response);
            return Ok();
        }

        /// <summary>
        /// Получаване на FLAP заявка
        /// </summary>
        [HttpPost]
        public IActionResult ReceiveFlapRequest([FromSecureBody] FLUXFLAPRequestMessageType request)
        {
            service.ReceiveFlapRequest(request);
            return Ok();
        }

        /// <summary>
        /// Актуализиране на статус в ИСС на изпратени данни към FLUX възел при изпращане от FLUX възел към централен FLUX възел/ друга страна-членка
        /// </summary>
        public override Task<IActionResult> UpdateReportStatus([FromSecureBody] FLUXResponseMessageType response)
        {
            return base.UpdateReportStatus(response);
        }
    }
}
