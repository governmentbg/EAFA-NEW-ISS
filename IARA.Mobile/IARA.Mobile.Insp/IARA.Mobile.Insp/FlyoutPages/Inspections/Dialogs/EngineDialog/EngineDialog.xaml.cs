using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Controls.ViewModels;
using IARA.Mobile.Insp.Domain.Enums;
using TechnoLogica.Xamarin.Controls;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.EngineDialog
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EngineDialog : TLBaseDialog<EngineDialogViewModel, WaterInspectionEngineDto>
    {
        public EngineDialog(EnginesViewModel engines, InspectionPageViewModel inspection, ViewActivityType dialogType, WaterInspectionEngineDto dto = null)
        {
            ViewModel.DialogType = dialogType;
            ViewModel.Engines = engines;
            ViewModel.Inspection = inspection;
            ViewModel.Edit = dto;
            InitializeComponent();
        }
    }
}