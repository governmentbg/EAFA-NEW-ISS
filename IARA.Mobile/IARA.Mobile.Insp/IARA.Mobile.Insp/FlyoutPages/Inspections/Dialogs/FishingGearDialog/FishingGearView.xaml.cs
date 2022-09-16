using IARA.Mobile.Shared.Views;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.FishingGearDialog
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FishingGearView : SectionView
    {
        public FishingGearView(FishingGearViewModel viewModel)
        {
            BindingContext = viewModel;
            InitializeComponent();
        }
    }
}