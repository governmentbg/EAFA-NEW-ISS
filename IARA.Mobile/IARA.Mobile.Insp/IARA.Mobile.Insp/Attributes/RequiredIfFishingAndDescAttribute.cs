using IARA.Mobile.Application.DTObjects.Nomenclatures;
using IARA.Mobile.Insp.Converters;
using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace IARA.Mobile.Insp.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class RequiredIfFishingAndDescAttribute : ValidationAttribute
    {
        public RequiredIfFishingAndDescAttribute(string fishingValidStatePropertyName, string otherValidStatePropertyName)
        {
            FishingValidStatePropertyName = fishingValidStatePropertyName;
            OtherValidStatePropertyName = otherValidStatePropertyName;
        }

        public string FishingValidStatePropertyName { get; }
        public string OtherValidStatePropertyName { get; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!validationContext.Items.TryGetValue(FishingValidStatePropertyName, out object fishingValue)
                || !validationContext.Items.TryGetValue(OtherValidStatePropertyName, out object otherValue)
                || !HasFishing(fishingValue) || !HasDescription(otherValue))
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

        private static bool HasFishing(object value)
        {
            if (value is SelectNomenclatureDto nom)
            {
                return HasFishingConverter.ConvertFrom(nom);
            }
            else if (value is IList list)
            {
                return HasFishingConverter.ConvertFrom(list);
            }

            return false;
        }
    }
}
