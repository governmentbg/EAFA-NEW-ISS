using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Converters;

namespace IARA.Mobile.Insp.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class RequiredIfHasDescriptionAttribute : ValidationAttribute
    {
        public RequiredIfHasDescriptionAttribute(string otherValidStatePropertyName)
        {
            OtherValidStatePropertyName = otherValidStatePropertyName;
        }

        public string OtherValidStatePropertyName { get; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!validationContext.Items.TryGetValue(OtherValidStatePropertyName, out object otherValue) || !HasDescription(otherValue))
            {
                return ValidationResult.Success;
            }

            return new RequiredAttribute
            {
                ErrorMessage = ErrorMessage
            }.GetValidationResult(value, validationContext);
        }

        private static bool HasDescription(object value)
        {
            if (value is DescrSelectNomenclatureDto nom)
            {
                return HasDescriptionConverter.ConvertFrom(nom);
            }
            else if (value is IList list)
            {
                return HasDescriptionConverter.ConvertFrom(list);
            }

            return false;
        }
    }
}
