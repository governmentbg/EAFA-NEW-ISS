using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Attributes;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.FishingGearDialog
{
    public class PingerViewModel : ViewModel
    {
        private bool _isSameAsInspected;
        public PingerViewModel(bool addedByInspector = false)
        {
            this.AddValidation();
            AddedByInspector.Value = addedByInspector;
        }
        public int? Id { get; set; }

        [Required]
        [NoRepeatingPingerNumbers(nameof(AddedByInspector))]
        public ValidState Number { get; set; }
        public SelectNomenclatureDto Status { get; set; }
        public ValidState Model { get; set; }
        public ValidState Brand { get; set; }
        public ValidStateBool AddedByInspector { get; set; }
        public bool IsSameAsInspected
        {
            get => _isSameAsInspected;
            set => SetProperty(ref _isSameAsInspected, value);
        }

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
                    List<PingerViewModel> pingers = FishingGearDialogViewModel.Instance.InspectedFishingGear.Pingers.ToList();
                    int count = pingers.Where(x => x.Number.Value == Number.Value).Count();
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
                if (!string.IsNullOrEmpty(Number.Value))
                {
                    return true;
                }
                return false;
            }
        }

        public static implicit operator FishingGearPingerDto(PingerViewModel viewModel)
        {
            if (viewModel.Status != null)
            {
                return new FishingGearPingerDto
                {
                    Id = viewModel.Id,
                    Number = viewModel.Number,
                    SelectedStatus = Enum.TryParse(viewModel.Status.Code, out FishingGearPingerStatusesEnum markStatus)
                                ? markStatus
                                : FishingGearPingerStatusesEnum.NEW,
                    StatusId = viewModel.Status.Id,
                    Model = viewModel.Model,
                    Brand = viewModel.Brand
                };
            }

            return null;
        }
    }
}
