using IARA.Mobile.Application.Attributes;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Base;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.DeclarationCatchDialog.CatchDialog
{
    public class CatchDialogViewModel : TLBaseDialogViewModel<InspectedDeclarationCatchDto>
    {
        private List<SelectNomenclatureDto> _fishTypes;
        private List<SelectNomenclatureDto> _presentations;
        public CatchDialogViewModel()
        {
            Save = CommandBuilder.CreateFrom(OnSave);
            this.AddValidation();
        }
        public override Task Initialize(object sender)
        {
            if (Edit != null)
            {
                FishType.AssignFrom(Edit.FishTypeId, FishTypes);
                CatchCount.AssignFrom(Edit.CatchCount);
                CatchQuantity.AssignFrom(Edit.CatchQuantity);
                Presentation.AssignFrom(Edit.PresentationId, Presentations);
            }
            return Task.CompletedTask;
        }
        public ViewActivityType DialogType { get; set; }
        public InspectedDeclarationCatchDto Edit { get; set; }

        public List<SelectNomenclatureDto> FishTypes
        {
            get => _fishTypes;
            set => SetProperty(ref _fishTypes, value);
        }
        public List<SelectNomenclatureDto> Presentations
        {
            get => _presentations;
            set => SetProperty(ref _presentations, value);
        }

        [Required]
        public ValidStateSelect<SelectNomenclatureDto> FishType { get; set; }

        [TLRange(0, 10000)]
        public ValidState CatchCount { get; set; }

        [Required]
        [TLRange(0, 10000, true)]
        public ValidState CatchQuantity { get; set; }

        [Required]
        public ValidStateSelect<SelectNomenclatureDto> Presentation { get; set; }

        public ICommand Save { get; set; }


        private Task OnSave()
        {
            Validation.Force();
            if (Validation.IsValid)
            {
                return HideDialog(new InspectedDeclarationCatchDto
                {
                    FishTypeId = FishType.Value,
                    CatchCount = ParseHelper.ParseInteger(CatchCount),
                    CatchQuantity = ParseHelper.ParseInteger(CatchQuantity),
                    PresentationId = Presentation.Value
                });
            }

            return Task.CompletedTask;
        }
    }
}
