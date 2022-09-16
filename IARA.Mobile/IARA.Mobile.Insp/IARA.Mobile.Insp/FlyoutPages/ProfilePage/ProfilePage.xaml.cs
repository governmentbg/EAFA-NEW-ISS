using IARA.Mobile.Shared.Views;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.FlyoutPages.ProfilePage
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : BasePage<ProfileViewModel>
    {
        public ProfilePage()
        {
            InitializeComponent();
        }
    }
}