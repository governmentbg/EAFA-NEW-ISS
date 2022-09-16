using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Controls.ViewModels;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Models;
using TechnoLogica.Xamarin.Controls;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.InspectorDialog
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InspectorDialog : TLBaseDialog<InspectorDialogViewModel, InspectorModel>
    {
        public InspectorDialog(InspectorsViewModel inspectors, InspectionPageViewModel inspection, ViewActivityType dialogType, InspectorModel dto = null)
        {
            ViewModel.DialogType = dialogType;
            ViewModel.Inspectors = inspectors;
            ViewModel.Inspection = inspection;
            ViewModel.Edit = dto;
            ViewModel.IsCurrentUser = dto?.IsCurrentInspector ?? false;
            InitializeComponent();
        }
    }
}