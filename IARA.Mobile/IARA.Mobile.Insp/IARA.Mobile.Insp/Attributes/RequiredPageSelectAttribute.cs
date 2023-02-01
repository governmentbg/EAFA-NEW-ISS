using System;
using System.ComponentModel.DataAnnotations;
using IARA.Mobile.Domain.Enums;
using IARA.Mobile.Insp.Models;

namespace IARA.Mobile.Insp.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class RequiredPageSelectAttribute : ValidationAttribute
    {
        private readonly bool addedByInspector;

        public RequiredPageSelectAttribute(bool addedByInspector)
        {
            this.addedByInspector = addedByInspector;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!validationContext.Items.TryGetValue(nameof(LogBookModel.AddedByInspectorState), out object addedByInspectorState)
                || !validationContext.Items.TryGetValue(nameof(LogBookModel.Corresponds), out object corresponds))
            {
                return ValidationResult.Success;
            }

            if ((bool)addedByInspectorState == addedByInspector && (corresponds as string) != nameof(CheckTypeEnum.X))
            {
                return new RequiredAttribute
                {
                    ErrorMessage = ErrorMessage
                }.GetValidationResult(value, validationContext);
            }

            return ValidationResult.Success;
        }
    }
}
