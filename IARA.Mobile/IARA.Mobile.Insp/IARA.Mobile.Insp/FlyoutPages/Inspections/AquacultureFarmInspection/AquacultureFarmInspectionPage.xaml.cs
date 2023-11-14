using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Shared.Views;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.AquacultureFarmInspection
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AquacultureFarmInspectionPage : BasePage<AquacultureFarmInspectionViewModel>
    {
        public AquacultureFarmInspectionPage(SubmitType sumbitType = SubmitType.Draft, ViewActivityType activityType = ViewActivityType.Add, InspectionAquacultureDto dto = null, bool isLocal = false)
        {
            ViewModel.SubmitType = sumbitType;
            ViewModel.ActivityType = activityType;
            ViewModel.Edit = dto;
            ViewModel.IsLocal = isLocal;
            InitializeComponent();
            ViewModel.Sections = forwardSections;
        }
    }
}