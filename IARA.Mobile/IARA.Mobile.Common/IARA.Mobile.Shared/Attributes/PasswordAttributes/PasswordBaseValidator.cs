using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace IARA.Mobile.Shared.Attributes.PasswordAttributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PasswordBaseValidator : ValidationAttribute
    {
        public string Pattern { get; }
        public PasswordBaseValidator(string pattern)
        {
            Pattern = pattern;
        }

        public override bool IsValid(object value)
        {
            string valueAsString = value as string;

            if (string.IsNullOrEmpty(valueAsString))
            {
                return true;
            }

            Regex regex = new Regex(Pattern);
            return regex.IsMatch(valueAsString);
        }
    }
}
