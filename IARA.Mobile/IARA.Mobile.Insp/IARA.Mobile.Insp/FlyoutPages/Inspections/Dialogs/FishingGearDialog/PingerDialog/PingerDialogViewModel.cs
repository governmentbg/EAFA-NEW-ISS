using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
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
        }

        public List<SelectNomenclatureDto> PingerStatuses
        {
            get => _pingerStatuses;
            set => SetProperty(ref _pingerStatuses, value);
        }
        public PingerModel Pinger { get; set; }
        public ViewActivityType ViewActivityType { get; set; }
        public bool IsEditable { get; set; }

        [Required]
        public ValidState Number { get; set; }

        [Required]
        public ValidStateSelect<SelectNomenclatureDto> Status { get; set; }

        public ValidState Model { get; set; }

        public ValidState Mark { get; set; }

        public override Task Initialize(object sender)
        {
            return Task.CompletedTask;
            //return HideDialog(PingerModel);
            //throw new System.NotImplementedException();
        }
    }
}
