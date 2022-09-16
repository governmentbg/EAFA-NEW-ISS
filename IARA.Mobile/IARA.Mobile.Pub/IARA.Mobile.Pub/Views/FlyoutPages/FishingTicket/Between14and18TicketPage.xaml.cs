using IARA.Mobile.Pub.ViewModels.FlyoutPages.FishingTicket;
using IARA.Mobile.Shared.Views;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Pub.Views.FlyoutPages.FishingTicket
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Between14and18TicketPage : BasePage<Between14and18TicketViewModel>
    {
        public Between14and18TicketPage(TicketMetadataModel metadata)
        {
            ViewModel.TicketMetadata = metadata;
            InitializeComponent();
        }
    }
}