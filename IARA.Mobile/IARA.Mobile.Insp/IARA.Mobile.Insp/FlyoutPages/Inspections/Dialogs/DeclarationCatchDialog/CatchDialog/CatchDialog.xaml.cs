using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Domain.Enums;
using System.Collections.Generic;
using TechnoLogica.Xamarin.Controls;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.DeclarationCatchDialog.CatchDialog
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CatchDialog : TLBaseDialog<CatchDialogViewModel, InspectedDeclarationCatchDto>
    {
        public CatchDialog(ViewActivityType dialogType, InspectedDeclarationCatchDto edit, List<SelectNomenclatureDto> fishes, List<SelectNomenclatureDto> presentations)
        {
            ViewModel.Edit = edit;
            ViewModel.DialogType = dialogType;
            ViewModel.FishTypes = fishes;
            ViewModel.Presentations = presentations;

            InitializeComponent();
        }
    }
}