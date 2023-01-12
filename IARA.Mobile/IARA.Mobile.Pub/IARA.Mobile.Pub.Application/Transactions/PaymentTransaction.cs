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

        public async Task<string> RegisterOfflinePayment(int applicationId)
        {
            HttpResult<string> result = await RestClient.GetAsync<string>("EGovPayments/RegisterOfflinePayment", "Integration", new { paymentRefNumber = applicationId });

            if (result.IsSuccessful)
            {
                return result.JsonResponse;
            }

            return null;
        }

        public Task MarkPaymentForProcessing(int paymentId, bool isPaymentCanceled, bool isFromEPay)
        {
            return RestClient.GetAsync<string>("PaymentsPublic/GetApplicationOriginType", "Public", new { paymentId, isPaymentCanceled, isFromEPay });
        }
    }
}
