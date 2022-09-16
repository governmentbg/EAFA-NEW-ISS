using IARA.Mobile.Application.DTObjects.Users;
using IARA.Mobile.Pub.ViewModels.FlyoutPages;
using TechnoLogica.Xamarin.Controls;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Pub.Views.FlyoutPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ConfirmCredentialsPage : TLBasePage<ConfirmCredentialsViewModel>
    {
        public ConfirmCredentialsPage(UserAuthDto userAuth)
        {
            ViewModel.UserAuth = userAuth;
            InitializeComponent();
        }
    }
}