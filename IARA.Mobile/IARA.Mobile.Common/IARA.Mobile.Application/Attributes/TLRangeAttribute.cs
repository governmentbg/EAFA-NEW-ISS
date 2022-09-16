using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace IARA.Mobile.Application.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class TLRangeAttribute : ValidationAttribute
    {
        private readonly bool _isInt;

        public double Minimum { get; }
        public double Maximum { get; }

        public TLRangeAttribute(int minimum, int maximum, bool allowFloatingPoint = false)
        {
            Minimum = minimum;
            Maximum = maximum;
            _isInt = !allowFloatingPoint;
        }

        public TLRangeAttribute(double minimum, double maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
            _isInt = false;
        }

        public override bool IsValid(object value)
        {
            // Automatically pass if value is null or empty. RequiredAttribute should be used to assert a value is not empty.
            string s = value as string;

            return string.IsNullOrEmpty(s)
                || double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out double res)
                    && (_isInt && res % 1 == 0 || !_isInt) // Check if it has a floating point
                    && Maximum >= res && res >= Minimum;
        }

        public override string FormatErrorMessage(string name)
        {
            return string.Format(ErrorMessage, name, Minimum, Maximum);
        }
    }
}
