using IARA.Mobile.Pub.ViewModels.FlyoutPages.Payments;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Pub.Views.FlyoutPages.Payments
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PayEGovBankPopup : PopupPage
    {
        public PayEGovBankPopup(PayEGovBankViewModel context)
        {
            BindingContext = context;
            InitializeComponent();
        }
    }
}