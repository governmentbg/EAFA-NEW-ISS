using IARA.Mobile.Shared.ViewModels;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Shared.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReportsPage : BasePage<ReportsViewModel>
    {
        public ReportsPage()
        {
            InitializeComponent();
        }
    }
}