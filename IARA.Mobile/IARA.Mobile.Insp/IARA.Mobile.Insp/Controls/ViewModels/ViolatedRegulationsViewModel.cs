using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.ViolatedRegulationDialog;
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
    public class ViolatedRegulationsViewModel : ViewModel
    {
        private bool _hasViolations;

        public ViolatedRegulationsViewModel(InspectionPageViewModel inspection)
        {
            Inspection = inspection;

            AddViolation = CommandBuilder.CreateFrom(OnAddViolation);
            ViewViolation = CommandBuilder.CreateFrom<ViolatedRegulationModel>(OnViewViolation);
            EditViolation = CommandBuilder.CreateFrom<ViolatedRegulationModel>(OnEditViolation);
            RemoveViolation = CommandBuilder.CreateFrom<ViolatedRegulationModel>(OnRemoveViolation);

            this.AddValidation();
        }

        public InspectionPageViewModel Inspection { get; set; }
        public ValidStateTable<ViolatedRegulationModel> ViolatedRegulations { get; set; }

        public bool HasViolations
        {
            get { return _hasViolations; }
            set { SetProperty(ref _hasViolations, value); }
        }

        public ICommand AddViolation { get; set; }
        public ICommand ViewViolation { get; set; }
        public ICommand EditViolation { get; set; }
        public ICommand RemoveViolation { get; set; }

        public void OnEdit(List<AuanViolatedRegulationDto> violatedRegulations)
        {
            if (violatedRegulations != null)
            {
                ViolatedRegulations.Value.AddRange(violatedRegulations.Select(x => new ViolatedRegulationModel()
                {
                    Id = x.Id,
                    Article = x.Article,
                    Paragraph = x.Paragraph,
                    Section = x.Section,
                    Letter = x.Letter,
                    LawSectionId = x.LawSectionId,
                    LawText = x.LawText,
                    Comments = x.Comments,
                }).ToList());
            }
        }

        private async Task OnAddViolation()
        {
            ViolatedRegulationModel violatedRegulation = await TLDialogHelper.ShowDialog(new ViolatedRegulationDialog(ViewActivityType.Add, null));
            if (violatedRegulation != null)
            {
                ViolatedRegulations.Value.Add(violatedRegulation);
            }
        }

        private async Task OnViewViolation(ViolatedRegulationModel model)
        {
            await TLDialogHelper.ShowDialog(new ViolatedRegulationDialog(ViewActivityType.Review, model));
        }

        private async Task OnEditViolation(ViolatedRegulationModel model)
        {
            ViolatedRegulationModel violatedRegulation = await TLDialogHelper.ShowDialog(new ViolatedRegulationDialog(ViewActivityType.Add, null));
            if (violatedRegulation != null)
            {
                model.AssignFrom(violatedRegulation);

                ViolatedRegulations.Value.Replace(model, model);
            }
        }

        private async Task OnRemoveViolation(ViolatedRegulationModel model)
        {
            bool result = await App.Current.MainPage.DisplayAlert(null,
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/DeleteMessage"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Yes"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/No"]
            );

            if (result)
            {
                ViolatedRegulations.Value.Remove(model);
            }
        }
    }
}
