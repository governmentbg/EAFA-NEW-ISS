using System;
using IARA.WebHelpers;
using IARA.WebHelpers.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using TL.EPayPayments.ControllerModels;
using TL.EPayPayments.Interfaces;
using TL.EPayPayments.Models;

namespace IARA.Web.Controllers.Integration
{
    [AreaRoute(AreaType.Integration)]
    public class EpayController : TL.EPayPayments.Controllers.EpayController
    {
        public EpayController(IConfiguration configuration)
            : base(configuration)
        { }

        [HttpPost]
        [CustomAuthorize]
        public override IActionResult GeneratePaymentRequest([FromBody] EPaymentRequestModel paymentRequest, [FromServices] IEPayInvoiceGenerationService service)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        [CustomAuthorize]
        public override IActionResult GeneratePaymentRequestByInvoiceNumber([FromBody] InvoicePaymentRequestModel invoicePaymentRequest, [FromServices] IEPayPaymentDataService service)
        {
            return base.GeneratePaymentRequestByInvoiceNumber(invoicePaymentRequest, service);
        }

        [HttpPost]
        [AllowAnonymous]
        public override IActionResult Notification([FromForm] EncodedNotificationModel notification, [FromServices] IEPayCallbackService service)
        {
            return base.Notification(notification, service);
        }

        protected override void ActionExecuting(ActionExecutingContext context)
        {
            context.SetCurrentPrincipal(true, "EPAY");
            base.ActionExecuting(context);
        }
    }
}
