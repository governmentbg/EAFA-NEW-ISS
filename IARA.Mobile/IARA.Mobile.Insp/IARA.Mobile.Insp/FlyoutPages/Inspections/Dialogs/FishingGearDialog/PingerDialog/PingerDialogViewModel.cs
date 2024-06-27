using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Insp.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Base;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.FishingGearDialog.PingerDialog
{
    public class PingerDialogViewModel : TLBaseDialogViewModel<PingerModel>
    {
        private List<SelectNomenclatureDto> _pingerStatuses;
        public PingerDialogViewModel()
        {
            this.AddValidation();
            Save = CommandBuilder.CreateFrom(OnSave);
        }

        public PingerModel Pinger { get; set; }
        public ViewActivityType ViewActivityType { get; set; }
        public bool IsEditable { get; set; }

        [Required]
        public ValidState Number { get; set; }

        [Required]
        public ValidStateSelect<SelectNomenclatureDto> Status { get; set; }

        public ValidState Model { get; set; }

        public ValidState Brand { get; set; }

        public List<SelectNomenclatureDto> PingerStatuses
        {
            get => _pingerStatuses;
            set => SetProperty(ref _pingerStatuses, value);
        }

        public ICommand Save { get; set; }

        public override Task Initialize(object sender)
        {
            if (ViewActivityType != ViewActivityType.Add)
            {
                Number.AssignFrom(Pinger.Number);
                Status.AssignFrom(Pinger.Status.Code, PingerStatuses);
                Model.AssignFrom(Pinger.Model);
                Brand.AssignFrom(Pinger.Brand);
            }
            return Task.CompletedTask;
        }

        private Task OnSave()
        {
            Validation.Force();
            if (!Validation.IsValid)
            {
                return Task.CompletedTask;
            }

            return HideDialog(new PingerModel()
            {
                Id = Pinger != null ? Pinger.Id : null,
                Number = Number.Value,
                Status = Status.Value,
                Model = Model.Value,
                Brand = Brand.Value
            });
        }
    }
}
