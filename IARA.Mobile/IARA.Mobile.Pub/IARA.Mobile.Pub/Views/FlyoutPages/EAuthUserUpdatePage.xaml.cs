using IARA.Mobile.Application.DTObjects.Users;
using IARA.Mobile.Pub.ViewModels.FlyoutPages;
using TechnoLogica.Xamarin.Controls;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Pub.Views.FlyoutPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EAuthUserUpdatePage : TLBasePage<EAuthUserUpdateViewModel>
    {
        public EAuthUserUpdatePage(UserAuthDto userAuth = null)
        {
            if (userAuth != null)
            {
                ViewModel.EgnLnc.Value = userAuth.EgnLnch;
            }
            InitializeComponent();
        }
    }
}