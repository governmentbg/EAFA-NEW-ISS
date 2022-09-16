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
    public class VesselDomainController : BaseFluxController
    {
        private IFluxVesselDomainInitiatorService service;
        public VesselDomainController(ICryptoHelper cryptoHelper, IFluxVesselDomainInitiatorService service, IExtendedLogger logger)
            : base(service, cryptoHelper, logger)
        {
            this.service = service;
        }

        /// <summary>
        /// Заявка за данни за характеристики на риболовен кораб от централен Flux възел към ИСС
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FluxVesselQuery([FromSecureBody] FLUXVesselQueryMessageType query)
        {
            bool result = await service.ReceiveVesselQuery(query);
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

        /// <summary>
        /// Отговор на централен FLUX възел/ локален FLUX възел към ИСС
        /// Отговор на централен FLUX възел/ локален FLUX възел към ИСС на подадената заявка за данни
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> VesselQueryReply([FromSecureBody] FLUXReportVesselInformationType response)
        {
            bool result = await service.VesselQueryReply(response);
            return result ? Ok() : InternalServerError();
        }

    }
}
