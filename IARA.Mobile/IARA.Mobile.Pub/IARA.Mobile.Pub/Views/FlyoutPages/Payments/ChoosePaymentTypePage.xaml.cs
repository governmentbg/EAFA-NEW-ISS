using IARA.Mobile.Pub.ViewModels.FlyoutPages.Payments;
using IARA.Mobile.Shared.Views;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Pub.Views.FlyoutPages.Payments
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChoosePaymentTypePage : BasePage<ChoosePaymentTypeViewModel>
    {
        public ChoosePaymentTypePage(int applicationId, decimal totalPrice)
        {
            ViewModel.ApplicationId = applicationId;
            ViewModel.TotalPrice = totalPrice;
            InitializeComponent();
        }
    }
}