using System.Collections.Generic;
using System.Linq;
using IARA.Mobile.Pub.ViewModels.FlyoutPages.Payments;
using IARA.Mobile.Shared.Views;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Pub.Views.FlyoutPages.Payments
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProceedToPaymentPage : BasePage<ProceedToPaymentViewModel>
    {
        public ProceedToPaymentPage(List<ItemViewModel> items, int applicationId)
        {
            ViewModel.Orders.AddRange(items);
            ViewModel.ApplicationId = applicationId;
            ViewModel.TotalPrice = items.Sum(x => x.Price);
            InitializeComponent();
        }
    }
}