using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Shared.Attributes;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Base;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Shared.ViewModels.Models.ReportParameters
{
    public class NomenclatureReportValidation : TLBaseViewModel, IReportValidation
    {
        public NomenclatureReportValidation(bool isMandatory, List<SelectNomenclatureDto> nomenclatures, string pattern, string errorMessage)
        {
            ItemsSource = nomenclatures;

            this.AddValidation();

            if (isMandatory)
            {
                ValidState.HasAsterisk = true;
                ValidState.Validations.Add(new TLValidator(new RequiredAttribute(), nameof(RequiredAttribute)));
            }
            if (!string.IsNullOrEmpty(pattern))
            {
                ValidState.Validations.Add(new TLValidator(new NomenclatureRegularExpressionAttribute(pattern, errorMessage), nameof(NomenclatureRegularExpressionAttribute)));
            }
        }

        public List<SelectNomenclatureDto> ItemsSource { get; }

        public ValidStateSelect<SelectNomenclatureDto> ValidState { get; set; }

        IValidState IReportValidation.ValidState => ValidState;

        public string GetValue()
        {
            return ValidState.Value?.Id.ToString();
        }
    }
}
