using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace IARA.Mobile.Pub.Application.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ListLengthAttribute : ValidationAttribute
    {
        public int MinLength { get; }
        public int MaxLength { get; set; } = int.MaxValue;

        public ListLengthAttribute(int minLength)
        {
            MinLength = minLength;
        }

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
            return string.Format(ErrorMessageString, name, MinLength);
        }
    }
}

