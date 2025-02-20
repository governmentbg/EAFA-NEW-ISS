using IARA.Mobile.Insp.Application.Interfaces.Utilities;
using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;
using Xamarin.Forms;

namespace IARA.Mobile.Insp.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class NoOlderThanAttribute : ValidationAttribute, ITLValidatable
    {
        public ValidationResult IsValid(object value, TLValidationContext validationContext)
        {
            if (value is DateTime date)
            {
                ISettings settings = DependencyService.Resolve<ISettings>();
                if (settings.LatestSubmissionDateForInspection == -1)
                {
                    return ValidationResult.Success;
                }
                else if (date.AddHours(settings.LatestSubmissionDateForInspection) > DateTime.Now)
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
