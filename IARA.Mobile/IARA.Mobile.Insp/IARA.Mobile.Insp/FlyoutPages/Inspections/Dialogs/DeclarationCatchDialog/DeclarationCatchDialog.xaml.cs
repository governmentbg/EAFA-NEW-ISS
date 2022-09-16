using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Controls.ViewModels;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Models;
using TechnoLogica.Xamarin.Controls;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.DeclarationCatchDialog
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DeclarationCatchDialog : TLBaseDialog<DeclarationCatchDialogViewModel, DeclarationCatchModel>
    {
        public DeclarationCatchDialog(DeclarationCatchesViewModel catches, InspectionPageViewModel inspection, bool hasCatchType, ViewActivityType dialogType, DeclarationCatchModel dto = null)
        {
            ViewModel.DialogType = dialogType;
            ViewModel.Catches = catches;
            ViewModel.Inspection = inspection;
            ViewModel.HasCatchType = hasCatchType;
            ViewModel.Edit = dto;
            ViewModel.OnInit();

            InitializeComponent();
        }
    }
}