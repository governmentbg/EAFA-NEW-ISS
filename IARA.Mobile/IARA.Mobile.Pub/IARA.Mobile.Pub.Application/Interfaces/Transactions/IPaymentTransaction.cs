using System.Threading.Tasks;

namespace IARA.Mobile.Pub.Application.Interfaces.Transactions
{
    public interface IPaymentTransaction
    {
        Task<string> RegisterOfflinePayment(string paymentRequestNum);

        Task MarkPaymentForProcessing(string paymentId, bool isPaymentCanceled, bool isFromEPay);
    }
}
