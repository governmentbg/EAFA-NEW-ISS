using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Controls.ViewModels;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Models;
using TechnoLogica.Xamarin.Controls;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.WaterVesselDialog
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WaterVesselDialog : TLBaseDialog<WaterVesselDialogViewModel, WaterVesselModel>
    {
        public WaterVesselDialog(WaterVesselsViewModel vessels, InspectionPageViewModel inspection, ViewActivityType dialogType, WaterVesselModel dto = null)
        {
            ViewModel.DialogType = dialogType;
            ViewModel.Vessels = vessels;
            ViewModel.Inspection = inspection;
            ViewModel.Edit = dto;
            InitializeComponent();
        }
    }
}