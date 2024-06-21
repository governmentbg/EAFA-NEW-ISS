using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using System;
using System.ComponentModel.DataAnnotations;

namespace IARA.Mobile.Insp.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class RequiredIfFishingGearHasHooksAttribute : ValidationAttribute
    {
        public RequiredIfFishingGearHasHooksAttribute(string fishingGearProperty)
        {
            FishingGearProperty = fishingGearProperty;
        }

        public string FishingGearProperty { get; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            object fishingGearPropertyValue = validationContext.Items[FishingGearProperty];
            if (fishingGearPropertyValue == null)
            {
                return ValidationResult.Success;
            }
            FishingGearSelectNomenclatureDto fishingGear = fishingGearPropertyValue as FishingGearSelectNomenclatureDto;

            if (fishingGear.HasHooks)
            {
                if (string.IsNullOrEmpty(value.ToString()))
                {

                    return new RequiredAttribute
                    {
                        ErrorMessage = ErrorMessage
                    }.GetValidationResult(value, validationContext);
                }
                return ValidationResult.Success;
            }

            return ValidationResult.Success;
        }
    }
}
