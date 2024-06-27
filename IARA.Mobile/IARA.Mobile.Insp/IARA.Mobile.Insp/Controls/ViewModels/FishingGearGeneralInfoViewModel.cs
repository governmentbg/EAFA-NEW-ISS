using IARA.Mobile.Application.Attributes;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Attributes;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.FishingGearDialog;
using IARA.Mobile.Insp.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class FishingGearGeneralInfoViewModel : ViewModel
    {
        private List<FishingGearSelectNomenclatureDto> _fishingGearTypes;
        private bool _isPoundnetSelected = false;
        private bool _isFishingGearSelected = false;
        private ViewActivityType _dialogType;

        public FishingGearGeneralInfoViewModel(ViewActivityType dialogType, bool isEditable, bool isPoundnetFormVisible = false, bool isFishingGearFormVisible = false)
        {
            DialogType = dialogType;
            IsEditable = isEditable;

            IsPoundnetSelected = isPoundnetFormVisible;
            IsFishingGearSelected = isFishingGearFormVisible;

            SelectFishingGearType = CommandBuilder.CreateFrom<FishingGearSelectNomenclatureDto>(OnSelectFishingGearType);

            this.AddValidation();
        }

        public bool IsEditable { get; set; }
        public FishingGearViewModel FishingGear { get; }

        public ViewActivityType DialogType
        {
            get { return _dialogType; }
            set { _dialogType = value; }
        }


        public List<FishingGearSelectNomenclatureDto> FishingGearTypes
        {
            get => _fishingGearTypes;
            private set => SetProperty(ref _fishingGearTypes, value);
        }
        public bool IsPoundnetSelected
        {
            get { return _isPoundnetSelected; }
            set { SetProperty(ref _isPoundnetSelected, value); }
        }

        public bool IsFishingGearSelected
        {
            get { return _isFishingGearSelected; }
            set { SetProperty(ref _isFishingGearSelected, value); }
        }

        [Required]
        public ValidStateSelect<FishingGearSelectNomenclatureDto> FishingGearType { get; set; }

        [TLRange(0, 10000)]
        public ValidState Count { get; set; }

        [TLRange(0, 10000, true)]
        [RequiredIfFishingGearEquals(nameof(FishingGearType), FishGearInputs.NetEyeSize, ErrorMessageResourceName = "Required")]
        public ValidState NetEyeSize { get; set; }

        [TLRange(0, 10000)]
        [RequiredIfFishingGearHasHooks(nameof(FishingGearType), ErrorMessageResourceName = "Required")]
        public ValidState HookCount { get; set; }

        [TLRange(0, 10000, true)]
        [RequiredIfOtherPropertyHasValue(nameof(FishingGearType), nameof(FishingGearType), ErrorMessageResourceName = "GearDimensionRequired")]
        [RequiredIfFishingGearEquals(nameof(FishingGearType), FishGearInputs.Length, ErrorMessageResourceName = "Required")]
        public ValidState Length { get; set; }

        [TLRange(0, 10000, true)]
        [RequiredIfFishingGearEquals(nameof(FishingGearType), FishGearInputs.Height, ErrorMessageResourceName = "Required")]
        public ValidState Height { get; set; }

        [TLRange(0, 10000, true)]
        public ValidState CordThickness { get; set; }

        [TLRange(0, 10000)]
        [RequiredIfFishingGearEquals(nameof(FishingGearType), FishGearInputs.LineCount, ErrorMessageResourceName = "Required")]
        public ValidState LineCount { get; set; }

        [TLRange(0, 10000, true)]
        [RequiredIfFishingGearEquals(nameof(FishingGearType), FishGearInputs.NetNominalLength, ErrorMessageResourceName = "Required")]
        public ValidState NetNominalLength { get; set; }

        [TLRange(0, 10000)]
        [RequiredIfFishingGearEquals(nameof(FishingGearType), FishGearInputs.NetsInFleetCount, ErrorMessageResourceName = "Required")]
        public ValidState NetsInFleetCount { get; set; }

        [RequiredIfOtherPropertyHasValue(nameof(Length), nameof(FishingGearType), ErrorMessageResourceName = "GearDimensionRequired")]
        [RequiredIfFishingGearEquals(nameof(FishingGearType), FishGearInputs.TrawlModel, ErrorMessageResourceName = "Required")]
        public ValidState TrawlModel { get; set; }

        [MaxLength(4000)]
        public ValidState Description { get; set; }


        [TLRange(0, 10000)]
        public ValidState TowelLength { get; set; }
        [TLRange(0, 10000)]
        public ValidState HouseLength { get; set; }
        [TLRange(0, 10000)]
        public ValidState HouseWidth { get; set; }


        public ICommand SelectFishingGearType { get; set; }

        public void OnSelectFishingGearType(FishingGearSelectNomenclatureDto dto)
        {
            if (dto.Code == "DLN")
            {
                IsPoundnetSelected = true;
                IsFishingGearSelected = false;

                int index = Count.Validations.FindIndex(f => f.Name == nameof(RequiredAttribute));
                if (index != -1)
                {
                    Count.Validations.RemoveAt(index);
                    Count.HasAsterisk = false;
                    OnPropertyChanged(nameof(Count));
                }
            }
            else
            {
                IsPoundnetSelected = false;
                IsFishingGearSelected = true;

                Count.Validations.Add(new TLValidator(new RequiredAttribute(), nameof(RequiredAttribute)));
                Count.HasAsterisk = true;
                OnPropertyChanged(nameof(Count));
            }
        }

        public void Init(List<FishingGearSelectNomenclatureDto> fishingGearTypes)
        {
            FishingGearTypes = fishingGearTypes;
        }

        public void AssignEdit(FishingGearDto dto)
        {
            FishingGearType.AssignFrom(dto.TypeId, FishingGearTypes);
            OnSelectFishingGearType(FishingGearType.Value);

            Count.AssignFrom(dto.Count);
            NetEyeSize.AssignFrom(dto.NetEyeSize);
            HookCount.AssignFrom(dto.HookCount);
            Length.AssignFrom(dto.Length);
            Height.AssignFrom(dto.Height);
            CordThickness.AssignFrom(dto.CordThickness);
            LineCount.AssignFrom(dto.LineCount);
            NetNominalLength.AssignFrom(dto.NetNominalLength);
            NetsInFleetCount.AssignFrom(dto.NetsInFleetCount);
            TrawlModel.AssignFrom(dto.TrawlModel);

            TowelLength.AssignFrom(dto.TowelLength);
            HouseLength.AssignFrom(dto.HouseLength);
            HouseWidth.AssignFrom(dto.HouseWidth);

            Description.AssignFrom(dto.Description);

            if (FishingGearType.Value == null)
            {
                FishingGearType.Value = FishingGearTypes[0];
                OnSelectFishingGearType(FishingGearType.Value);
            }
        }
    }
}
