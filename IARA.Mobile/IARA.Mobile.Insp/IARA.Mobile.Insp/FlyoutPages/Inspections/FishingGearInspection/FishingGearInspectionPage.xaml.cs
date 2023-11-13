using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Shared.Views;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.FishingGearInspection
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FishingGearInspectionPage : BasePage<FishingGearInspectionViewModel>
    {
        public FishingGearInspectionPage(SubmitType submitType = SubmitType.Draft, ViewActivityType activityType = ViewActivityType.Add, InspectionCheckToolMarkDto dto = null, bool isLocal = false)
        {
            ViewModel.SubmitType = submitType;
            ViewModel.ActivityType = activityType;
            ViewModel.Edit = dto;
            ViewModel.IsLocal = isLocal;
            InitializeComponent();
            ViewModel.Sections = forwardSections;
        }
    }
}