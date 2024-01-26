using System.Threading.Tasks;
using System.Windows.Input;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Pub.ViewModels.Base;
using IARA.Mobile.Pub.Views.FlyoutPages.Payments;
using IARA.Mobile.Shared.Menu;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;

namespace IARA.Mobile.Pub.ViewModels.FlyoutPages.Payments
{
    public class ProceedToPaymentViewModel : PageViewModel
    {
        public ProceedToPaymentViewModel()
        {
            Orders = new TLObservableCollection<ItemViewModel>();
            Checkout = CommandBuilder.CreateFrom(OnCheckout);
        }
        public string PaymentRequestNum { get; set; }
        public decimal TotalPrice { get; set; }
        public TLObservableCollection<ItemViewModel> Orders { get; }
        public ICommand Checkout { get; }
        public override GroupResourceEnum[] GetPageIndexes()
        {
            return new[] {
                GroupResourceEnum.Payments,
            };
        }

        public override Task Initialize(object sender)
        {
            return Task.CompletedTask;
        }

        private async Task OnCheckout()
        {
            await MainNavigator.Current.GoToPageAsync(new ChoosePaymentTypePage(PaymentRequestNum, TotalPrice));
        }
    }
}