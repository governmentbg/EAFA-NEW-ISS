using IARA.Mobile.Pub.ViewModels.FlyoutPages;
using TechnoLogica.Xamarin.Controls;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Pub.Views.FlyoutPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CompletedRegistration : TLBasePage<CompletedRegistrationViewModel>
    {
        public CompletedRegistration(string email)
        {
            ViewModel.Email = email;
            InitializeComponent();
        }
    }
}