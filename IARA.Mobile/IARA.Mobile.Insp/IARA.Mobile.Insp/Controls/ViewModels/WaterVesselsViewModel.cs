using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.WaterVesselDialog;
using IARA.Mobile.Insp.Models;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class WaterVesselsViewModel : ViewModel
    {
        public WaterVesselsViewModel(InspectionPageViewModel inspection)
        {
            Inspection = inspection;

            Review = CommandBuilder.CreateFrom<WaterVesselModel>(OnReview);
            Add = CommandBuilder.CreateFrom(OnAdd);
            Edit = CommandBuilder.CreateFrom<WaterVesselModel>(OnEdit);
            Remove = CommandBuilder.CreateFrom<WaterVesselModel>(OnRemove);

            this.AddValidation();
        }

        public InspectionPageViewModel Inspection { get; }

        public ValidStateValidatableTable<WaterVesselModel> Vessels { get; set; }

        public ICommand Review { get; }
        public ICommand Add { get; }
        public ICommand Edit { get; }
        public ICommand Remove { get; }

        private Task OnReview(WaterVesselModel model)
        {
            return TLDialogHelper.ShowDialog(new WaterVesselDialog(this, Inspection, ViewActivityType.Review, model));
        }

        private async Task OnAdd()
        {
            WaterVesselModel result = await TLDialogHelper.ShowDialog(new WaterVesselDialog(this, Inspection, ViewActivityType.Add));

            if (result != null)
            {
                Vessels.Value.Add(result);
            }
        }

        private async Task OnEdit(WaterVesselModel vessel)
        {
            WaterVesselModel result = await TLDialogHelper.ShowDialog(new WaterVesselDialog(this, Inspection, ViewActivityType.Edit, vessel));

            if (result != null)
            {
                vessel.Dto = result.Dto;
                vessel.AllChanged();
            }
        }

        private async Task OnRemove(WaterVesselModel vessel)
        {
            bool result = await App.Current.MainPage.DisplayAlert(null,
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/DeleteMessage"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Yes"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/No"]
            );

            if (result)
            {
                Vessels.Value.Remove(vessel);
            }
        }

        public static implicit operator List<WaterInspectionVesselDto>(WaterVesselsViewModel viewModel)
        {
            return viewModel == null
                ? new List<WaterInspectionVesselDto>()
                : viewModel.Vessels.Select(f => f.Dto).ToList();
        }
    }
}
