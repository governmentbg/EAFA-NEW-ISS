using IARA.Mobile.Application.DTObjects.Nomenclatures;
using System;
using System.ComponentModel.DataAnnotations;

namespace IARA.Mobile.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NomenclatureRegularExpressionAttribute : ValidationAttribute
    {
        public NomenclatureRegularExpressionAttribute(string pattern, string customErrorMessage)
        {
            Pattern = pattern;
            CustomErrorMessage = customErrorMessage;
        }

        public string Pattern { get; }
        public string CustomErrorMessage { get; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!(value is SelectNomenclatureDto nom))
            {
                return ValidationResult.Success;
            }

            return new CustomRegularExpressionAttribute(Pattern, CustomErrorMessage)
                .GetValidationResult(nom.Name, validationContext);
        }

        public override string FormatErrorMessage(string name)
        {
            return CustomErrorMessage;
        }
    }
}
