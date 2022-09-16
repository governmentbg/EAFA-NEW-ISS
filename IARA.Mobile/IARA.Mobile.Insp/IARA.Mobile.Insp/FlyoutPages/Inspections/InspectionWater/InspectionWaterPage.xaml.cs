using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Shared.Views;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.InspectionWater
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InspectionWaterPage : BasePage<InspectionWaterViewModel>
    {
        public InspectionWaterPage(ViewActivityType activityType = ViewActivityType.Add, InspectionCheckWaterObjectDto dto = null, bool isLocal = false)
        {
            ViewModel.ActivityType = activityType;
            ViewModel.Edit = dto;
            ViewModel.IsLocal = isLocal;
            InitializeComponent();
        }
    }
}