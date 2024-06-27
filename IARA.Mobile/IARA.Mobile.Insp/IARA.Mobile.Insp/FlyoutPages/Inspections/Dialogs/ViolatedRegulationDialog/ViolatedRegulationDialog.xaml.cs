using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Models;
using TechnoLogica.Xamarin.Controls;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.ViolatedRegulationDialog
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ViolatedRegulationDialog : TLBaseDialog<ViolatedRegulationDialogViewModel, ViolatedRegulationModel>
    {
        public ViolatedRegulationDialog(ViewActivityType dialogType, ViolatedRegulationModel model)
        {
            ViewModel.DialogType = dialogType;
            ViewModel.Edit = model;

            InitializeComponent();
        }
    }
}