using System.Net.Mime;
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
    [Consumes(MediaTypeNames.Application.Json)]
    [Produces("application/json")]
    [AreaRoute(AreaType.Integration)]
    public class FishingActivitiesController : BaseFluxController
    {
        private IFluxFishingActivitiesInitiatorService service;
        public FishingActivitiesController(ICryptoHelper cryptoHelper, IFluxFishingActivitiesInitiatorService service, IExtendedLogger logger)
            : base(service, cryptoHelper, logger)
        {
            this.service = service;
        }

        /// <summary>
        /// Рапортуване на данни от риболовни дейности
        /// </summary>
        /// <param name="reponse"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> PublishFAReport([FromSecureBody] FLUXFAReportMessageType reponse)
        {
            bool result = await service.FAReportReceived(reponse);
            return result ? Ok() : InternalServerError();
        }

        /// <summary>
        /// Flux - FA - Query with parameters - VesselID, Period of time
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FAQuery([FromSecureBody] FLUXFAQueryMessageType query)
        {
            bool result = await service.ReceiveFAQuery(query);
            return result ? Ok() : InternalServerError();
        }


        /// <summary>
        /// Актуализиране на статус в ИСС на изпратени данни към FLUX възел при изпращане от FLUX възел към централен FLUX възел
        /// Актуализиране на статус в ИСС на изпратена заявка за данни към FLUX възел при изпращане от FLUX възел към централен FLUX възел
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public override Task<IActionResult> UpdateReportStatus([FromSecureBody] FLUXResponseMessageType response)
        {
            return base.UpdateReportStatus(response);
        }
    }
}
