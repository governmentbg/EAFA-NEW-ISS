using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Shared.Views;
using System;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.VehicleInspection
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class VehicleInspectionPage : BasePage<VehicleInspectionViewModel>
    {
        public VehicleInspectionPage(DateTime CreatedOn, SubmitType submitType = SubmitType.Draft, ViewActivityType activityType = ViewActivityType.Add, InspectionTransportVehicleDto dto = null, bool isLocal = false, bool createdByCurrentUser = true)
        {
            ViewModel.SubmitType = submitType;
            ViewModel.ActivityType = activityType;
            ViewModel.Edit = dto;
            ViewModel.IsLocal = isLocal;
            ViewModel.CreatedByCurrentUser = createdByCurrentUser;
            ViewModel.CreatedOn = CreatedOn;
            InitializeComponent();
            ViewModel.Sections = forwardSections;
        }
    }
}