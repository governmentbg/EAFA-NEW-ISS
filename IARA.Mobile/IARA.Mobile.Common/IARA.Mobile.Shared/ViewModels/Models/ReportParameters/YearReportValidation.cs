using System;
using System.Globalization;
using IARA.Mobile.Application.Attributes;
using IARA.Mobile.Shared.Attributes;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Base;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Shared.ViewModels.Models.ReportParameters
{
    public class YearReportValidation : TLBaseViewModel, IReportValidation
    {
        public YearReportValidation(bool isMandatory, string defaultValue, string pattern, string errorMessage)
        {
            this.AddValidation();

            if (isMandatory)
            {
                ValidState.HasAsterisk = true;
                ValidState.Validations.Add(new TLValidator(new YearValidAttribute { ErrorMessageResourceName = "Required" }, nameof(YearValidAttribute)));
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

        [TLRange(1900, 9999)]
        public ValidState ValidState { get; set; }

        IValidState IReportValidation.ValidState => ValidState;

        public string GetValue()
        {
            return int.TryParse(ValidState.Value, out int year)
                ? new DateTime(year, 1, 1).ToString(CultureInfo.InvariantCulture)
                : null;
        }
    }
}
