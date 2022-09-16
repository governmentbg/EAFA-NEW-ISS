using IARA.Mobile.Shared.Views;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.FlyoutPages.InspectionsPage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InspectionsPage : BasePage<InspectionsViewModel>
    {
        public InspectionsPage()
        {
            InitializeComponent();
        }
    }
}