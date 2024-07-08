using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Models;
using System.ComponentModel.DataAnnotations;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class AdditionalInfoViewModel : ViewModel
    {
        private bool _hasViolations;
        public AdditionalInfoViewModel(InspectionPageViewModel inspection)
        {
            Inspection = inspection;
            ViolatedRegulations = new ViolatedRegulationsViewModel(inspection);

            this.AddValidation();

            ObservationsOrViolations.Category = InspectionObservationCategory.AdditionalInfo;
        }

        public InspectionPageViewModel Inspection { get; }
        public ViolatedRegulationsViewModel ViolatedRegulations { get; }

        [MaxLength(4000)]
        public ValidStateObservation ObservationsOrViolations { get; set; }

        [MaxLength(4000)]
        public ValidState InspectorComment { get; set; }

        [MaxLength(4000)]
        public ValidState ActionsTaken { get; set; }

        public bool HasViolations
        {
            get { return _hasViolations; }
            set { _hasViolations = value; }
        }


        public void OnEdit(InspectionEditDto dto)
        {
            ActionsTaken.Value = dto.ActionsTaken ?? string.Empty;
            InspectorComment.Value = dto.InspectorComment ?? string.Empty;
            ViolatedRegulations.HasViolations = dto.AdministrativeViolation ?? false;
            ViolatedRegulations.OnEdit(dto.ViolatedRegulations);

            InspectionObservationTextDto additionalInfoObservation = dto.ObservationTexts?.Find(f => f.Category == InspectionObservationCategory.AdditionalInfo);

            if (additionalInfoObservation != null)
            {
                ObservationsOrViolations.Id = additionalInfoObservation.Id;
                ObservationsOrViolations.Value = additionalInfoObservation.Text ?? string.Empty;
            }
        }
    }
}
