using System.Collections.Generic;
using IARA.Mobile.Insp.Application.DTObjects.Inspections;
using IARA.Mobile.Insp.Domain.Enums;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Models
{
    public class ValidStateObservation : ValidState<string>
    {
        public ValidStateObservation(List<TLValidator> validations, List<string> groups, IViewModelValidation validation)
            : base(validations, groups, validation)
        {
            base.Value = string.Empty;
        }

        public int? Id { get; set; }
        public InspectionObservationCategory Category { get; set; }

        public static implicit operator InspectionObservationTextDto(ValidStateObservation validState)
        {
            if (validState == null)
            {
                return null;
            }

            return new InspectionObservationTextDto
            {
                Id = validState.Id,
                Text = validState.Value,
                Category = validState.Category,
            };
        }
    }
}
