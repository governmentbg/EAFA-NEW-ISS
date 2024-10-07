using IARA.Mobile.Shared.Helpers;
using System;
using System.ComponentModel.DataAnnotations;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class EIKValidationAttribute : ValidationAttribute, ITLValidatable
    {
        public ValidationResult IsValid(object value, TLValidationContext validationContext)
        {
            string str = value as string;

            if (string.IsNullOrEmpty(str))
            {
                return ValidationResult.Success;
            }
            else if (EgnHelper.IsEIKValid(str))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(FormatErrorMessage(null));
        }

        public override string FormatErrorMessage(string name)
        {
            return ErrorMessage;
        }
    }
}
