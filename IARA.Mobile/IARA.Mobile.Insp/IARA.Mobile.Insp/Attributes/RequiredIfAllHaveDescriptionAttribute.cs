using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Converters;

namespace IARA.Mobile.Insp.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class RequiredIfAllHaveDescriptionAttribute : ValidationAttribute
    {
        public RequiredIfAllHaveDescriptionAttribute(params string[] otherValidStatePropertyNames)
        {
            OtherValidStatePropertyNames = otherValidStatePropertyNames;
        }

        public string[] OtherValidStatePropertyNames { get; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            bool allContain = true;

            for (int i = 0; i < OtherValidStatePropertyNames.Length; i++)
            {
                if (!validationContext.Items.TryGetValue(OtherValidStatePropertyNames[i], out object otherValue) || !HasDescription(otherValue))
                {
                    allContain = false;
                    break;
                }
            }

            if (!allContain)
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
