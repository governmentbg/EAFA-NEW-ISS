using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Shared.Views;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.VehicleInspection
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VehicleInspectionPage : BasePage<VehicleInspectionViewModel>
    {
        public VehicleInspectionPage(ViewActivityType activityType = ViewActivityType.Add, InspectionTransportVehicleDto dto = null, bool isLocal = false)
        {
            ViewModel.ActivityType = activityType;
            ViewModel.Edit = dto;
            ViewModel.IsLocal = isLocal;
            InitializeComponent();
        }
    }
}