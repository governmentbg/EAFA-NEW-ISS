using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Controls.ViewModels;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Insp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.FishingGearDialog
{
    public class FishingGearViewModel : ViewModel
    {
        private SelectNomenclatureDto _inspectedMarkStatus;
        private List<SelectNomenclatureDto> _allMarkStatuses;
        private List<SelectNomenclatureDto> _markStatuses;
        private List<SelectNomenclatureDto> _pingerStatuses;
        private bool _hasPingers;

        public FishingGearViewModel(InspectionPageViewModel inspection, ViewActivityType dialogType, bool isEditable)
        {
            Inspection = inspection;
            DialogType = dialogType;
            IsEditable = isEditable;
            FishingGearGeneralInfo = new FishingGearGeneralInfoViewModel(dialogType, isEditable);

            AddMark = CommandBuilder.CreateFrom(OnAddMark);
            GenerateMarks = CommandBuilder.CreateFrom(OnGenerateMarks);
            RemoveMark = CommandBuilder.CreateFrom<MarkViewModel>(OnRemoveMark);
            AddPinger = CommandBuilder.CreateFrom(OnAddPinger);
            RemovePinger = CommandBuilder.CreateFrom<PingerViewModel>(OnRemovePinger);

            this.AddValidation(others: new IValidatableViewModel[]
            {
                FishingGearGeneralInfo
            });
        }

        public InspectionPageViewModel Inspection { get; set; }
        public FishingGearGeneralInfoViewModel FishingGearGeneralInfo { get; set; }
        public ViewActivityType DialogType { get; set; }
        public bool IsEditable { get; set; }
        public bool IsInspectedGear { get; set; }

        public int? Id { get; set; }


        public ValidStateValidatableTable<MarkViewModel> Marks { get; set; }

        public ValidStateValidatableTable<PingerViewModel> Pingers { get; set; }


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
        public SelectNomenclatureDto DefaultPingerStatus { get; set; }
        public bool HasPingers
        {
            get { return _hasPingers; }
            set { SetProperty(ref _hasPingers, value); }
        }

        public ICommand AddMark { get; }
        public ICommand GenerateMarks { get; }
        public ICommand RemoveMark { get; }
        public ICommand AddPinger { get; }
        public ICommand RemovePinger { get; }
        public ICommand EditPinger { get; }
        public ICommand ViewPinger { get; }
        public ICommand MoveMark { get; set; }
        public ICommand MoveAllMarks { get; set; }
        public ICommand MovePinger { get; set; }
        public ICommand MoveAllPingers { get; set; }
        public ICommand MarkDeleted { get; set; }
        public ICommand PingerDeleted { get; set; }

        public void Init(List<FishingGearSelectNomenclatureDto> fishingGearTypes, List<SelectNomenclatureDto> markStatuses, List<SelectNomenclatureDto> pingerStatuses)
        {
            FishingGearGeneralInfo.Init(fishingGearTypes);

            _allMarkStatuses = markStatuses;
            PingerStatuses = pingerStatuses;

            MarkStatuses = markStatuses.FindAll(f => f.Code != "MARKED");
            _inspectedMarkStatus = markStatuses.Find(f => f.Code == "MARKED");

            DefaultPingerStatus = PingerStatuses.Where(x => x.Code == nameof(FishingGearPingerStatusesEnum.NEW)).First();
        }

        public void AssignEdit(FishingGearDto dto)
        {
            if (dto == null)
            {
                return;
            }

            Id = dto.Id;

            FishingGearGeneralInfo.AssignEdit(dto);

            if (dto.Marks?.Count > 0)
            {
                foreach (FishingGearMarkDto mark in dto.Marks)
                {
                    MarkViewModel markViewModel = new MarkViewModel(mark.SelectedStatus == FishingGearMarkStatus.MARKED)
                    {
                        Id = mark.Id,
                        CreatedOn = mark.CreatedOn,
                    };
                    if (mark.FullNumber != null)
                    {
                        markViewModel.Number.Value = mark.FullNumber.InputValue;
                        markViewModel.Prefix = mark.FullNumber.Prefix;
                    }
                    markViewModel.Status.Value = _allMarkStatuses.Find(f => f.Id == mark.StatusId);
                    Marks.Value.Add(markViewModel);
                }
            }

            if (dto.Pingers?.Count > 0 && HasPingers)
            {
                foreach (FishingGearPingerDto pinger in dto.Pingers)
                {
                    bool addedByInspector = false;
                    if (IsEditable)
                    {
                        addedByInspector = !FishingGearDialogViewModel.Instance.PermittedFishingGear.Pingers.Select(x => x.Number.Value).ToList().Contains(pinger.Number);
                    }
                    PingerViewModel pingerModel = new PingerViewModel(addedByInspector)
                    {
                        Id = pinger.Id,
                    };
                    pingerModel.Number.Value = pinger.Number;
                    pingerModel.Status = PingerStatuses.Find(f => f.Id == pinger.StatusId);
                    pingerModel.Model.Value = pinger.Model;
                    pingerModel.Brand.Value = pinger.Brand;
                    Pingers.Value.Add(pingerModel);
                }
            }
            else
            {
                HasPingers = false;
            }
        }

        public bool IsValid
        {
            get
            {
                Validation.Force();
                foreach (var mark in Marks.Value)
                {
                    if (!mark.IsValid)
                    {
                        return !IsEditable;
                    }
                }
                foreach (var pinger in Pingers.Value)
                {
                    if (!pinger.IsValid)
                    {
                        return !IsEditable;
                    }
                }
                var generalInfoValidator = Validation.OtherValidations.FirstOrDefault(f => f.Name == nameof(FishingGearGeneralInfoViewModel));
                if (generalInfoValidator != null)
                {
                    if (generalInfoValidator.IsValid)
                    {
                        return true;
                    }
                }
                return !IsEditable;
            }
        }

        private void OnAddMark()
        {
            MarkViewModel mark = new MarkViewModel(true)
            {
                CreatedOn = DateTime.Now,
            };
            mark.Status.Value = _inspectedMarkStatus;
            Marks.Value.Add(mark);
        }

        private async Task OnGenerateMarks()
        {
            GenerateMarksModel generateMarks = await TLDialogHelper.ShowDialog(new GenerateMarksDialog.GenerateMarksDialog());
            if (generateMarks != null)
            {
                List<MarkViewModel> marks = new List<MarkViewModel>();
                for (int i = generateMarks.From; i <= generateMarks.To; i++)
                {
                    MarkViewModel mark = new MarkViewModel(true)
                    {
                        CreatedOn = DateTime.Now,
                    };
                    mark.Number.AssignFrom(i);
                    mark.Status.Value = _inspectedMarkStatus;
                    marks.Add(mark);
                }
                Marks.Value.AddRange(marks);
            }
        }

        private void OnRemoveMark(MarkViewModel mark)
        {
            Marks.Value.Remove(mark);
            MarkDeleted?.Execute(mark);
        }

        private void OnAddPinger()
        {
            PingerViewModel pinger = new PingerViewModel(true)
            {
                Status = DefaultPingerStatus,
            };
            Pingers.Value.Add(pinger);
        }

        private void OnRemovePinger(PingerViewModel pinger)
        {
            Pingers.Value.Remove(pinger);
            PingerDeleted?.Execute(pinger);
        }

        public static implicit operator FishingGearDto(FishingGearViewModel viewModel)
        {
            if (viewModel.FishingGearGeneralInfo.FishingGearType.Value != null)
            {
                return new FishingGearDto
                {
                    Id = viewModel.Id,
                    Count = ParseHelper.ParseInteger(viewModel.FishingGearGeneralInfo.Count) ?? 0,
                    Description = viewModel.FishingGearGeneralInfo.Description,
                    Height = ParseHelper.ParseDecimal(viewModel.FishingGearGeneralInfo.Height),
                    HookCount = ParseHelper.ParseInteger(viewModel.FishingGearGeneralInfo.HookCount),
                    NetEyeSize = ParseHelper.ParseDecimal(viewModel.FishingGearGeneralInfo.NetEyeSize),
                    TypeId = viewModel.FishingGearGeneralInfo.FishingGearType.Value,
                    Length = ParseHelper.ParseDecimal(viewModel.FishingGearGeneralInfo.Length),
                    CordThickness = ParseHelper.ParseDecimal(viewModel.FishingGearGeneralInfo.CordThickness),
                    TowelLength = ParseHelper.ParseInteger(viewModel.FishingGearGeneralInfo.TowelLength),
                    HouseLength = ParseHelper.ParseInteger(viewModel.FishingGearGeneralInfo.HouseLength),
                    HouseWidth = ParseHelper.ParseInteger(viewModel.FishingGearGeneralInfo.HouseWidth),
                    LineCount = ParseHelper.ParseInteger(viewModel.FishingGearGeneralInfo.LineCount),
                    NetNominalLength = ParseHelper.ParseInteger(viewModel.FishingGearGeneralInfo.NetNominalLength),
                    NetsInFleetCount = ParseHelper.ParseInteger(viewModel.FishingGearGeneralInfo.NetsInFleetCount),
                    TrawlModel = viewModel.FishingGearGeneralInfo.TrawlModel,
                    HasPingers = viewModel.HasPingers,
                    Marks = viewModel.Marks
                        .Select(f => (FishingGearMarkDto)f)
                        .Where(f => f != null)
                        .ToList(),
                    Pingers = viewModel.HasPingers || viewModel.Pingers.Count() != 0 ? viewModel.Pingers
                        .Select(f => (FishingGearPingerDto)f)
                        .Where(f => f != null)
                        .ToList() : null
                };
            }

            return null;
        }
    }
}
