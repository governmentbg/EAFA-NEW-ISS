using System.Threading.Tasks;
using IARA.Logging.Abstractions.Interfaces;
using IARA.WebHelpers;
using IARA.WebHelpers.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using TL.EGovPayments.ControllerModels;
using TL.EGovPayments.Interfaces;
using TL.EGovPayments.JsonEnums;
using TL.EGovPayments.JsonModels;

namespace IARA.Web.Controllers.Public
{
    [AreaRoute(AreaType.Integration)]
    public class EGovPaymentsController : TL.EGovPayments.Controllers.EGovPaymentsController
    {
        private IExtendedLogger logger;

        public EGovPaymentsController(IConfiguration configuration,
                                      IEGovPaymentService paymentService,
                                      IEGovIntegrationService egovIntegrationService,
                                      IExtendedLogger logger)
            : base(configuration, paymentService, egovIntegrationService)
        {
            this.logger = logger;
        }

        [HttpGet]
        [CustomAuthorize]
        public override IActionResult CreateVPOSPayment(string paymentId, VPOSPaymentTypes paymentType, bool mobilePayment = false)
        {
            return base.CreateVPOSPayment(paymentId, paymentType, mobilePayment);
        }

        [HttpPost]
        [CustomAuthorize]
        public override Task<IActionResult> CreateVPOSPayment([FromBody] EGovPaymentRequestModel paymentRequest)
        {
            return base.CreateVPOSPayment(paymentRequest);
        }

        [HttpGet]
        [CustomAuthorize]
        public override Task<IActionResult> CreateVPOSPaymentByRefNumber([FromQuery] string paymentRefNumber, [FromQuery] VPOSPaymentTypes paymentType, [FromQuery] bool mobilePayment = false)
        {
            return base.CreateVPOSPaymentByRefNumber(paymentRefNumber, paymentType, mobilePayment);
        }

        [HttpPost]
        [CustomAuthorize]
        public override Task<IActionResult> RegisterOfflinePayment([FromBody] EGovPaymentRequestModel paymentRequest)
        {
            return base.RegisterOfflinePayment(paymentRequest);
        }

        [HttpGet]
        [CustomAuthorize]
        public override Task<IActionResult> RegisterOfflinePayment([FromQuery] string paymentRefNumber)
        {
            return base.RegisterOfflinePayment(paymentRefNumber);
        }

        public override IActionResult PaymentStatusCallback([FromForm] MessageWrapper<PaymentStatus> message)
        {
            logger.LogInfo($"Payment Status Changed ClientId: {message.ClientId} Data: {message.Data}");
            return base.PaymentStatusCallback(message);
        }

        protected override void ActionExecuting(ActionExecutingContext context)
        {
            context.SetCurrentPrincipal(true, "EGOV");
            base.ActionExecuting(context);
        }
    }
}
