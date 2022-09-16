using IARA.Mobile.Pub.Application.DTObjects.ScientificFishing.LocalDb;
using IARA.Mobile.Pub.ViewModels.FlyoutPages.ScientificFishing;
using IARA.Mobile.Shared.Views;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Pub.Views.FlyoutPages.ScientificFishing
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OutingsPage : BasePage<OutingsViewModel>
    {
        public OutingsPage(int id, bool isActive, SFPermitDto permit)
        {
            ViewModel.Id = id;
            ViewModel.IsActive = isActive;
            ViewModel.Permit = permit;
            InitializeComponent();
        }
    }
}