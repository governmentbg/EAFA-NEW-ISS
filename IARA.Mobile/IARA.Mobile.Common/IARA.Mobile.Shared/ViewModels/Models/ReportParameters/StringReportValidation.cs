using IARA.Mobile.Shared.Attributes;
using System.ComponentModel.DataAnnotations;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Base;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Shared.ViewModels.Models.ReportParameters
{
    public class StringReportValidation : TLBaseViewModel, IReportValidation
    {
        public StringReportValidation(bool isMandatory, string defaultValue, string pattern, string errorMessage)
        {
            this.AddValidation();

            if (isMandatory)
            {
                ValidState.HasAsterisk = true;
                ValidState.Validations.Add(new TLValidator(new RequiredAttribute(), nameof(RequiredAttribute)));
            }
            if (!string.IsNullOrEmpty(pattern))
            {
                ValidState.Validations.Add(new TLValidator(new CustomRegularExpressionAttribute(pattern, errorMessage), nameof(CustomRegularExpressionAttribute)));
            }
            if (!string.IsNullOrEmpty(defaultValue))
            {
                ValidState.Value = defaultValue;
            }
        }

        public ValidState ValidState { get; set; }

        IValidState IReportValidation.ValidState => ValidState;

        public string GetValue()
        {
            return ValidState.Value;
        }
    }
}
