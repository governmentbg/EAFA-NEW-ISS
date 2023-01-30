using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.EngineDialog;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class EnginesViewModel : ViewModel
    {
        public EnginesViewModel(InspectionPageViewModel inspection)
        {
            Inspection = inspection;

            Review = CommandBuilder.CreateFrom<WaterInspectionEngineDto>(OnReview);
            Add = CommandBuilder.CreateFrom(OnAdd);
            Edit = CommandBuilder.CreateFrom<WaterInspectionEngineDto>(OnEdit);
            Remove = CommandBuilder.CreateFrom<WaterInspectionEngineDto>(OnRemove);

            this.AddValidation();
        }

        public InspectionPageViewModel Inspection { get; }

        public ValidStateTable<WaterInspectionEngineDto> Engines { get; set; }

        public ICommand Review { get; }
        public ICommand Add { get; }
        public ICommand Edit { get; }
        public ICommand Remove { get; }

        private Task OnReview(WaterInspectionEngineDto model)
        {
            return TLDialogHelper.ShowDialog(new EngineDialog(this, Inspection, ViewActivityType.Review, model));
        }

        private async Task OnAdd()
        {
            WaterInspectionEngineDto result = await TLDialogHelper.ShowDialog(new EngineDialog(this, Inspection, ViewActivityType.Add));

            if (result != null)
            {
                Engines.Value.Add(result);
            }
        }

        private async Task OnEdit(WaterInspectionEngineDto engine)
        {
            WaterInspectionEngineDto result = await TLDialogHelper.ShowDialog(new EngineDialog(this, Inspection, ViewActivityType.Edit, engine));

            if (result != null)
            {
                Engines.Value.Replace(engine, result);
            }
        }

        private async Task OnRemove(WaterInspectionEngineDto engine)
        {
            bool result = await App.Current.MainPage.DisplayAlert(null,
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/DeleteMessage"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Yes"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/No"]
            );

            if (result)
            {
                Engines.Value.Remove(engine);
            }
        }

        public static implicit operator List<WaterInspectionEngineDto>(EnginesViewModel viewModel)
        {
            return viewModel == null
                ? new List<WaterInspectionEngineDto>()
                : viewModel.Engines.ToList();
        }
    }
}
