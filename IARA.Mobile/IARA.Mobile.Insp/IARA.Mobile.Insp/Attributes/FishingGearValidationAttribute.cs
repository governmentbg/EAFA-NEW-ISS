using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TechnoLogica.Xamarin.Helpers;
using TechnoLogica.Xamarin.ResourceTranslator;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class FishingGearValidationAttribute : ValidationAttribute, ITLValidatable
    {
        public ValidationResult IsValid(object value, TLValidationContext validationContext)
        {
            List<string> invalidGearTypes = new List<string>();
            if (value is IEnumerable<FishingGearModel> gears)
            {
                foreach (var gear in gears)
                {
                    if (gear.LogBookId != null)
                    {
                        if (!gear.Inspected)
                        {
                            invalidGearTypes.Add(gear.Type.Name);
                        }
                    }
                }
            }

            if (invalidGearTypes.Count > 0)
            {
                return new ValidationResult(string.Format(ErrorMessage, string.Concat("\n\t", string.Join(",\n\t", invalidGearTypes), "\n")));
            }
            return ValidationResult.Success;
        }
    }
}
