using IARA.Mobile.Pub.ViewModels.FlyoutPages.ScientificFishing;
using IARA.Mobile.Shared.Views;
using TechnoLogica.Xamarin.Extensions;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Pub.Views.FlyoutPages.ScientificFishing
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReviewPage : BasePage<ReviewViewModel>
    {
        public ReviewPage(int id)
        {
            ViewModel.Id = id;
#if DEBUG
            SafeFireExtensions.SafeFire(InitializeComponent);
#else
            InitializeComponent();
#endif
        }
    }
}