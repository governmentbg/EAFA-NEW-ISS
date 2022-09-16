using IARA.Common.Enums;
using IARA.Interfaces;
using IARA.Security;
using IARA.WebHelpers;
using IARA.WebHelpers.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace IARA.Web.Controllers.Public
{
    [AreaRoute(AreaType.Public)]
    public class PaymentsPublicController : BaseController
    {
        private readonly IPaymentsService paymentsService;

        public PaymentsPublicController(IPermissionsService permissionsService, IPaymentsService paymentsService)
            : base(permissionsService)
        {
            this.paymentsService = paymentsService;
        }

        [HttpGet]
        [CustomAuthorize]
        public IActionResult GetApplicationOriginType([FromQuery] string paymentId, [FromQuery] bool isPaymentCanceled, [FromQuery] bool isFromEPay)
        {
            ApplicationHierarchyTypesEnum hierarchyType = this.paymentsService.MovePaymentStatus(paymentId, isPaymentCanceled, isFromEPay);

            return Ok(hierarchyType);
        }
    }
}
