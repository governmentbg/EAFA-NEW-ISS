using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Controls.ViewModels;
using IARA.Mobile.Insp.Domain.Enums;
using TechnoLogica.Xamarin.Controls;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.WaterVesselDialog
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WaterVesselDialog : TLBaseDialog<WaterVesselDialogViewModel, WaterInspectionVesselDto>
    {
        public WaterVesselDialog(WaterVesselsViewModel vessels, InspectionPageViewModel inspection, ViewActivityType dialogType, WaterInspectionVesselDto dto = null)
        {
            ViewModel.DialogType = dialogType;
            ViewModel.Vessels = vessels;
            ViewModel.Inspection = inspection;
            ViewModel.Edit = dto;
            InitializeComponent();
        }
    }
}