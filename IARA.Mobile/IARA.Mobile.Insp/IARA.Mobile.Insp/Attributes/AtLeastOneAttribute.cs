using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class AtLeastOneAttribute : ValidationAttribute, ITLValidatable
    {
        public ValidationResult IsValid(object value, TLValidationContext validationContext)
        {
            int count = ((ICollection)value).Count;
            if (count > 0)
            {
                return ValidationResult.Success;
            }
            return new ValidationResult(ErrorMessage);
        }

        protected virtual string FormatErrorMessage(string name, string otherName)
        {
            return string.Format(ErrorMessage, name, otherName);
        }
    }
}
