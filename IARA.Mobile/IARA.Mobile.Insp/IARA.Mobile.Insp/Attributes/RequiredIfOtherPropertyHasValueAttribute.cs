using IARA.Mobile.Insp.Application.DTObjects.Nomenclatures;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IARA.Mobile.Insp.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class RequiredIfOtherPropertyHasValueAttribute : ValidationAttribute
    {
        public RequiredIfOtherPropertyHasValueAttribute(string otherValidStatePropertyName, string fishingGearProperty)
        {
            OtherValidStatePropertyName = otherValidStatePropertyName;
            FishingGearProperty = fishingGearProperty;
        }

        public string OtherValidStatePropertyName { get; }
        public string FishingGearProperty { get; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            object fishingGearPropertyValue = validationContext.Items[FishingGearProperty];
            if (fishingGearPropertyValue == null)
            {
                return ValidationResult.Success;
            }
            FishingGearSelectNomenclatureDto fishingGear = fishingGearPropertyValue as FishingGearSelectNomenclatureDto;

            if (RequireInputWhen.Contains(fishingGear.Code))
            {
                if (!validationContext.Items.TryGetValue(OtherValidStatePropertyName, out object otherValue))
                {
                    return ValidationResult.Success;
                }


                return new RequiredAttribute
                {
                    ErrorMessage = ErrorMessage
                }.GetValidationResult(value, validationContext);
            }
            return ValidationResult.Success;
        }

        private readonly List<string> RequireInputWhen = new List<string>()
        {
             "OTB", "OT", "OTT", "OTP", "PTB", "PT", "TB", "TBN", "TBS", "PUL"
        };
    }
}
