using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Attributes;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.PatrolVehicleDialog;
using IARA.Mobile.Insp.Models;
using TechnoLogica.Xamarin.Commands;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class PatrolVehiclesViewModel : ViewModel
    {
        public PatrolVehiclesViewModel(InspectionPageViewModel inspection, bool? isWaterVehicle)
        {
            Inspection = inspection;
            IsWaterVehicle = isWaterVehicle;

            Review = CommandBuilder.CreateFrom<PatrolVehicleModel>(OnReview);
            Add = CommandBuilder.CreateFrom(OnAdd);
            Edit = CommandBuilder.CreateFrom<PatrolVehicleModel>(OnEdit);
            Remove = CommandBuilder.CreateFrom<PatrolVehicleModel>(OnRemove);

            this.AddValidation();
        }

        public bool? IsWaterVehicle { get; }

        public InspectionPageViewModel Inspection { get; }

        [ListMinLength(1)]
        public ValidStateTable<PatrolVehicleModel> InspectorVehicles { get; set; }

        public ICommand Review { get; }
        public ICommand Add { get; }
        public ICommand Edit { get; }
        public ICommand Remove { get; }

        public void OnEdit(List<VesselDuringInspectionDto> patrolVehicles)
        {
            if (patrolVehicles == null || patrolVehicles.Count == 0)
            {
                return;
            }

            List<SelectNomenclatureDto> insitutions = NomenclaturesTransaction.GetInstitutions();
            List<SelectNomenclatureDto> patrolVehicleTypes = NomenclaturesTransaction.GetPatrolVehicleTypes(IsWaterVehicle);

            List<PatrolVehicleModel> vehicles = patrolVehicles.ConvertAll(f => new PatrolVehicleModel
            {
                Institution = insitutions.Find(s => s.Id == f.InstitutionId)?.Code,
                PatrolVehicleType = patrolVehicleTypes.Find(s => s.Id == f.PatrolVehicleTypeId)?.Name,
                Dto = f
            });

            InspectorVehicles.Value.AddRange(vehicles);
        }

        private Task OnReview(PatrolVehicleModel model)
        {
            return TLDialogHelper.ShowDialog(new PatrolVehicleDialog(this, IsWaterVehicle, Inspection, ViewActivityType.Review, model));
        }

        private async Task OnAdd()
        {
            PatrolVehicleModel result = await TLDialogHelper.ShowDialog(new PatrolVehicleDialog(this, IsWaterVehicle, Inspection, ViewActivityType.Add));

            if (result != null && result.Dto != null)
            {
                InspectorVehicles.Value.Add(result);
            }
        }

        private async Task OnEdit(PatrolVehicleModel model)
        {
            PatrolVehicleModel result = await TLDialogHelper.ShowDialog(new PatrolVehicleDialog(this, IsWaterVehicle, Inspection, ViewActivityType.Edit, model));

            if (result != null && result.Dto != null)
            {
                model.AssignFrom(result);
                InspectorVehicles.Value.Replace(model, model);
            }
        }

        private async Task OnRemove(PatrolVehicleModel model)
        {
            bool result = await App.Current.MainPage.DisplayAlert(null,
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/DeleteMessage"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/Yes"],
                TranslateExtension.Translator[nameof(GroupResourceEnum.Common) + "/No"]
            );

            if (result)
            {
                InspectorVehicles.Value.Remove(model);
            }
        }

        public static implicit operator List<VesselDuringInspectionDto>(PatrolVehiclesViewModel viewModel)
        {
            return viewModel.InspectorVehicles
                .Select(f => f.Dto)
                .ToList();
        }
    }
}
