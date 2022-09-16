using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Shared.Attributes;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ViewModels.Base;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Shared.ViewModels.Models.ReportParameters
{
    public class NomenclatureMultiSelectReportValidation : TLBaseViewModel, IReportValidation
    {
        public NomenclatureMultiSelectReportValidation(bool isMandatory, List<SelectNomenclatureDto> nomenclatures, string pattern, string errorMessage)
        {
            ItemsSource = nomenclatures;

            this.AddValidation();

            if (isMandatory)
            {
                ValidState.HasAsterisk = true;
                ValidState.Validations.Add(new TLValidator(new ListMinLengthAttribute(1) { ErrorMessageResourceName = nameof(RequiredAttribute) }, nameof(RequiredAttribute)));
            }
            if (!string.IsNullOrEmpty(pattern))
            {
                ValidState.Validations.Add(new TLValidator(new NomenclatureMultiSelectRegularExpressionAttribute(pattern, errorMessage), nameof(NomenclatureMultiSelectRegularExpressionAttribute)));
            }
        }

        public List<SelectNomenclatureDto> ItemsSource { get; }

        public ValidStateMultiSelect<SelectNomenclatureDto> ValidState { get; set; }

        IValidState IReportValidation.ValidState => ValidState;

        public string GetValue()
        {
            return string.Join(",", ValidState.Value.Select(f => f.Id));
        }
    }
}
