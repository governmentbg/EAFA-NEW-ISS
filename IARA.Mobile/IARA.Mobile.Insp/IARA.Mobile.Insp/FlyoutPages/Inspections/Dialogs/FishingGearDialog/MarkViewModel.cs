using AutoMapper;
using IARA.Mobile.Application.Attributes;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Attributes;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.FishingGearDialog
{
    public class MarkViewModel : ViewModel
    {
        private bool _isSameAsInspected;
        public MarkViewModel(bool addedByInspector = false)
        {
            this.AddValidation();
            AddedByInspector.Value = addedByInspector;
        }

        public bool IsSameAsInspected
        {
            get => _isSameAsInspected;
            set => SetProperty(ref _isSameAsInspected, value);
        }

        public int? Id { get; set; }

        public ValidStateBool AddedByInspector { get; set; }

        public DateTime? CreatedOn { get; set; }

        [Required]
        [NoRepeatingMarkNumbers(nameof(AddedByInspector), ErrorMessageResourceName = "NoRepeatingMarkNumbers")]
        public ValidState Number { get; set; }

        public string Prefix { get; set; }

        [Required]
        public ValidStateSelect<SelectNomenclatureDto> Status { get; set; }

        public bool IsValid
        {
            get
            {
                if (AddedByInspector == true)
                {
                    if (FishingGearDialogViewModel.Instance == null)
                    {
                        return true;
                    }
                    List<MarkViewModel> marks = FishingGearDialogViewModel.Instance.InspectedFishingGear.Marks.ToList();
                    int count = marks.Where(x => x.Number.Value == Number.Value).Count();
                    if (count == 1)
                    {
                        if (!string.IsNullOrEmpty(Number.Value))
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                if (!string.IsNullOrEmpty(Number.Value) && Status.Value != null)
                {
                    return true;
                }
                return false;
            }
        }

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
