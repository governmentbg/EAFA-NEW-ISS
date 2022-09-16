using IARA.Mobile.Shared.Attributes;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Base;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Shared.ViewModels.Models.ReportParameters
{
    public class DateReportValidation : TLBaseViewModel, IReportValidation
    {
        public DateReportValidation(bool isMandatory, string defaultValue, string pattern, string errorMessage)
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

            if (DateTime.TryParse(defaultValue, out DateTime result))
            {
                ValidState.Value = result;
            }
        }

        public ValidStateDate ValidState { get; set; }

        IValidState IReportValidation.ValidState => ValidState;

        public string GetValue()
        {
            return ValidState.Value?.ToString("yyyy-MM-dd");
        }
    }
}
