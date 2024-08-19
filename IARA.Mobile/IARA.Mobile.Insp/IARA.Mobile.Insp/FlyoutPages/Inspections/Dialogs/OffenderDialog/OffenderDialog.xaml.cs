using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using System.Collections.Generic;
using TechnoLogica.Xamarin.Controls;
using Xamarin.Forms.Xaml;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.OffenderDialog
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OffenderDialog : TLBaseDialog<OffenderDialogViewModel, InspectionSubjectPersonnelDto>
    {
        public OffenderDialog(InspectionPageViewModel inspection, List<SelectNomenclatureDto> countries, ViewActivityType dialogType, InspectionSubjectPersonnelDto dto = null)
        {
            ViewModel.Inspection = inspection;
            ViewModel.DialogType = dialogType;
            ViewModel.Edit = dto;
            ViewModel.Counties = countries;

            InitializeComponent();
        }
    }
}