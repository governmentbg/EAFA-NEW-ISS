using System;
using System.ComponentModel.DataAnnotations;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class BiggerThanOrEqualToIntAttribute : ValidationAttribute, ITLValidatable
    {
        public BiggerThanOrEqualToIntAttribute(string otherPropertyName)
        {
            OtherPropertyName = otherPropertyName;
        }

        public string OtherPropertyName { get; }

        public ValidationResult IsValid(object value, TLValidationContext validationContext)
        {
            if (value == null
                || !validationContext.Items.TryGetValue(OtherPropertyName, out IValidState otherValue)
                || otherValue == null)
            {
                return ValidationResult.Success;
            }

            string s = value.ToString();
            string os = otherValue.ObjectValue.ToString();

            if (double.TryParse(s, out double valueRes) && double.TryParse(os, out double otherValueRes))
            {
                return valueRes >= otherValueRes
                    ? ValidationResult.Success
                    : new ValidationResult(FormatErrorMessage(validationContext.DisplayName, otherValue.Title));
            }

            return ValidationResult.Success;
        }

        protected virtual string FormatErrorMessage(string name, string otherName)
        {
            return string.Format(ErrorMessage, name, otherName);
        }
    }
}
