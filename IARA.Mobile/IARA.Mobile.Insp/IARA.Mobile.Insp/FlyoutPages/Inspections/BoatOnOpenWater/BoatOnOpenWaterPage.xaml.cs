using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Shared.Views;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.BoatOnOpenWater
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BoatOnOpenWaterPage : BasePage<BoatOnOpenWaterViewModel>
    {
        public BoatOnOpenWaterPage(ViewActivityType activityType = ViewActivityType.Add, ObservationAtSeaDto dto = null, bool isLocal = false)
        {
            ViewModel.ActivityType = activityType;
            ViewModel.Edit = dto;
            ViewModel.IsLocal = isLocal;
            InitializeComponent();
            ViewModel.Sections = forwardSections;
        }
    }
}