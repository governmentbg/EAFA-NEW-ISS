using System;
using System.ComponentModel.DataAnnotations;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class DifferenceBetweenTwoIntAttributesLowerThanAttribute : ValidationAttribute, ITLValidatable
    {
        public DifferenceBetweenTwoIntAttributesLowerThanAttribute(string otherPropertyName, int lowerThan)
        {
            OtherPropertyName = otherPropertyName;
            LowerThan = lowerThan;
        }

        public string OtherPropertyName { get; }
        public int LowerThan { get; }

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
                return valueRes - otherValueRes <= LowerThan
                    ? ValidationResult.Success
                    : new ValidationResult(FormatErrorMessage());
            }

            return ValidationResult.Success;
        }

        protected virtual string FormatErrorMessage()
        {
            return string.Format(ErrorMessage, LowerThan);
        }
    }
}
