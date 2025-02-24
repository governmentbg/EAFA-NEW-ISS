using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Attributes;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.FishingGearDialog;
using IARA.Mobile.Insp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class FishingGearsViewModel : ViewModel
    {
        private PermitLicensesViewModel _permitLicenses;
        public FishingGearsViewModel(InspectionPageViewModel inspection, PermitLicensesViewModel permitLicenses, bool hasPingers = true, bool hasAttachmentForFishingGear = false, bool showXIconWhenUnregistered = true)
        {
            Inspection = inspection;
            HasPingers = hasPingers;
            HasAttachmentForFishingGear = hasAttachmentForFishingGear;
            ShowXIconWhenUnregistered = showXIconWhenUnregistered;
            _permitLicenses = permitLicenses;

            Review = CommandBuilder.CreateFrom<FishingGearModel>(OnReview);
            Add = CommandBuilder.CreateFrom(OnAdd);
            Edit = CommandBuilder.CreateFrom<FishingGearModel>(OnEdit);
            Remove = CommandBuilder.CreateFrom<FishingGearModel>(OnRemove);
            GenerateFromPermitLicense = CommandBuilder.CreateFrom(OnGenerateFromPermitLicense);
            ShowRequiredDialog = CommandBuilder.CreateFrom<FishingGearModel>(OnShowRequiredDialog);

            AllFishingGears = new List<FishingGearModel>();
            this.AddValidation();
        }

        public InspectionPageViewModel Inspection { get; }
        public bool HasPingers { get; }
        public bool ShowXIconWhenUnregistered { get; }
        public bool HasAttachmentForFishingGear { get; }

        [FishingGearValidation(ErrorMessageResourceName = "FishingGearValidation")]
        public ValidStateTable<FishingGearModel> FishingGears { get; set; }

        public List<FishingGearModel> AllFishingGears { get; set; }

        public ICommand Review { get; }
        public ICommand Add { get; }
        public ICommand Edit { get; }
        public ICommand Remove { get; }
        public ICommand ShowRequiredDialog { get; }
        public ICommand GenerateFromPermitLicense { get; }

        public void OnEdit(List<InspectedFishingGearDto> fishingGears, List<int> permitIds)
        {
            if (fishingGears == null || fishingGears.Count == 0)
            {
                return;
            }

            List<SelectNomenclatureDto> fishingGearTypes = NomenclaturesTransaction.GetFishingGears().Select(x => new SelectNomenclatureDto()
            {
                Code = x.Code,
                Id = x.Id,
                Name = x.Name
            }).ToList();

            if (fishingGears.Count > 0)
            {
                FishingGears.Value.Clear();

                foreach (InspectedFishingGearDto fishingGear in fishingGears)
                {
                    if (fishingGear.InspectedFishingGear == null && fishingGear.PermittedFishingGear == null)
                    {
                        continue;
                    }

                    FishingGearDto mainFishingGear = fishingGear.InspectedFishingGear ?? fishingGear.PermittedFishingGear;

                    FishingGearModel model = new FishingGearModel
                    {
                        CheckedValue = fishingGear.CheckInspectedMatchingRegisteredGear,
                        IsAddedByInspector = fishingGear.PermittedFishingGear == null,
                        Count = mainFishingGear.Count,
                        NetEyeSize = mainFishingGear.NetEyeSize,
                        Type = fishingGearTypes.Find(f => f.Id == mainFishingGear.TypeId) ?? fishingGearTypes[0],
                        Marks = mainFishingGear.Marks == null
                            ? string.Empty
                            : string.Join(", ", mainFishingGear.Marks.Select(f => f.FullNumber?.ToString()).Where(f => !string.IsNullOrEmpty(f))),
                        Dto = fishingGear,
                    };

                    AllFishingGears.Add(model);

                    if (fishingGear.PermittedFishingGear?.PermitId == null || permitIds.Contains(fishingGear.PermittedFishingGear.PermitId.Value))
                    {
                        FishingGears.Value.Add(model);
                    }
                }
            }
        }

        public void Reset()
        {
            FishingGears.Value.Clear();
            AllFishingGears.Clear();
        }

        private Task OnReview(FishingGearModel model)
        {
            return TLDialogHelper.ShowDialog(new FishingGearDialog(Inspection, ViewActivityType.Review, HasPingers, HasAttachmentForFishingGear, model));
        }

        private async Task OnAdd()
        {
            FishingGearModel result = await TLDialogHelper.ShowDialog(new FishingGearDialog(Inspection, ViewActivityType.Add, HasPingers, HasAttachmentForFishingGear));

            if (result != null)
            {
                AllFishingGears.Add(result);
                FishingGears.Value.Add(result);
            }
            this.Validation.Force();
        }

        private async Task OnEdit(FishingGearModel fishingGear)
        {
            FishingGearModel result = await TLDialogHelper.ShowDialog(new FishingGearDialog(Inspection, ViewActivityType.Edit, HasPingers, HasAttachmentForFishingGear, fishingGear));

            if (result != null)
            {
                result.Marks = string.Join(", ",
                    (result.Dto.InspectedFishingGear ?? result.Dto.PermittedFishingGear).Marks
                    .Select(f => f.FullNumber.ToString())
                    .Where(f => !string.IsNullOrWhiteSpace(f))
                    .ToArray());
                if (result.Dto.PermittedFishingGear?.Id != null && result.Dto.InspectedFishingGear != null)
                {
                    result.Dto.InspectedFishingGear.Id = null;
                }
                fishingGear.AssignFrom(result);
                FishingGears.Value.Replace(fishingGear, fishingGear);
            }
            this.Validation.Force();
        }

        private async Task OnRemove(FishingGearModel fishingGear)
        {
            bool result = await App.Current.MainPage.DisplayAlert(null,
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/DeleteMessage"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Yes"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/No"]
            );

            if (result)
            {
                AllFishingGears.Remove(fishingGear);
                FishingGears.Value.Remove(fishingGear);
            }
            this.Validation.Force();
        }
        private Task OnShowRequiredDialog(FishingGearModel fishingGear)
        {
            return App.Current.MainPage.DisplayAlert(
                TranslateExtension.Translator[nameof(GroupResourceEnum.FishingGear) + "/RequiredGearInspectionTitle"],
                string.Format(TranslateExtension.Translator[nameof(GroupResourceEnum.FishingGear) + "/RequiredGearInspection"], fishingGear.LogBookId),
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Ok"]
            );
        }

        private void OnGenerateFromPermitLicense()
        {
            if (_permitLicenses != null)
            {
                List<int> permitIds = _permitLicenses.PermitLicenses
                                        .Where(f => f.Dto?.PermitLicenseId != null)
                                        .Select(f => f.Dto.PermitLicenseId.Value)
                                        .ToList();

                FishingGears.Value.Clear();
                FishingGears.Value.AddRange(
                    AllFishingGears
                        .FindAll(f => f.Dto.PermittedFishingGear == null || permitIds.Contains(f.Dto.PermittedFishingGear.PermitId.Value))
                );
                this.Validation.Force();
            }
        }

        public static implicit operator List<InspectedFishingGearDto>(FishingGearsViewModel viewModel)
        {
            return viewModel == null
                ? new List<InspectedFishingGearDto>()
                : viewModel.FishingGears.Select(f => f.Dto).ToList();
        }
    }
}
