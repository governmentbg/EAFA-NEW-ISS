using IARA.Mobile.Pub.ViewModels.FlyoutPages.FishingTicket;
using IARA.Mobile.Shared.Views;
using TechnoLogica.Xamarin.Extensions;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Pub.Views.FlyoutPages.FishingTicket
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AssociationTicketPage : BasePage<AssociationTicketViewModel>
    {
        public AssociationTicketPage(TicketMetadataModel metadata)
        {
            ViewModel.TicketMetadata = metadata;
#if DEBUG
            SafeFireExtensions.SafeFire(InitializeComponent);
#else
            InitializeComponent();
#endif
        }
    }
}