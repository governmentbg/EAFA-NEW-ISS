using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace IARA.Mobile.Insp.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ListMinLengthAttribute : ValidationAttribute
    {
        public ListMinLengthAttribute(int minLength)
        {
            MinLength = minLength;
        }

        public int MinLength { get; }

        public override bool IsValid(object value)
        {
            if (!(value is ICollection collection))
            {
                return true;
            }

            return collection.Count >= MinLength;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessage, name, MinLength);
        }
    }
}
