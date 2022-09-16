using System.Net.Mime;
using System.Threading.Tasks;
using IARA.Flux.Models;
using IARA.Interfaces.Flux.AggregatedCatchReports;
using IARA.Logging.Abstractions.Interfaces;
using IARA.WebAPI.Controllers.Integration;
using IARA.WebHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TL.SysToSysSecCom.Interfaces;
using TL.SysToSysSecCom.Utils;

namespace IARA.Web.Controllers.Integration
{
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces("application/json")]
    [AreaRoute(AreaType.Integration)]
    public class AggregatedReportController : BaseFluxController
    {
        private IFluxAggregatedCatchReportInitiatorService service;
        public AggregatedReportController(ICryptoHelper cryptoHelper, IFluxAggregatedCatchReportInitiatorService service, IExtendedLogger logger)
            : base(service, cryptoHelper, logger)
        {
            this.service = service;
        }

        /// <summary>
        /// Отговор на централен FLUX възел/ локален FLUX възел към ИСС
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AggregatedReportReceived([FromSecureBody] FLUXResponseMessageType response)
        {
            bool result = await service.ReceiveResponse(response);
            return result ? Ok() : InternalServerError();
        }

        /// <summary>
        /// Актуализиране на статус в ИСС на изпратени данни към FLUX възел при изпращане от FLUX възел към централен FLUX възел
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AggregatedReportSentToFlux([FromSecureBody] FLUXResponseMessageType response)
        {
            bool result = await service.ReceiveResponse(response);
            return result ? Ok() : InternalServerError();
        }
    }
}
