using IARA.DomainModels.DTOModels.Application;
using IARA.Interfaces;
using IARA.Security;
using IARA.Security.Permissions;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace IARA.Web.Controllers.Public
{
    // TODO add security checks
    [AreaRoute(AreaType.Public)]
    public class DeliveryPublicController : BaseController
    {
        private readonly IDeliveryService service;

        public DeliveryPublicController(IPermissionsService permissionsService, IDeliveryService deliveryService)
            : base(permissionsService)
        {
            this.service = deliveryService;
        }

        [HttpGet]
        [CustomAuthorize(Permissions.OnlineSubmittedApplicationsRead, Permissions.OnlineSubmittedApplicationsReadRegister)]
        public IActionResult GetDeliveryData([FromQuery] int deliveryId)
        {
            ApplicationDeliveryDTO result = service.GetDeliveryData(deliveryId);
            return Ok(result);
        }
    }
}
