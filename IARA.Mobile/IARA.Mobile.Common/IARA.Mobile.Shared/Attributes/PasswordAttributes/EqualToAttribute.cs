using System;
using System.ComponentModel.DataAnnotations;

namespace IARA.Mobile.Shared.Attributes.PasswordAttributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class EqualToAttribute : ValidationAttribute
    {
        public EqualToAttribute(string otherValidStatePropertyName)
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

            if (value.ToString() == otherValue.ToString())
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
    }
}
