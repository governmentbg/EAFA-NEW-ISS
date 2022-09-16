using IARA.Mobile.Pub.ViewModels.FlyoutPages;
using IARA.Mobile.Shared.Views;
using TechnoLogica.Xamarin.Extensions;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Pub.Views.FlyoutPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MyTicketsPage : MainPage<MyTicketsViewModel>
    {
        public MyTicketsPage()
        {
#if DEBUG
            SafeFireExtensions.SafeFire(InitializeComponent);
#else
            InitializeComponent();
#endif
        }

        protected override string PageName => nameof(MyTicketsPage);
    }
}