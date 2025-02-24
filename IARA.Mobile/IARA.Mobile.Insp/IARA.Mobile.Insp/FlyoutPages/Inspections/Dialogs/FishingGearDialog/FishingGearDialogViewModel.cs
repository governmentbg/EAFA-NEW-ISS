using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.Interfaces.Transactions;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Insp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Base;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using static System.Net.Mime.MediaTypeNames;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.FishingGearDialog
{
    public class FishingGearDialogViewModel : TLBaseDialogViewModel<FishingGearModel>
    {
        public static FishingGearDialogViewModel Instance { get; set; }
        public FishingGearDialogViewModel()
        {
            Save = CommandBuilder.CreateFrom(OnSave);
            CorrespondsChanged = CommandBuilder.CreateFrom<string>(OnCorrespondsChanged);
            Instance = this;
        }

        public InspectionPageViewModel Inspection { get; set; }
        public ViewActivityType DialogType { get; set; }
        public FishingGearModel Edit { get; set; }
        public bool HasPingers { get; set; }

        public FishingGearViewModel PermittedFishingGear { get; private set; }
        public FishingGearViewModel InspectedFishingGear { get; private set; }

        public ValidStateMultiToggle Corresponds { get; set; }
        public ValidStateMultiToggle HasAttachedAppliances { get; set; }

        public ICommand Save { get; }
        public ICommand CorrespondsChanged { get; }

        public void BeforeInit()
        {
            PermittedFishingGear = new FishingGearViewModel(Inspection, DialogType, false)
            {
                HasPingers = HasPingers,
                MoveMark = DialogType != ViewActivityType.Review
                    ? CommandBuilder.CreateFrom<MarkViewModel>(OnMoveMark)
                    : null,
                MoveAllMarks = DialogType != ViewActivityType.Review
                    ? CommandBuilder.CreateFrom(OnMoveAllMark)
                    : null,
                MovePinger = DialogType != ViewActivityType.Review
                    ? CommandBuilder.CreateFrom<PingerViewModel>(OnMovePinger)
                    : null,
                MoveAllPingers = DialogType != ViewActivityType.Review
                    ? CommandBuilder.CreateFrom(OnMoveAllPinger)
                    : null,
            };
            InspectedFishingGear = new FishingGearViewModel(Inspection, DialogType, true)
            {
                HasPingers = HasPingers,
                IsInspectedGear = true,
                MarkDeleted = CommandBuilder.CreateFrom<MarkViewModel>(OnInspectedMarkDeleted),
                PingerDeleted = CommandBuilder.CreateFrom<PingerViewModel>(OnInspectedPingerDeleted),
            };

            this.AddValidation(others: new IValidatableViewModel[] { PermittedFishingGear, InspectedFishingGear });

            if (Edit?.IsAddedByInspector != false)
            {
                Corresponds.Value = nameof(InspectedFishingGearEnum.I);
            }
        }

        public override Task Initialize(object sender)
        {
            INomenclatureTransaction nomTransaction = DependencyService.Resolve<INomenclatureTransaction>();

            List<FishingGearSelectNomenclatureDto> fishingGearTypes = nomTransaction.GetFishingGears().Select(x => new FishingGearSelectNomenclatureDto()
            {
                Name = x.DisplayValue,
                Code = x.Code,
                Id = x.Id,
                HasHooks = x.HasHooks,
            }).OrderBy(x => x.Code).ToList();
            FishingGearSelectNomenclatureDto Poundnet = fishingGearTypes.FirstOrDefault(x => x.Code == "DLN");
            int index = fishingGearTypes.IndexOf(Poundnet);
            fishingGearTypes.RemoveAt(index);
            fishingGearTypes.Insert(0, Poundnet);

            List<SelectNomenclatureDto> markStatuses = nomTransaction.GetFishingGearMarkStatuses();

            List<SelectNomenclatureDto> pingerStatuses = null;

            if (HasPingers)
            {
                pingerStatuses = nomTransaction.GetFishingGearPingerStatuses();
            }

            List<InspectionCheckTypeDto> checkTypes = nomTransaction.GetInspectionCheckTypes(InspectionType.IBP);

            InspectedFishingGear.Init(fishingGearTypes, markStatuses, pingerStatuses);
            PermittedFishingGear.Init(fishingGearTypes, markStatuses, pingerStatuses);

            if (Edit != null)
            {
                PermittedFishingGear.AssignEdit(Edit.Dto.PermittedFishingGear);
                InspectedFishingGear.AssignEdit(Edit.Dto.InspectedFishingGear ?? RemapFishingGear(Edit.Dto.PermittedFishingGear));
                Corresponds.Value = Edit.CheckedValue?.ToString();

                HasAttachedAppliances.Value = Edit.Dto.HasAttachedAppliances == null
                    ? null
                    : Edit.Dto.HasAttachedAppliances.Value
                    ? nameof(CheckTypeEnum.Y)
                    : nameof(CheckTypeEnum.N);

                Validation.Force();

                if (Edit.Dto.InspectedFishingGear != null)
                {
                    foreach (var permittedMark in PermittedFishingGear.Marks)
                    {
                        if (Edit.Dto.InspectedFishingGear.Marks.Any(x => x.FullNumber.ToString() == permittedMark.Number))
                        {
                            permittedMark.IsSameAsInspected = true;
                        }
                    }

                    foreach (var permittedPinger in PermittedFishingGear.Pingers)
                    {
                        if (Edit.Dto.InspectedFishingGear.Pingers.Any(x => x.Number == permittedPinger.Number))
                        {
                            permittedPinger.IsSameAsInspected = true;
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }

        private void OnCorrespondsChanged(string corresponds)
        {
            if (string.IsNullOrEmpty(corresponds))
            {
                return;
            }

            if (corresponds != nameof(CheckTypeEnum.N))
            {
                PermittedFishingGear.Marks.ForEach(x => x.IsSameAsInspected = false);
                InspectedFishingGear.Marks.Value.Clear();
            }

        }

        private void OnMoveAllPinger()
        {
            if (Corresponds.Value == nameof(CheckTypeEnum.N))
            {
                foreach (var pinger in PermittedFishingGear.Pingers)
                {
                    if (!InspectedFishingGear.Pingers.Any(f => f.Id == pinger.Id))
                    {
                        PingerViewModel newPinger = new PingerViewModel
                        {
                            Id = pinger.Id,
                            Number = pinger.Number,
                            Status = pinger.Status,
                            Model = pinger.Model,
                            Brand = pinger.Brand
                        };
                        InspectedFishingGear.Pingers.Value.Add(newPinger);
                        pinger.IsSameAsInspected = true;
                    }
                }
            }
        }

        private void OnMovePinger(PingerViewModel pinger)
        {
            if (Corresponds.Value == nameof(CheckTypeEnum.N))
            {
                if (!InspectedFishingGear.Pingers.Any(f => f.Id == pinger.Id))
                {
                    PingerViewModel newPinger = new PingerViewModel
                    {
                        Id = pinger.Id,
                        Number = pinger.Number,
                        Status = pinger.Status,
                        Model = pinger.Model,
                        Brand = pinger.Brand
                    };
                    InspectedFishingGear.Pingers.Value.Add(newPinger);
                    pinger.IsSameAsInspected = true;
                    return;
                }
            }
        }

        private void OnInspectedPingerDeleted(PingerViewModel model)
        {
            var pingerViewModels = PermittedFishingGear.Pingers.Where(f => f.Number.Value == model.Number.Value);

            if (!InspectedFishingGear.Pingers.Any(x => x.Number.Value == model.Number.Value))
            {
                foreach (var pinger in pingerViewModels)
                {
                    pinger.IsSameAsInspected = false;
                }
            }
        }

        private void OnMoveAllMark()
        {
            if (Corresponds.Value == nameof(CheckTypeEnum.N))
            {
                foreach (var mark in PermittedFishingGear.Marks)
                {
                    if (!InspectedFishingGear.Marks.Any(f => f.Id == mark.Id))
                    {
                        MarkViewModel newMark = new MarkViewModel(mark.Status.Value.Code == nameof(FishingGearMarkStatus.MARKED))
                        {
                            Id = mark.Id,
                        };

                        newMark.Status.Value = mark.Status.Value;
                        newMark.Number.Value = mark.Number.Value;
                        newMark.CreatedOn = mark.CreatedOn;

                        InspectedFishingGear.Marks.Value.Add(newMark);
                        mark.IsSameAsInspected = true;
                    }
                }
            }
        }

        private void OnMoveMark(MarkViewModel mark)
        {
            if (!InspectedFishingGear.Marks.Any(f => f.Id == mark.Id) && Corresponds.Value == nameof(CheckTypeEnum.N))
            {
                MarkViewModel newMark = new MarkViewModel(mark.Status.Value.Code == nameof(FishingGearMarkStatus.MARKED))
                {
                    Id = mark.Id,
                };

                newMark.Status.Value = mark.Status.Value;
                newMark.Number.Value = mark.Number.Value;
                newMark.CreatedOn = mark.CreatedOn;

                InspectedFishingGear.Marks.Value.Add(newMark);

                mark.IsSameAsInspected = true;

                return;
            }
        }
        private void OnInspectedMarkDeleted(MarkViewModel model)
        {
            var markViewModels = PermittedFishingGear.Marks.Where(f => f.Number.Value == model.Number.Value);

            if (!InspectedFishingGear.Marks.Any(x => x.Number.Value == model.Number.Value))
            {
                foreach (var mark in markViewModels)
                {
                    mark.IsSameAsInspected = false;
                }
            }
        }

        private Task OnSave()
        {
            bool isDialogValid;
            if (Edit?.IsAddedByInspector != false)
            {
                isDialogValid = InspectedFishingGear.IsValid;
            }
            else
            {
                isDialogValid = PermittedFishingGear.IsValid && (
                    InspectedFishingGear.IsValid
                    || Corresponds.Value == nameof(InspectedFishingGearEnum.Y)
                );
            }

            if (!isDialogValid)
            {
                return Task.CompletedTask;
            }

            InspectedFishingGearEnum? checkValue = Enum.TryParse(Corresponds.Value, out InspectedFishingGearEnum checkType)
                ? new InspectedFishingGearEnum?(checkType)
                : null;

            FishingGearViewModel fishingGear = checkValue == InspectedFishingGearEnum.N || checkValue == InspectedFishingGearEnum.I
                ? InspectedFishingGear
                : PermittedFishingGear;

            IReadOnlyList<string> result = (fishingGear.FishingGearGeneralInfo.FishingGearType as IValidState).ForceValidation();
            if (result?.Count > 0)
            {
                return Task.CompletedTask;
            }

            return HideDialog(new FishingGearModel
            {
                IsAddedByInspector = Edit == null ? true : Edit.IsAddedByInspector,
                Marks = string.Join(", ", fishingGear.Marks
                    .Select(f => f.Number.Value)
                    .Where(f => !string.IsNullOrWhiteSpace(f))
                    .ToArray()),
                Type = fishingGear.FishingGearGeneralInfo.FishingGearType.Value,
                Count = ParseHelper.ParseInteger(fishingGear.FishingGearGeneralInfo.Count) ?? 0,
                NetEyeSize = ParseHelper.ParseInteger(fishingGear.FishingGearGeneralInfo.NetEyeSize),
                CheckedValue = checkValue,
                Dto = new InspectedFishingGearDto
                {
                    CheckInspectedMatchingRegisteredGear = checkValue,
                    HasAttachedAppliances = HasAttachedAppliances.Value == null ? null : new bool?(HasAttachedAppliances.Value == nameof(CheckTypeEnum.Y)),
                    InspectedFishingGear = checkValue == InspectedFishingGearEnum.N || checkValue == InspectedFishingGearEnum.I
                        ? (FishingGearDto)InspectedFishingGear
                        : null,
                    PermittedFishingGear = Edit?.Dto.PermittedFishingGear == null ? null : (FishingGearDto)PermittedFishingGear,
                }
            });
        }

        private FishingGearDto RemapFishingGear(FishingGearDto fishingGear)
        {
            if (fishingGear == null)
            {
                return null;
            }

            return new FishingGearDto
            {
                Id = fishingGear.Id,
                CordThickness = fishingGear.CordThickness,
                Count = fishingGear.Count,
                Description = fishingGear.Description,
                Height = fishingGear.Height,
                HookCount = fishingGear.HookCount,
                HouseLength = fishingGear.HouseLength,
                HouseWidth = fishingGear.HouseWidth,
                IsActive = fishingGear.IsActive,
                Length = fishingGear.Length,
                NetEyeSize = fishingGear.NetEyeSize,
                PermitId = fishingGear.PermitId,
                TowelLength = fishingGear.TowelLength,
                LineCount = fishingGear.LineCount,
                NetNominalLength = fishingGear.NetNominalLength,
                NetsInFleetCount = fishingGear.NetsInFleetCount,
                TrawlModel = fishingGear.TrawlModel,
                TypeId = fishingGear.TypeId,
                Pingers = new List<FishingGearPingerDto>(),
                Marks = new List<FishingGearMarkDto>(),
            };
        }
    }
}
