using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace IARA.Common.CustomValidators
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class RequiredIfAttribute : ValidationAttribute
    {
        public RequiredIfAttribute(string propertyName, string errorMessageResourceName, Type errorMessageResourceType, params object[] desiredValues)
        {
            PropertyName = propertyName;
            DesiredValues = desiredValues;
            ErrorMessageResourceName = errorMessageResourceName;
            ErrorMessageResourceType = errorMessageResourceType;
        }

        private string PropertyName { get; set; }
        private object[] DesiredValues { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            object instance = context.ObjectInstance;

            Type type = instance.GetType();
            object otherPropertyValue = type.GetProperty(PropertyName).GetValue(instance, null);

            if (DesiredValues.Contains(otherPropertyValue))
            {
                if (value == null)
                {
                    return GetFailedValidationResult(context.DisplayName);
                }

                if (value.GetType() == typeof(string))
                {
                    if (string.IsNullOrEmpty(value.ToString()))
                    {
                        return GetFailedValidationResult(context.DisplayName);
                    }
                }
                else
                {
                    if (value == Activator.CreateInstance(value.GetType()))
                    {
                        return GetFailedValidationResult(context.DisplayName);
                    }
                }

                if (IsEnumerableType(value.GetType()) && !Any(value))
                {
                    return GetFailedValidationResult(context.DisplayName);
                }
            }

            return ValidationResult.Success;
        }

        private bool IsEnumerableType(Type type)
        {
            return (type.GetInterface(nameof(ICollection)) != null);
        }

        private bool Any(object value)
        {            
            IEnumerator enumerator = (value as IEnumerable).GetEnumerator();
            bool hasAny = enumerator.MoveNext();

            if (enumerator is IDisposable)
            {
                (enumerator as IDisposable).Dispose();
            }

            return hasAny;
        }

        private ValidationResult GetFailedValidationResult(string displayName)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                return new ValidationResult(string.Format(CultureInfo.CurrentCulture, ErrorMessage, displayName));
            }
            else
            {
                PropertyInfo property = ErrorMessageResourceType.GetProperty(ErrorMessageResourceName, BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic);
                string errorMessageTemplate = (string)property.GetValue(null, null);
                return new ValidationResult(string.Format(CultureInfo.CurrentCulture, errorMessageTemplate, displayName));
            }
        }
    }
}
