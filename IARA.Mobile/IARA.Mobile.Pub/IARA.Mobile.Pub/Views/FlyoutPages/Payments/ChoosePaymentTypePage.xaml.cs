using IARA.Mobile.Pub.ViewModels.FlyoutPages.Payments;
using IARA.Mobile.Shared.Views;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Pub.Views.FlyoutPages.Payments
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChoosePaymentTypePage : BasePage<ChoosePaymentTypeViewModel>
    {
        public ChoosePaymentTypePage(string paymentRequestNum, decimal totalPrice)
        {
            ViewModel.PaymentRequestNum = paymentRequestNum;
            ViewModel.TotalPrice = totalPrice;
            InitializeComponent();
        }
    }
}