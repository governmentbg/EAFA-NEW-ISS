using System;
using System.ComponentModel.DataAnnotations;

namespace IARA.Mobile.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class RequiredIfBooleanEqualsAttribute : ValidationAttribute
    {
        public RequiredIfBooleanEqualsAttribute(string otherValidStatePropertyName, bool equals = true)
        {
            OtherValidStatePropertyName = otherValidStatePropertyName;
            BooleanEquals = equals;
        }

        public string OtherValidStatePropertyName { get; }
        public bool BooleanEquals { get; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!validationContext.Items.TryGetValue(OtherValidStatePropertyName, out object otherValue)
                || !(otherValue is bool otherBool) || otherBool == BooleanEquals)
            {
                return ValidationResult.Success;
            }

            return new RequiredAttribute
            {
                ErrorMessage = ErrorMessage
            }.GetValidationResult(value, validationContext);
        }
    }
}
