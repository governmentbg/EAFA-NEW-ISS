using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.WaterFishingGearDialog;
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
    public class WaterFishingGearsViewModel : ViewModel
    {
        public WaterFishingGearsViewModel(InspectionPageViewModel inspection)
        {
            Inspection = inspection;

            Review = CommandBuilder.CreateFrom<WaterFishingGearModel>(OnReview);
            Add = CommandBuilder.CreateFrom(OnAdd);
            Edit = CommandBuilder.CreateFrom<WaterFishingGearModel>(OnEdit);
            Remove = CommandBuilder.CreateFrom<WaterFishingGearModel>(OnRemove);

            this.AddValidation();
        }

        public InspectionPageViewModel Inspection { get; }

        public ValidStateValidatableTable<WaterFishingGearModel> FishingGears { get; set; }

        public ICommand Review { get; }
        public ICommand Add { get; }
        public ICommand Edit { get; }
        public ICommand Remove { get; }

        private Task OnReview(WaterFishingGearModel model)
        {
            return TLDialogHelper.ShowDialog(new WaterFishingGearDialog(this, Inspection, ViewActivityType.Review, model));
        }

        private async Task OnAdd()
        {
            WaterFishingGearModel result = await TLDialogHelper.ShowDialog(new WaterFishingGearDialog(this, Inspection, ViewActivityType.Add));

            if (result != null)
            {
                FishingGears.Value.Add(result);
            }
        }

        private async Task OnEdit(WaterFishingGearModel fishingGear)
        {
            WaterFishingGearModel result = await TLDialogHelper.ShowDialog(new WaterFishingGearDialog(this, Inspection, ViewActivityType.Edit, fishingGear));

            if (result != null)
            {
                fishingGear.Marks = result.Marks;
                fishingGear.Type = result.Type;
                fishingGear.Dto = result.Dto;
                fishingGear.AllChanged();
            }
        }

        private async Task OnRemove(WaterFishingGearModel fishingGear)
        {
            bool result = await App.Current.MainPage.DisplayAlert(null,
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/DeleteMessage"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Yes"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/No"]
            );

            if (result)
            {
                FishingGears.Value.Remove(fishingGear);
            }
        }

        public static implicit operator List<WaterInspectionFishingGearDto>(WaterFishingGearsViewModel viewModel)
        {
            return viewModel == null
                ? new List<WaterInspectionFishingGearDto>()
                : viewModel.FishingGears.Select(f => f.Dto).ToList();
        }
    }
}
