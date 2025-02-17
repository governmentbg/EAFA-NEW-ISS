using IARA.Mobile.Insp.FlyoutPages.Inspections.Dialogs.FishingGearDialog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class NoRepeatingPingerNumbersAttribute : ValidationAttribute, ITLValidatable
    {
        public NoRepeatingPingerNumbersAttribute(string otherPropertyName)
        {
            OtherPropertyName = otherPropertyName;
        }

        public string OtherPropertyName { get; }

        public ValidationResult IsValid(object value, TLValidationContext validationContext)
        {
            if (validationContext.Items.TryGetValue(OtherPropertyName, out IValidState otherValue))
            {
                if (otherValue is ValidStateBool addedByInspector)
                {
                    if (value is string number && addedByInspector == true)
                    {
                        if (FishingGearDialogViewModel.Instance == null)
                        {
                            return ValidationResult.Success;
                        }
                        List<PingerViewModel> pingers = FishingGearDialogViewModel.Instance.InspectedFishingGear.Pingers.ToList();
                        int count = pingers.Where(x => x.Number.Value == number).Count();
                        if (count == 1)
                        {
                            return ValidationResult.Success;
                        }
                    }
                }
            }

            return new ValidationResult(ErrorMessage);
        }

        protected virtual string FormatErrorMessage(string name, string otherName)
        {
            return string.Format(ErrorMessage, name, otherName);
        }
    }
}
