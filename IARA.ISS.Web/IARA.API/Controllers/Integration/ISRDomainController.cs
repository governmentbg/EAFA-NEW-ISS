using System.Threading.Tasks;
using IARA.FluxInspectionModels;
using IARA.Interfaces.Flux.InspectionAndSurveillance;
using IARA.WebAPI.Controllers.Integration;
using IARA.WebHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TL.Logging.Abstractions.Interfaces;
using TL.SysToSysSecCom.Abstractions.Interfaces;
using TL.SysToSysSecCom.Abstractions.Models;
using TL.SysToSysSecCom.Utils;

namespace IARA.Web.Controllers.Integration
{
    [AreaRoute(AreaType.Integration)]
    public class ISRDomainController : BaseFluxController
    {
        private readonly IFluxInspectionsDomainInitiatorService service;

        public ISRDomainController(ICryptoHelper cryptoHelper,
                                           SysToSysCryptoSettings settings,
                                           IRequestContentSerializer serializer,
                                           IFluxInspectionsDomainInitiatorService service,
                                           IExtendedLogger logger)
            : base(service, settings, serializer, cryptoHelper, logger)
        {
            this.service = service;
        }

        /// <summary>
        /// Получаване на данни от инспекция
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ReceiveISRReport([FromSecureBody] FLUXISRReportResponse message)
        {
            bool result = await service.ReceiveISRReport(message);
            return result ? Ok() : InternalServerError();
        }

        /// <summary>
        /// Получаване на данни от инспекция
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ReceiveISRQuery([FromSecureBody] FLUXISRQueryResponse message)
        {
            bool result = await service.ReceiveISRQuery(message);
            return result ? Ok() : InternalServerError();
        }

        /// <summary>
        /// Получаване на данни от инспекция
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ReceiveISRResponse([FromSecureBody] FLUXISRResponseMessageType message)
        {
            bool result = await service.ReceiveISRResponse(message);
            return result ? Ok() : InternalServerError();
        }
    }
}
