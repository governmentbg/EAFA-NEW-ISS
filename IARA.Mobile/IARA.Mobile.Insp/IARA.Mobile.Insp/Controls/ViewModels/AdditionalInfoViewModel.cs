using System.ComponentModel.DataAnnotations;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Base;
using IARA.Mobile.Insp.Domain.Enums;
using IARA.Mobile.Insp.Models;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Controls.ViewModels
{
    public class AdditionalInfoViewModel : ViewModel
    {
        public AdditionalInfoViewModel(InspectionPageViewModel inspection)
        {
            Inspection = inspection;

            this.AddValidation();

            ObservationsOrViolations.Category = InspectionObservationCategory.AdditionalInfo;
        }

        public InspectionPageViewModel Inspection { get; }

        [MaxLength(4000)]
        public ValidStateObservation ObservationsOrViolations { get; set; }

        [MaxLength(4000)]
        public ValidState InspectorComment { get; set; }

        [MaxLength(4000)]
        public ValidState ActionsTaken { get; set; }

        public ValidStateBool AdministrativeViolation { get; set; }

        public void OnEdit(InspectionEditDto dto)
        {
            ActionsTaken.Value = dto.ActionsTaken ?? string.Empty;
            InspectorComment.Value = dto.InspectorComment ?? string.Empty;
            AdministrativeViolation.Value = dto.AdministrativeViolation ?? false;

            InspectionObservationTextDto additionalInfoObservation = dto.ObservationTexts?.Find(f => f.Category == InspectionObservationCategory.AdditionalInfo);

            if (additionalInfoObservation != null)
            {
                ObservationsOrViolations.Id = additionalInfoObservation.Id;
                ObservationsOrViolations.Value = additionalInfoObservation.Text ?? string.Empty;
            }
        }
    }
}
