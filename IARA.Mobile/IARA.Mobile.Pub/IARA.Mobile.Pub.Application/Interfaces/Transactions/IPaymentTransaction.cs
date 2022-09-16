using System.Threading.Tasks;

namespace IARA.Mobile.Pub.Application.Interfaces.Transactions
{
    public interface IPaymentTransaction
    {
        Task<string> RegisterOfflinePayment(int applicationId);

        Task MarkPaymentForProcessing(int paymentId, bool isPaymentCanceled, bool isFromEPay);
    }
}
