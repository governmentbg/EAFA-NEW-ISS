using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Shared.Views;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.HarbourInspection
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HarbourInspectionPage : BasePage<HarbourInspectionViewModel>
    {
        public HarbourInspectionPage(ViewActivityType activityType = ViewActivityType.Add, InspectionTransboardingDto dto = null, bool isLocal = false)
        {
            ViewModel.ActivityType = activityType;
            ViewModel.Edit = dto;
            ViewModel.IsLocal = isLocal;
            InitializeComponent();
        }
    }
}