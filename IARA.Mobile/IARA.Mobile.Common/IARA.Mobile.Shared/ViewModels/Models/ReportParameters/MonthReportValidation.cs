using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Application.Extensions;
using IARA.Mobile.Shared.Attributes;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Base;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Shared.ViewModels.Models.ReportParameters
{
    public class MonthReportValidation : TLBaseViewModel, IReportValidation
    {
        public MonthReportValidation(bool isMandatory, string defaultValue, string pattern, string errorMessage)
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
            if (!string.IsNullOrEmpty(defaultValue) && int.TryParse(defaultValue, out int result) && result >= 1 && result <= 12)
            {
                ValidState.Value = new SelectNomenclatureDto
                {
                    Id = result,
                    Name = new DateTime(1900, result, 1).ToString("MMMM", CultureInfo.CurrentUICulture).CapitalizeFirstLetter()
                };
            }
        }

        public ValidStateSelect<SelectNomenclatureDto> ValidState { get; set; }

        IValidState IReportValidation.ValidState => ValidState;

        public string GetValue()
        {
            return ValidState.Value != null
                ? new DateTime(1900, ValidState.Value.Id, 1).ToString(CultureInfo.InvariantCulture)
                : null;
        }
    }
}
