using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Controls.ViewModels;
using IARA.Mobile.Insp.Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Base;
using TechnoLogica.Xamarin.ViewModels.Interfaces;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.OffenderDialog
{
    public class OffenderDialogViewModel : TLBaseDialogViewModel<InspectionSubjectPersonnelDto>
    {
        private PersonViewModel _offender;
        public OffenderDialogViewModel()
        {
            Save = CommandBuilder.CreateFrom(OnSave);
        }

        public override Task Initialize(object sender)
        {
            Offender = new PersonViewModel(Inspection, InspectedPersonType.Offender, false);
            Offender.Init(Counties);
            if (DialogType != ViewActivityType.Add)
            {
                Offender.OnEdit(Edit);
            }
            this.AddValidation(others: new IValidatableViewModel[]
            {
                Offender,
            });

            return Task.CompletedTask;
        }

        public InspectionPageViewModel Inspection { get; set; }
        public ViewActivityType DialogType { get; set; }
        public InspectionSubjectPersonnelDto Edit { get; set; }
        public List<SelectNomenclatureDto> Counties { get; set; }
        public PersonViewModel Offender
        {
            get => _offender;
            set => SetProperty(ref _offender, value);
        }

        public ICommand Save { get; set; }

        private Task OnSave()
        {
            Validation.Force();
            if (Validation.IsValid)
            {
                return HideDialog(Offender);
            }

            return Task.CompletedTask;
        }
    }
}
