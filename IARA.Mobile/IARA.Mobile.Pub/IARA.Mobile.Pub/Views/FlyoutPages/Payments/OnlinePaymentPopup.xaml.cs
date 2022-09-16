using IARA.Mobile.Pub.ViewModels.FlyoutPages.Payments;
using Rg.Plugins.Popup.Pages;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Pub.Views.FlyoutPages.Payments
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OnlinePaymentPopup : PopupPage
    {
        public OnlinePaymentPopup(OnlinePaymentViewModel context)
        {
            BindingContext = context;
            InitializeComponent();
            context.WebView = webView;
        }
    }
}