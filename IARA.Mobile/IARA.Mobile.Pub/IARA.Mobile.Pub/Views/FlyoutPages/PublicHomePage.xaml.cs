using IARA.Mobile.Pub.ViewModels.FlyoutPages;
using TechnoLogica.Xamarin.Controls;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Pub.Views.FlyoutPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PublicHomePage : TLBasePage<PublicHomeViewModel>
    {
        public PublicHomePage()
        {
            InitializeComponent();
        }
    }
}