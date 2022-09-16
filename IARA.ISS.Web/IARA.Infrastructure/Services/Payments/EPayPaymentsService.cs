using System;
using IARA.DataAccess;
using IARA.Interfaces.FSM;
using IARA.Logging.Abstractions.Interfaces;
using TL.EPayPayments.Enums;
using TL.EPayPayments.Interfaces;
using TL.EPayPayments.Models;

namespace IARA.Infrastructure.Services.Payments
{
    public class EPayPaymentsService : BasePaymentService, IEPayCallbackService, IEPayPaymentDataService
    {
        public EPayPaymentsService(IARADbContext dbContext, IApplicationStateMachine stateMachine, IExtendedLogger logger)
            : base(dbContext, stateMachine, logger)
        { }

        public EPaymentResponseStatuses InvoiceReceived(EPayNotificationModel payment)
        {
            try
            {
                int applicationId = int.Parse(payment.Invoice);
                MovePaymentStatus(applicationId, payment.Status != EPayInvoiceStatuses.PAID);

                if (payment.Status == EPayInvoiceStatuses.PAID)
                {
                    MarkPaymentAsPaid(applicationId, payment.Invoice, payment.Status.ToString(), payment.PayTime);
                }
                else
                {
                    MarkPaymentAsAnnuled(applicationId, payment.Status.ToString());
                }

                return EPaymentResponseStatuses.OK;
            }
            catch (Exception)
            {
                return EPaymentResponseStatuses.ERR;
            }
        }

        public PaymentDetails GetPaymentData(string invoiceNumber)
        {
            int applicationId = int.Parse(invoiceNumber);
            var paymentDetails = GetPaymentDetails(applicationId);

            return new PaymentDetails
            {
                Amount = (float)paymentDetails.Amount,
                Description = paymentDetails.Description,
            };
        }
    }
}
