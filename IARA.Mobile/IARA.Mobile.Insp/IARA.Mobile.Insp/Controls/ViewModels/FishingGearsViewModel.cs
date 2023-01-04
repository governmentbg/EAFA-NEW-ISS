using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.FishingGearDialog;
using IARA.Mobile.Insp.Models;
using System.Collections.Generic;
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
        public FishingGearsViewModel(InspectionPageViewModel inspection, bool hasPingers = false)
        {
            Inspection = inspection;
            HasPingers = hasPingers;

            Review = CommandBuilder.CreateFrom<FishingGearModel>(OnReview);
            Add = CommandBuilder.CreateFrom(OnAdd);
            Edit = CommandBuilder.CreateFrom<FishingGearModel>(OnEdit);
            Remove = CommandBuilder.CreateFrom<FishingGearModel>(OnRemove);

            AllFishingGears = new List<FishingGearModel>();

            this.AddValidation();
        }

        public InspectionPageViewModel Inspection { get; }
        public bool HasPingers { get; }

        public ValidStateValidatableTable<FishingGearModel> FishingGears { get; set; }

        public List<FishingGearModel> AllFishingGears { get; set; }

        public ICommand Review { get; }
        public ICommand Add { get; }
        public ICommand Edit { get; }
        public ICommand Remove { get; }

        public void OnEdit(List<InspectedFishingGearDto> fishingGears, List<int> permitIds)
        {
            if (fishingGears == null || fishingGears.Count == 0)
            {
                return;
            }

            List<SelectNomenclatureDto> fishingGearTypes = NomenclaturesTransaction.GetFishingGears();

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
                            : string.Join(", ", mainFishingGear.Marks.Select(f => f.Number).Where(f => !string.IsNullOrEmpty(f))),
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

        private Task OnReview(FishingGearModel model)
        {
            return TLDialogHelper.ShowDialog(new FishingGearDialog(Inspection, ViewActivityType.Review, HasPingers, model));
        }

        private async Task OnAdd()
        {
            FishingGearModel result = await TLDialogHelper.ShowDialog(new FishingGearDialog(Inspection, ViewActivityType.Add, HasPingers));

            if (result != null)
            {
                AllFishingGears.Add(result);
                FishingGears.Value.Add(result);
            }
        }

        private async Task OnEdit(FishingGearModel fishingGear)
        {
            FishingGearModel result = await TLDialogHelper.ShowDialog(new FishingGearDialog(Inspection, ViewActivityType.Edit, HasPingers, fishingGear));

            if (result != null)
            {
                fishingGear.Marks = result.Marks;
                fishingGear.CheckedValue = result.CheckedValue;
                fishingGear.Type = result.Type;
                fishingGear.Count = result.Count;
                fishingGear.NetEyeSize = result.NetEyeSize;
                fishingGear.Dto = result.Dto;
                fishingGear.AllChanged();
            }
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
        }

        public static implicit operator List<InspectedFishingGearDto>(FishingGearsViewModel viewModel)
        {
            return viewModel == null
                ? new List<InspectedFishingGearDto>()
                : viewModel.AllFishingGears.ConvertAll(f => f.Dto);
        }
    }
}
