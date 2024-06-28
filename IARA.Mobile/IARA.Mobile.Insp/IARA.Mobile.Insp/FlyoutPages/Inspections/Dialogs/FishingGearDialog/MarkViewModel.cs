using IARA.Mobile.Application.Attributes;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.FishingGearDialog
{
    public class MarkViewModel : ViewModel
    {
        public MarkViewModel()
        {
            this.AddValidation();
        }

        public int? Id { get; set; }

        public bool AddedByInspector { get; set; }

        public DateTime? CreatedOn { get; set; }

        [Required]
        [MaxLength(50)]
        [TLRange(0, 1_000_000_000)]
        public ValidState Number { get; set; }

        public string Prefix { get; set; }

        [Required]
        public ValidStateSelect<SelectNomenclatureDto> Status { get; set; }

        public static implicit operator FishingGearMarkDto(MarkViewModel viewModel)
        {
            if (viewModel.Status.Value != null)
            {
                return new FishingGearMarkDto
                {
                    Id = viewModel.Id,
                    IsActive = true,
                    FullNumber = new PrefixInputDto
                    {
                        Prefix = viewModel.Prefix,
                        InputValue = viewModel.Number
                    },
                    SelectedStatus = Enum.TryParse(viewModel.Status.Value.Code, out FishingGearMarkStatus status)
                        ? status
                        : FishingGearMarkStatus.MARKED,
                    StatusId = viewModel.Status.Value.Id,
                    CreatedOn = viewModel.CreatedOn,
                };
            }

            return null;
        }
    }
}
