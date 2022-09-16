using System;
using IARA.Flux.Models;
using IARA.Interfaces.FluxIntegrations;
using IARA.Logging.Abstractions.Interfaces;
using IARA.WebHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TL.SysToSysSecCom.Interfaces;
using TL.SysToSysSecCom.Utils;

namespace IARA.WebAPI.Controllers.Integration
{
    [AreaRoute(AreaType.Integration)]
    public class MasterDataManagementController : BaseFluxController
    {
        private IMasterManagementDomainService service;
        public MasterDataManagementController(ICryptoHelper cryptoHelper, IMasterManagementDomainService service, IExtendedLogger logger)
              : base(service, cryptoHelper, logger)
        {
            this.service = service;
        }

        protected override void ActionExecuting(ActionExecutingContext context)
        {
            try
            {
                base.ActionExecuting(context);
            }
            catch (System.Exception ex)
            {
                //LOG
                Console.WriteLine(ex.Message);
            }
        }


        [HttpPost]
        public IActionResult ReceiveNomenclatureData([FromSecureBody] FLUXMDRReturnMessageType response)
        {
            service.ReceiveNomenclatureData(response);
            return Ok();
        }
    }
}
