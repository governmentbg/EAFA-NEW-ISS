using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Base;
using System.ComponentModel.DataAnnotations;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.FishingGearDialog
{
    public class PingerViewModel : ViewModel
    {
        public PingerViewModel()
        {
            this.AddValidation();
        }

        public int? Id { get; set; }

        [Required]
        [MaxLength(50)]
        public ValidState Number { get; set; }

        [Required]
        public ValidStateSelect<SelectNomenclatureDto> Status { get; set; }

        public static implicit operator FishingGearPingerDto(PingerViewModel viewModel)
        {
            if (viewModel.Status.Value != null)
            {
                return new FishingGearPingerDto
                {
                    Id = viewModel.Id,
                    Number = viewModel.Number,
                    SelectedStatus = new NomenclatureDto
                    {
                        Value = viewModel.Status.Value.Id,
                        Code = viewModel.Status.Value.Code,
                        DisplayName = viewModel.Status.Value.Name,
                    },
                    StatusId = viewModel.Status.Value.Id,
                };
            }

            return null;
        }
    }
}
