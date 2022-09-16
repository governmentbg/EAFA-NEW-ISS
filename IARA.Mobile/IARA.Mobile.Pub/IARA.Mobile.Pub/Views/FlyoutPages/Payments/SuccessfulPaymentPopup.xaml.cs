using IARA.Mobile.Shared.Menu;
using Rg.Plugins.Popup.Pages;
using System.Threading.Tasks;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Pub.Views.FlyoutPages.Payments
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SuccessfulPaymentPopup : PopupPage
    {
        public SuccessfulPaymentPopup()
        {
            InitializeComponent();
        }

        private void PopupPage_BackgroundClicked(object sender, System.EventArgs e)
        {
            MainNavigator.Current.GoToPageAsync(nameof(HomePage));
        }
    }
}