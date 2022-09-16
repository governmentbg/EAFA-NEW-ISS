using System;
using System.ComponentModel.DataAnnotations;

namespace IARA.Mobile.Shared.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class YearValidAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (int.TryParse(value as string, out int year))
            {
                try
                {
                    new DateTime(year, 1, 1);
                    return true;
                }
                catch
                {
                }
            }

            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return ErrorMessage;
        }
    }
}
