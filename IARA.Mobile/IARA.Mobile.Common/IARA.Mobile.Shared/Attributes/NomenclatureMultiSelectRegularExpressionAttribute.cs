using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using IARA.Mobile.Application.DTObjects.Nomenclatures;


namespace IARA.Mobile.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NomenclatureMultiSelectRegularExpressionAttribute : ValidationAttribute
    {
        public NomenclatureMultiSelectRegularExpressionAttribute(string pattern, string customErrorMessage)
        {
            Pattern = pattern;
            CustomErrorMessage = customErrorMessage;
        }

        public string Pattern { get; }
        public string CustomErrorMessage { get; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IEnumerable list)
            {
                foreach (object item in list)
                {
                    if (item is SelectNomenclatureDto nom)
                    {
                        ValidationResult result = new CustomRegularExpressionAttribute(Pattern, CustomErrorMessage)
                            .GetValidationResult(nom.Name, validationContext);

                        if (result.ErrorMessage != null)
                        {
                            return result;
                        }
                    }
                }
            }

            return ValidationResult.Success;
        }

        public override string FormatErrorMessage(string name)
        {
            return CustomErrorMessage;
        }
    }
}
