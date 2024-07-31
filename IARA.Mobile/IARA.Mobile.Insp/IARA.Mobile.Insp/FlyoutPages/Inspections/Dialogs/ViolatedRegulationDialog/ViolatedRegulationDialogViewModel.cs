using IARA.Mobile.Application;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.Interfaces.Transactions;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Insp.Models;
using IARA.Mobile.Insp.ViewModels.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Windows.Input;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Base;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.ViolatedRegulationDialog
{
    public class ViolatedRegulationDialogViewModel : TLBaseDialogViewModel<ViolatedRegulationModel>
    {
        private bool _isRegulationChosen;
        public ViolatedRegulationDialogViewModel()
        {
            Save = CommandBuilder.CreateFrom(OnSave);
            RegulationChosen = CommandBuilder.CreateFrom<AuanViolatedRegulationDto>(OnRegulationChosen);
            SwitchRegulationExists = CommandBuilder.CreateFrom<bool>(OnSwitchRegulationExists);
            this.AddValidation();

            Regulation.ItemsSource = new TLObservableCollection<AuanViolatedRegulationDto>();
            Regulation.GetMore = (int page, int pageSize, string search) =>
                DependencyService.Resolve<INomenclatureTransaction>().GetLaws(page, pageSize, search);
        }

        public ViolatedRegulationModel Edit { get; set; }
        public ViewActivityType DialogType { get; set; }
        public bool IsRegulationChosen
        {
            get => _isRegulationChosen;
            set => SetProperty(ref _isRegulationChosen, value);
        }


        [Required]
        public ValidState Article { get; set; }
        public ValidState Paragraph { get; set; }
        public ValidState Section { get; set; }
        public ValidState Letter { get; set; }
        public ValidState Comments { get; set; }
        public ValidState LawText { get; set; }

        public ValidStateBool RegulationExists { get; set; }
        public ValidStateInfiniteSelect<AuanViolatedRegulationDto> Regulation { get; set; }

        public ICommand Save { get; set; }
        public ICommand RegulationChosen { get; set; }
        public ICommand SwitchRegulationExists { get; set; }

        public override Task Initialize(object sender)
        {
            Regulation.ItemsSource.AddRange(DependencyService.Resolve<INomenclatureTransaction>().GetLaws(0, CommonGlobalVariables.PullItemsCount));
            RegulationExists.AssignFrom(true);
            if (Edit != null)
            {
                if (Edit.LawSectionId != null)
                {
                    AuanViolatedRegulationDto violatedRegulation = DependencyService.Resolve<INomenclatureTransaction>().GetLaw(Edit.LawSectionId.Value);
                    Regulation.Value = violatedRegulation;
                    OnRegulationChosen(violatedRegulation);
                }
                else
                {
                    Article.AssignFrom(Edit.Article);
                    Paragraph.AssignFrom(Edit.Paragraph);
                    Section.AssignFrom(Edit.Section);
                    Letter.AssignFrom(Edit.Letter);
                    Comments.AssignFrom(Edit.Comments);
                    LawText.AssignFrom(Edit.LawText);
                    RegulationExists.AssignFrom(false);
                }
            }

            return Task.CompletedTask;
        }

        private void OnSwitchRegulationExists(bool regulationExists)
        {
            if (regulationExists)
            {
                Regulation.Value = null;
                Article.Value = null;
                Paragraph.Value = null;
                Section.Value = null;
                Letter.Value = null;
                Comments.Value = null;
                LawText.Value = null;
                IsRegulationChosen = false;
            }
        }

        private void OnRegulationChosen(AuanViolatedRegulationDto dto)
        {
            if (dto != null)
            {
                Article.AssignFrom(dto.Article);
                Paragraph.AssignFrom(dto.Paragraph);
                Section.AssignFrom(dto.Section);
                Letter.AssignFrom(dto.Letter);
                Comments.AssignFrom(dto.Comments);
                LawText.AssignFrom(dto.LawText);
                IsRegulationChosen = true;

                Validation.Force();
            }
        }

        private Task OnSave()
        {
            Validation.Force();

            if (!Validation.IsValid)
            {
                return Task.CompletedTask;
            }

            return HideDialog(new ViolatedRegulationModel()
            {
                Id = Edit == null ? null : Edit.Id,
                Article = Article.Value,
                Paragraph = Paragraph.Value,
                Section = Section.Value,
                Letter = Letter.Value,
                Comments = Comments.Value,
                LawText = LawText.Value,
                LawSectionId = Regulation.Value?.Id,
            });
        }
    }
}
