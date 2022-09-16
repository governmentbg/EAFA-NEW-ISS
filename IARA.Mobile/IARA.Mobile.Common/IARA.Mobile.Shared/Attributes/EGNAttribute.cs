using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Shared.Helpers;
using IARA.Mobile.Shared.ViewModels.Models;
using System;
using System.ComponentModel.DataAnnotations;
using TechnoLogica.Xamarin.ViewModels.Interfaces;
using TechnoLogica.Xamarin.ViewModels.Models;

namespace IARA.Mobile.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class EGNAttribute : ValidationAttribute, ITLValidatable
    {
        public EGNAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }

        public string PropertyName { get; }

        public ValidationResult IsValid(object value, TLValidationContext validationContext)
        {
            string str = value as string;

            if (string.IsNullOrEmpty(str))
            {
                return ValidationResult.Success;
            }
            if (validationContext.Items[PropertyName] is EgnLncValidState validState)
            {
                if (validState.IdentifierType != IdentifierTypeEnum.EGN || EgnHelper.IsEgnValid(str))
                {
                    return ValidationResult.Success;
                }
            }
            else if (EgnHelper.IsEgnValid(str))
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
