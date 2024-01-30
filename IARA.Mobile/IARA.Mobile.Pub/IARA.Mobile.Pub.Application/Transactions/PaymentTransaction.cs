using System.Threading.Tasks;
using IARA.Mobile.Domain.Models;
using IARA.Mobile.Pub.Application.Interfaces.Transactions;
using IARA.Mobile.Pub.Application.Transactions.Base;

namespace IARA.Mobile.Pub.Application.Transactions
{
    public class PaymentTransaction : BaseTransaction, IPaymentTransaction
    {
        public PaymentTransaction(BaseTransactionProvider provider) : base(provider)
        {
        }

        public async Task<string> RegisterOfflinePayment(string paymentRequestNum)
        {
            HttpResult<string> result = await RestClient.GetAsync<string>("EGovPayments/RegisterOfflinePayment", "Integration", new { paymentRefNumber = paymentRequestNum });

            if (result.IsSuccessful)
            {
                return result.JsonResponse;
            }

            return null;
        }

        public Task MarkPaymentForProcessing(string paymentId, bool isPaymentCanceled, bool isFromEPay)
        {
            return RestClient.GetAsync<string>("PaymentsPublic/GetApplicationOriginType", "Public", new { paymentId, isPaymentCanceled, isFromEPay });
        }
    }
}
