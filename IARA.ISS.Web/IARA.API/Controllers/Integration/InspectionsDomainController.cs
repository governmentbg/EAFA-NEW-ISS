using IARA.Interfaces.Flux.InspectionAndSurveillance;
using IARA.Logging.Abstractions.Interfaces;
using IARA.WebAPI.Controllers.Integration;
using IARA.WebHelpers;
using Microsoft.AspNetCore.Mvc;
using TL.SysToSysSecCom.Interfaces;

namespace IARA.Web.Controllers.Integration
{
    [AreaRoute(AreaType.Integration)]
    public class InspectionsDomainController : BaseFluxController
    {
        private IFluxInspectionsDomainInitiatorService service;
        public InspectionsDomainController(ICryptoHelper cryptoHelper, IFluxInspectionsDomainInitiatorService service, IExtendedLogger logger)
            : base(service, cryptoHelper, logger)
        {
            this.service = service;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return Ok();
        }
    }
}
