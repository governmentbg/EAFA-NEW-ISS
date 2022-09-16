using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Application.Interfaces.Transactions;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.DeclarationCatchDialog;
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
    public class DeclarationCatchesViewModel : ViewModel
    {
        public DeclarationCatchesViewModel(InspectionPageViewModel inspection, bool hasCatchType = true)
        {
            Inspection = inspection;
            HasCatchType = hasCatchType;

            Review = CommandBuilder.CreateFrom<DeclarationCatchModel>(OnReview);
            Add = CommandBuilder.CreateFrom(OnAdd);
            Edit = CommandBuilder.CreateFrom<DeclarationCatchModel>(OnEdit);
            Remove = CommandBuilder.CreateFrom<DeclarationCatchModel>(OnRemove);

            this.AddValidation();
        }

        public InspectionPageViewModel Inspection { get; }
        public bool HasCatchType { get; }

        public ValidStateValidatableTable<DeclarationCatchModel> Catches { get; set; }

        public ICommand Review { get; }
        public ICommand Add { get; }
        public ICommand Edit { get; }
        public ICommand Remove { get; }

        public void OnEdit(List<InspectedDeclarationCatchDto> catches)
        {
            if (catches == null || catches.Count == 0)
            {
                return;
            }

            INomenclatureTransaction nomTransaction = NomenclaturesTransaction;

            List<CatchZoneNomenclatureDto> catchZones = nomTransaction.GetCatchZones();
            List<SelectNomenclatureDto> fishTypes = nomTransaction.GetFishes();
            List<SelectNomenclatureDto> presentations = nomTransaction.GetFishPresentations();
            List<SelectNomenclatureDto> catchTypes = nomTransaction.GetCatchInspectionTypes();

            Catches.Value.ReplaceRange(catches.ConvertAll(f => new DeclarationCatchModel
            {
                CatchType = catchTypes.Find(s => s.Id == f.FishTypeId)?.DisplayValue,
                Type = fishTypes.Find(s => s.Id == f.FishTypeId)?.DisplayValue,
                CatchZone = catchZones.Find(s => s.Id == f.CatchZoneId)?.Name,
                Presentation = presentations.Find(s => s.Id == f.PresentationId)?.Name,
                Dto = f,
            }));
        }

        private Task OnReview(DeclarationCatchModel model)
        {
            return TLDialogHelper.ShowDialog(new DeclarationCatchDialog(this, Inspection, HasCatchType, ViewActivityType.Review, model));
        }

        private async Task OnAdd()
        {
            DeclarationCatchModel result = await TLDialogHelper.ShowDialog(new DeclarationCatchDialog(this, Inspection, HasCatchType, ViewActivityType.Add));

            if (result != null && result.Dto != null)
            {
                Catches.Value.Add(result);
            }
        }

        private async Task OnEdit(DeclarationCatchModel model)
        {
            DeclarationCatchModel result = await TLDialogHelper.ShowDialog(new DeclarationCatchDialog(this, Inspection, HasCatchType, ViewActivityType.Edit, model));

            if (result != null && result.Dto != null)
            {
                model.Dto = result.Dto;
                model.AllChanged();
            }
        }

        private async Task OnRemove(DeclarationCatchModel model)
        {
            bool result = await App.Current.MainPage.DisplayAlert(null,
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/DeleteMessage"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Yes"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/No"]
            );

            if (result)
            {
                Catches.Value.Remove(model);
            }
        }

        public static implicit operator List<InspectedDeclarationCatchDto>(DeclarationCatchesViewModel viewModel)
        {
            return viewModel.Catches
                .Select(f => f.Dto)
                .ToList();
        }
    }
}
