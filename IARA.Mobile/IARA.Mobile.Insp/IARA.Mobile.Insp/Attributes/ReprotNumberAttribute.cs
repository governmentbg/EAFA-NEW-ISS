using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;

namespace IARA.Mobile.Insp.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class ReportNumberAttribute : ValidationAttribute
    {
        public ReportNumberAttribute(string otherValidStatePropertyName)
        {
            OtherValidStatePropertyName = otherValidStatePropertyName;
        }

        public string OtherValidStatePropertyName { get; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (validationContext.Items.TryGetValue(OtherValidStatePropertyName, out object otherValue))
            {
                if((bool)otherValue)
                {
                    Regex regex = new Regex("^[0-9]{3}-[0-9]{3}-[0-9]{3}(#[0-9]{1,3})?$");
                    if(regex.IsMatch(value.ToString()))
                    {
                        return ValidationResult.Success;
                    }
                }
                else
                {
                    Regex regex = new Regex("^[0-9]{3}$");
                    if (regex.IsMatch(value.ToString()))
                    {
                        return ValidationResult.Success;
                    }
                }
            }
            return new ValidationResult(ErrorMessage);
        }
    }
}
