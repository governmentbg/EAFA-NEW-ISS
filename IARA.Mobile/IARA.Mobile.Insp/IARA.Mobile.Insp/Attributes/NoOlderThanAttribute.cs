using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Insp.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class NoOlderThanAttribute : ValidationAttribute, ITLValidatable
    {
        public NoOlderThanAttribute(int hours)
        {
            Hours = hours;
        }
        public int Hours { get; set; }
        public ValidationResult IsValid(object value, TLValidationContext validationContext)
        {
            if (value is DateTime date)
            {
                if (date.AddHours(Hours) > DateTime.Now)
                {
                    return ValidationResult.Success;
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
