using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class SelectLessThanOrEqualToIntAttribute : ValidationAttribute, ITLValidatable
    {
        public SelectLessThanOrEqualToIntAttribute(string otherPropertyName, string selectPropertyName)
        {
            OtherPropertyName = otherPropertyName;
            SelectPropertyName = selectPropertyName;
        }

        public string OtherPropertyName { get; }
        public string SelectPropertyName { get; }

        public ValidationResult IsValid(object value, TLValidationContext validationContext)
        {
            if (value == null
                || !validationContext.Items.TryGetValue(OtherPropertyName, out IValidState otherValue)
                || otherValue == null)
            {
                return ValidationResult.Success;
            }

            Type type = value.GetType();
            PropertyInfo prop = type.GetProperty(SelectPropertyName);
            object obj = prop.GetValue(value, null);

            string s = obj?.ToString();
            string os = otherValue.ObjectValue.ToString();

            if (double.TryParse(s, out double valueRes) && double.TryParse(os, out double otherValueRes))
            {
                return valueRes <= otherValueRes
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
