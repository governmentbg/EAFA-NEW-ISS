using IARA.Common.Enums;

namespace IARA.Interfaces
{
    public interface IPaymentsService
    {
        ApplicationHierarchyTypesEnum MovePaymentStatus(string paymentId, bool isPaymentCanceled, bool isFromEPay);
    }
}
