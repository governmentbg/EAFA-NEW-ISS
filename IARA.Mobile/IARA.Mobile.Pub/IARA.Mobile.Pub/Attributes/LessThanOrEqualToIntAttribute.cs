using System;
using System.ComponentModel.DataAnnotations;

namespace IARA.Mobile.Pub.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class LessThanOrEqualToIntAttribute : ValidationAttribute
    {
        public LessThanOrEqualToIntAttribute(string otherValidStatePropertyName)
        {
            OtherValidStatePropertyName = otherValidStatePropertyName;
        }

        public string OtherValidStatePropertyName { get; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null
                || !validationContext.Items.TryGetValue(OtherValidStatePropertyName, out object otherValue)
                || otherValue == null)
            {
                return ValidationResult.Success;
            }

            string s = value.ToString();
            string os = otherValue.ToString();

            if (double.TryParse(s, out double valueRes) && double.TryParse(os, out double otherValueRes))
            {
                if (valueRes <= otherValueRes)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    string[] memberNames = validationContext.MemberName != null
                        ? new string[] { validationContext.MemberName }
                        : null;
                    return new ValidationResult(FormatErrorMessage(validationContext.DisplayName), memberNames);
                }
            }

            return ValidationResult.Success;
        }
    }
}
