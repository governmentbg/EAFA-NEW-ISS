using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Controls.ViewModels;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Models;
using TechnoLogica.Xamarin.Controls;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.PatrolVehicleDialog
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PatrolVehicleDialog : TLBaseDialog<PatrolVehicleDialogViewModel, PatrolVehicleModel>
    {
        public PatrolVehicleDialog(PatrolVehiclesViewModel patrolVehicles, bool? isWaterVehicle, InspectionPageViewModel inspection, ViewActivityType dialogType, PatrolVehicleModel dto = null)
        {
            ViewModel.IsWaterVehicle = isWaterVehicle;
            ViewModel.DialogType = dialogType;
            ViewModel.PatrolVehicles = patrolVehicles;
            ViewModel.Inspection = inspection;
            ViewModel.Edit = dto;
            InitializeComponent();
        }
    }
}