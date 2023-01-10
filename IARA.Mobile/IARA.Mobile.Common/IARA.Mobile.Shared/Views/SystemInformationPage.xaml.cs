using IARA.Mobile.Shared.ViewModels;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Shared.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SystemInformationPage : BasePage<SystemInformationViewModel>
    {
        public SystemInformationPage()
        {
            InitializeComponent();
        }
    }
}