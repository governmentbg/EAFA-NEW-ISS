using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.WaterCatchDialog;
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
    public class WaterCatchesViewModel : ViewModel
    {
        public WaterCatchesViewModel(InspectionPageViewModel inspection)
        {
            Inspection = inspection;

            Review = CommandBuilder.CreateFrom<WaterCatchModel>(OnReview);
            Add = CommandBuilder.CreateFrom(OnAdd);
            Edit = CommandBuilder.CreateFrom<WaterCatchModel>(OnEdit);
            Remove = CommandBuilder.CreateFrom<WaterCatchModel>(OnRemove);

            this.AddValidation();
        }

        public InspectionPageViewModel Inspection { get; }

        public ValidStateValidatableTable<WaterCatchModel> CatchMeasures { get; set; }

        public ICommand Review { get; }
        public ICommand Add { get; }
        public ICommand Edit { get; }
        public ICommand Remove { get; }

        private Task OnReview(WaterCatchModel model)
        {
            return TLDialogHelper.ShowDialog(new WaterCatchDialog(this, Inspection, ViewActivityType.Review, model));
        }

        private async Task OnAdd()
        {
            WaterCatchModel result = await TLDialogHelper.ShowDialog(new WaterCatchDialog(this, Inspection, ViewActivityType.Add));

            if (result != null)
            {
                CatchMeasures.Value.Add(result);
            }
        }

        private async Task OnEdit(WaterCatchModel catchMeasure)
        {
            WaterCatchModel result = await TLDialogHelper.ShowDialog(new WaterCatchDialog(this, Inspection, ViewActivityType.Edit, catchMeasure));

            if (result != null)
            {
                catchMeasure.FishName = result.FishName;
                catchMeasure.Dto = result.Dto;
                catchMeasure.AllChanged();
            }
        }

        private async Task OnRemove(WaterCatchModel catchMeasure)
        {
            bool result = await App.Current.MainPage.DisplayAlert(null,
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/DeleteMessage"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Yes"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/No"]
            );

            if (result)
            {
                CatchMeasures.Value.Remove(catchMeasure);
            }
        }

        public static implicit operator List<InspectionCatchMeasureDto>(WaterCatchesViewModel viewModel)
        {
            return viewModel == null
                ? new List<InspectionCatchMeasureDto>()
                : viewModel.CatchMeasures.Select(f => f.Dto).ToList();
        }
    }
}
