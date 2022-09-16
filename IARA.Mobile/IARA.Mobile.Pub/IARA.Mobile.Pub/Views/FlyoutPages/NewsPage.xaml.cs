using IARA.Mobile.Pub.ViewModels.FlyoutPages;
using IARA.Mobile.Shared.Views;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Pub.Views.FlyoutPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewsPage : MainPage<NewsViewModel>
    {
        public NewsPage()
        {
            InitializeComponent();
        }

        protected override string PageName => nameof(NewsPage);
    }
}