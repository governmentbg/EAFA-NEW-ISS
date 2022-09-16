using IARA.Mobile.Pub.ViewModels.FlyoutPages;
using IARA.Mobile.Shared.Views;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Pub.Views.FlyoutPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : MainPage<ProfileViewModel>
    {
        public ProfilePage()
        {
            InitializeComponent();
        }

        protected override string PageName => nameof(ProfilePage);
    }
}