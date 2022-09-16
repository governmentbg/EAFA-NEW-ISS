using IARA.Mobile.Pub.ViewModels.FlyoutPages.FishingTicket;
using IARA.Mobile.Shared.Views;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Pub.Views.FlyoutPages.FishingTicket
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Between14and18AssociationTicketPage : BasePage<Between14and18AssociationTicketViewModel>
    {
        public Between14and18AssociationTicketPage(TicketMetadataModel metadata)
        {
            ViewModel.TicketMetadata = metadata;
            InitializeComponent();
        }
    }
}