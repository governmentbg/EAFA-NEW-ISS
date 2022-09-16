using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Controls.ViewModels;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Models;
using TechnoLogica.Xamarin.Controls;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.WaterCatchDialog
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WaterCatchDialog : TLBaseDialog<WaterCatchViewModel, WaterCatchModel>
    {
        public WaterCatchDialog(WaterCatchesViewModel waterCatches, InspectionPageViewModel inspection, ViewActivityType dialogType, WaterCatchModel dto = null)
        {
            ViewModel.DialogType = dialogType;
            ViewModel.WaterCatches = waterCatches;
            ViewModel.Inspection = inspection;
            ViewModel.Edit = dto;
            InitializeComponent();
        }
    }
}