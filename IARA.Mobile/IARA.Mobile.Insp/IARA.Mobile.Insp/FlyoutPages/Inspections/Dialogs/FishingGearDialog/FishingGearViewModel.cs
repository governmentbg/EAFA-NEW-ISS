using IARA.Mobile.Application.Attributes;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Helpers;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.FishingGearDialog
{
    public class FishingGearViewModel : ViewModel
    {
        private SelectNomenclatureDto _inspectedMarkStatus;
        private List<SelectNomenclatureDto> _allMarkStatuses;
        private List<SelectNomenclatureDto> _markStatuses;
        private List<SelectNomenclatureDto> _pingerStatuses;
        private List<SelectNomenclatureDto> _fishingGearTypes;

        public FishingGearViewModel(InspectionPageViewModel inspection, ViewActivityType dialogType)
        {
            Inspection = inspection;
            DialogType = dialogType;

            AddMark = CommandBuilder.CreateFrom(OnAddMark);
            RemoveMark = CommandBuilder.CreateFrom<MarkViewModel>(OnRemoveMark);
            AddPinger = CommandBuilder.CreateFrom(OnAddPinger);
            RemovePinger = CommandBuilder.CreateFrom<PingerViewModel>(OnRemovePinger);

            this.AddValidation();
        }

        public InspectionPageViewModel Inspection { get; set; }
        public ViewActivityType DialogType { get; set; }
        public bool IsEditable { get; set; }
        public bool IsInspectedGear { get; set; }
        public bool HasPingers { get; set; }

        public int? Id { get; set; }

        [Required]
        public ValidStateSelect<SelectNomenclatureDto> FishingGearType { get; set; }

        [Required]
        [TLRange(1, 10000)]
        public ValidState Count { get; set; }

        [TLRange(1, 10000, true)]
        public ValidState NetEyeSize { get; set; }

        [TLRange(0, 10000)]
        public ValidState HookCount { get; set; }

        [TLRange(1, 10000, true)]
        public ValidState Length { get; set; }

        [TLRange(1, 10000, true)]
        public ValidState Height { get; set; }

        [TLRange(1, 10000, true)]
        public ValidState CordThickness { get; set; }

        public ValidStateValidatableTable<MarkViewModel> Marks { get; set; }

        public ValidStateValidatableTable<PingerViewModel> Pingers { get; set; }

        [MaxLength(4000)]
        public ValidState Description { get; set; }

        public List<SelectNomenclatureDto> FishingGearTypes
        {
            get => _fishingGearTypes;
            private set => SetProperty(ref _fishingGearTypes, value);
        }
        public List<SelectNomenclatureDto> MarkStatuses
        {
            get => _markStatuses;
            private set => SetProperty(ref _markStatuses, value);
        }
        public List<SelectNomenclatureDto> PingerStatuses
        {
            get => _pingerStatuses;
            private set => SetProperty(ref _pingerStatuses, value);
        }

        public ICommand AddMark { get; }
        public ICommand RemoveMark { get; }
        public ICommand AddPinger { get; }
        public ICommand RemovePinger { get; }

        public void Init(List<SelectNomenclatureDto> fishingGearTypes, List<SelectNomenclatureDto> markStatuses, List<SelectNomenclatureDto> pingerStatuses)
        {
            FishingGearTypes = fishingGearTypes;
            _allMarkStatuses = markStatuses;
            PingerStatuses = pingerStatuses;

            MarkStatuses = markStatuses.FindAll(f => f.Code != "MARKED");
            _inspectedMarkStatus = markStatuses.Find(f => f.Code == "MARKED");

            FishingGearType.Value = fishingGearTypes[0];
        }

        public void AssignEdit(FishingGearDto dto)
        {
            if (dto == null)
            {
                return;
            }

            Id = dto.Id;
            FishingGearType.AssignFrom(dto.TypeId, FishingGearTypes);
            Count.AssignFrom(dto.Count);
            NetEyeSize.AssignFrom(dto.NetEyeSize);
            HookCount.AssignFrom(dto.HookCount);
            Length.AssignFrom(dto.Length);
            Height.AssignFrom(dto.Height);
            CordThickness.AssignFrom(dto.CordThickness);
            Description.AssignFrom(dto.Description);

            if (FishingGearType.Value == null)
            {
                FishingGearType.Value = FishingGearTypes[0];
            }

            if (dto.Marks?.Count > 0)
            {
                foreach (FishingGearMarkDto mark in dto.Marks)
                {
                    MarkViewModel markViewModel = new MarkViewModel
                    {
                        Id = mark.Id,
                        AddedByInspector = mark.SelectedStatus == FishingGearMarkStatus.MARKED,
                    };
                    markViewModel.Number.Value = mark.Number;
                    markViewModel.Status.Value = _allMarkStatuses.Find(f => f.Id == mark.StatusId);
                    Marks.Value.Add(markViewModel);
                }
            }

            if (dto.Pingers?.Count > 0 && HasPingers)
            {
                foreach (FishingGearPingerDto pinger in dto.Pingers)
                {
                    PingerViewModel pingerViewModel = new PingerViewModel
                    {
                        Id = pinger.Id
                    };
                    pingerViewModel.Number.Value = pinger.Number;
                    pingerViewModel.Status.Value = PingerStatuses.Find(f => f.Id == pinger.StatusId);
                    Pingers.Value.Add(pingerViewModel);
                }
            }
        }

        private void OnAddMark()
        {
            MarkViewModel mark = new MarkViewModel
            {
                AddedByInspector = true
            };
            mark.Status.Value = _inspectedMarkStatus;
            Marks.Value.Add(mark);
        }

        private void OnRemoveMark(MarkViewModel mark)
        {
            Marks.Value.Remove(mark);
        }

        private void OnAddPinger()
        {
            PingerViewModel pinger = new PingerViewModel();
            pinger.Status.Value = PingerStatuses[0];
            Pingers.Value.Add(pinger);
        }

        private void OnRemovePinger(PingerViewModel pinger)
        {
            Pingers.Value.Remove(pinger);
        }

        public static implicit operator FishingGearDto(FishingGearViewModel viewModel)
        {
            if (viewModel.FishingGearType.Value != null)
            {
                return new FishingGearDto
                {
                    Id = viewModel.Id,
                    Count = ParseHelper.ParseInteger(viewModel.Count) ?? 0,
                    Description = viewModel.Description,
                    Height = ParseHelper.ParseDecimal(viewModel.Height),
                    HookCount = ParseHelper.ParseInteger(viewModel.HookCount),
                    NetEyeSize = ParseHelper.ParseDecimal(viewModel.NetEyeSize),
                    TypeId = viewModel.FishingGearType.Value,
                    Length = ParseHelper.ParseDecimal(viewModel.Length),
                    CordThickness = ParseHelper.ParseDecimal(viewModel.CordThickness),
                    Marks = viewModel.Marks
                        .Select(f => (FishingGearMarkDto)f)
                        .Where(f => f != null)
                        .ToList(),
                    Pingers = viewModel.Pingers
                        .Select(f => (FishingGearPingerDto)f)
                        .Where(f => f != null)
                        .ToList(),
                };
            }

            return null;
        }
    }
}
