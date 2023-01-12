using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.Interfaces.Transactions;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Insp.Models;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Base;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.FishingGearDialog
{
    public class FishingGearDialogViewModel : TLBaseDialogViewModel<FishingGearModel>
    {
        public FishingGearDialogViewModel()
        {
            Save = CommandBuilder.CreateFrom(OnSave);
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

        public void BeforeInit()
        {
            PermittedFishingGear = new FishingGearViewModel(Inspection, DialogType)
            {
                IsEditable = false,
                HasPingers = HasPingers,
                MoveMark = DialogType != ViewActivityType.Review
                    ? CommandBuilder.CreateFrom<MarkViewModel>(OnMoveMark)
                    : null,
            };
            InspectedFishingGear = new FishingGearViewModel(Inspection, DialogType)
            {
                IsEditable = true,
                HasPingers = HasPingers,
                IsInspectedGear = true,
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

            List<SelectNomenclatureDto> fishingGearTypes = nomTransaction.GetFishingGears();
            List<SelectNomenclatureDto> markStatuses = nomTransaction.GetFishingGearMarkStatuses();

            List<SelectNomenclatureDto> pingerStatuses = null;

            if (HasPingers)
            {
                pingerStatuses = nomTransaction.GetFishingGearPingerStatuses();
            }

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
            }

            return Task.CompletedTask;
        }

        private void OnMoveMark(MarkViewModel mark)
        {
            if (!InspectedFishingGear.Marks.Any(f => f.Id == mark.Id))
            {
                MarkViewModel newMark = new MarkViewModel
                {
                    Id = mark.Id,
                    AddedByInspector = mark.Status.Value.Code == nameof(FishingGearMarkStatus.MARKED),
                };

                newMark.Status.Value = mark.Status.Value;
                newMark.Number.Value = mark.Number.Value;

                InspectedFishingGear.Marks.Value.Add(newMark);
            }
        }

        private Task OnSave()
        {
            InspectedFishingGearEnum? checkValue = Enum.TryParse(Corresponds.Value, out InspectedFishingGearEnum checkType)
                ? new InspectedFishingGearEnum?(checkType)
                : null;

            FishingGearViewModel fishingGear = checkValue == InspectedFishingGearEnum.N || checkValue == InspectedFishingGearEnum.I
                ? InspectedFishingGear
                : PermittedFishingGear;

            IReadOnlyList<string> result = (fishingGear.FishingGearType as IValidState).ForceValidation();
            if (result?.Count > 0)
            {
                return Task.CompletedTask;
            }

            return HideDialog(new FishingGearModel
            {
                IsAddedByInspector = Edit == null,
                Marks = string.Join(", ", fishingGear.Marks
                    .Select(f => f.Number.Value)
                    .Where(f => !string.IsNullOrWhiteSpace(f))
                    .ToArray()),
                Type = fishingGear.FishingGearType.Value,
                Count = ParseHelper.ParseInteger(fishingGear.Count) ?? 0,
                NetEyeSize = ParseHelper.ParseInteger(fishingGear.NetEyeSize),
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
                Pingers = fishingGear.Pingers,
                TowelLength = fishingGear.TowelLength,
                TypeId = fishingGear.TypeId,
                Marks = new List<FishingGearMarkDto>(),
            };
        }
    }
}
