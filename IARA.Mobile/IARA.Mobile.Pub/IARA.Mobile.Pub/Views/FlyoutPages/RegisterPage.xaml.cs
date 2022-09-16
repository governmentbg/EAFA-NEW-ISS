using IARA.Mobile.Application.DTObjects.Users;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Pub.ViewModels.FlyoutPages;
using TechnoLogica.Xamarin.Controls;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Pub.Views.FlyoutPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage : TLBasePage<RegisterViewModel>
    {
        public RegisterPage(UserAuthDto userAuth = null)
        {
            if (userAuth != null)
            {
                ViewModel.IsEAuthLogin = userAuth.CurrentLoginType == LoginTypeEnum.EAUTH;
                ViewModel.HasUserPassLogin = userAuth.HasUserPassLogin ?? false;
                ViewModel.FirstName.Value = userAuth.FirstName;
                ViewModel.MiddleName.Value = userAuth.MiddleName;
                ViewModel.LastName.Value = userAuth.LastName;
                ViewModel.EgnLnc.Value = userAuth.EgnLnch;
                ViewModel.IsIdentifierDisabled = !string.IsNullOrEmpty(userAuth.EgnLnch);
            }

            InitializeComponent();
        }
    }
}