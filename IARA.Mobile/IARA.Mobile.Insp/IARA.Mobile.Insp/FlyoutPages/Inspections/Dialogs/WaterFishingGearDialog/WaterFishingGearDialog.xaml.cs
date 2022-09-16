using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Controls.ViewModels;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Models;
using TechnoLogica.Xamarin.Controls;
using Xamarin.Forms.Xaml;
namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.WaterFishingGearDialog
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WaterFishingGearDialog : TLBaseDialog<WaterFishingGearDialogViewModel, WaterFishingGearModel>
    {
        public WaterFishingGearDialog(WaterFishingGearsViewModel waterFishingGears, InspectionPageViewModel inspection, ViewActivityType dialogType, WaterFishingGearModel dto = null)
        {
            ViewModel.DialogType = dialogType;
            ViewModel.WaterFishingGears = waterFishingGears;
            ViewModel.Inspection = inspection;
            ViewModel.Edit = dto;
            InitializeComponent();
        }
    }
}