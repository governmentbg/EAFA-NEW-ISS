using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Shared.Views;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.InWaterOnBoard
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InWaterOnBoardInspectionPage : BasePage<InWaterOnBoardInspectionViewModel>
    {
        public InWaterOnBoardInspectionPage(ViewActivityType activityType = ViewActivityType.Add, InspectionAtSeaDto dto = null, bool isLocal = false)
        {
            ViewModel.ActivityType = activityType;
            ViewModel.Edit = dto;
            ViewModel.IsLocal = isLocal;
            InitializeComponent();
            ViewModel.Sections = forwardSections;
        }
    }
}