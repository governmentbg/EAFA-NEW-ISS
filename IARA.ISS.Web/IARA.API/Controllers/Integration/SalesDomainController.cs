using System.Net;
using System.Threading.Tasks;
using IARA.Flux.Models;
using IARA.Interfaces.Flux;
using IARA.Logging.Abstractions.Interfaces;
using IARA.WebAPI.Controllers.Integration;
using IARA.WebHelpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TL.SysToSysSecCom.Interfaces;
using TL.SysToSysSecCom.Utils;

namespace IARA.Web.Controllers.Integration
{
    [AreaRoute(AreaType.Integration)]
    public class SalesDomainController : BaseFluxController
    {
        private IFluxSalesDomainInitiatorService service;
        public SalesDomainController(ICryptoHelper cryptoHelper, IFluxSalesDomainInitiatorService service, IExtendedLogger logger)
            : base(service, cryptoHelper, logger)
        {
            this.service = service;
        }

        /// <summary>
        /// Получаване на данни от декларации за първа продажба, приемане
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ReceiveSalesQuery([FromSecureBody] FLUXSalesQueryMessageType query)
        {
            bool result = await service.ReceiveFluxSalesQuery(query);
            return result ? Ok() : InternalServerError();
        }


        /// <summary>
        /// 1. Актуализиране на статус в ИСС на изпратени данни към FLUX възел при изпращане от FLUX възел към централен FLUX възел
        /// 2. Получаване на отговор от Централен Flux възел при рапортуване на данни за деклрации за първа продажба, приемане
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public override Task<IActionResult> UpdateReportStatus([FromSecureBody] FLUXResponseMessageType response)
        {
            return base.UpdateReportStatus(response);
        }

        /// <summary>
        /// Получаване на данни от декларации за първа продажба, приемане
        /// </summary>
        /// <param name="report"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ReceiveSalesReport([FromSecureBody] FLUXSalesReportMessageType report)
        {
            bool result = await service.ReceiveSalesReport(report);
            return result ? Ok() : InternalServerError();
        }


        /// <summary>
        /// Получаване на данни от декларации за първа продажба, приемане
        /// </summary>
        /// <param name="response">Съдържа данни за заявената продажба</param>
        /// <returns>Status code 200</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FluxQueryResponse([FromSecureBody] FLUXSalesReportMessageType response)
        {
            bool result = await service.ReceiveSalesReport(response);
            return result ? Ok() : InternalServerError();
        }

        private IActionResult InternalServerError()
        {
            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }
}
