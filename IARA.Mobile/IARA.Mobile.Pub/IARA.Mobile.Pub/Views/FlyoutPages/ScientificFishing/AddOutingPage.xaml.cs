using IARA.Mobile.Pub.Application.DTObjects.ScientificFishing.LocalDb;
using IARA.Mobile.Pub.ViewModels.FlyoutPages.ScientificFishing;
using IARA.Mobile.Shared.Views;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Pub.Views.FlyoutPages.ScientificFishing
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddOutingPage : BasePage<AddOutingViewModel>
    {
        public AddOutingPage(int id, bool canEdit, SFPermitDto permit, SFOutingDto outing = null)
        {
            ViewModel.Id = id;
            ViewModel.Edit = outing;
            ViewModel.CanEdit = canEdit;
            ViewModel.Permit = permit;
            InitializeComponent();
        }
    }
}