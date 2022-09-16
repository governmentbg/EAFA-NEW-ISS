using IARA.Mobile.Pub.ViewModels.FlyoutPages.FishingTicket;
using IARA.Mobile.Shared.Views;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Pub.Views.FlyoutPages.FishingTicket
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ElderTicketPage : BasePage<ElderTicketViewModel>
    {
        public ElderTicketPage(TicketMetadataModel metadata)
        {
            ViewModel.TicketMetadata = metadata;
            InitializeComponent();
        }
    }
}