using IARA.Mobile.Application;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.Interfaces.Transactions;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Helpers;
using IARA.Mobile.Insp.Models;
using IARA.Mobile.Insp.ViewModels.Models;
using System;
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
        public ViolatedRegulationDialogViewModel()
        {
            Save = CommandBuilder.CreateFrom(OnSave);
            RegulationChosen = CommandBuilder.CreateFrom<AuanViolatedRegulationDto>(OnRegulationChosaen);
            this.AddValidation();

            Regulation.ItemsSource = new TLObservableCollection<AuanViolatedRegulationDto>();
            Regulation.GetMore = (int page, int pageSize, string search) =>
                DependencyService.Resolve<INomenclatureTransaction>().GetLaws(page, pageSize, search);
        }

        public ViolatedRegulationModel Edit { get; set; }
        public ViewActivityType DialogType { get; set; }

        [Required]
        public ValidState Article { get; set; }
        public ValidState Paragraph { get; set; }
        public ValidState Section { get; set; }
        public ValidState Letter { get; set; }
        public ValidState Comments { get; set; }
        public ValidState LawText { get; set; }

        public ValidStateInfiniteSelect<AuanViolatedRegulationDto> Regulation { get; set; }

        public ICommand Save { get; set; }
        public ICommand RegulationChosen { get; set; }

        public override Task Initialize(object sender)
        {
            Regulation.ItemsSource.AddRange(DependencyService.Resolve<INomenclatureTransaction>().GetLaws(0, CommonGlobalVariables.PullItemsCount));

            if (Edit != null)
            {
                Article.AssignFrom(Edit.Article);
                Paragraph.AssignFrom(Edit.Paragraph);
                Section.AssignFrom(Edit.Section);
                Letter.AssignFrom(Edit.Letter);
                Comments.AssignFrom(Edit.Comments);
                LawText.AssignFrom(Edit.LawText);
            }

            return Task.CompletedTask;
        }

        private void OnRegulationChosaen(AuanViolatedRegulationDto dto)
        {
            throw new NotImplementedException();
        }

        private Task OnSave()
        {
            return HideDialog(new ViolatedRegulationModel()
            {
                Id = Edit == null ? null : Edit.Id,
                Article = Article.Value,
                Paragraph = Paragraph.Value,
                Section = Section.Value,
                Letter = Letter.Value,
                Comments = Comments.Value,
                LawText = LawText.Value
            });
        }
    }
}
