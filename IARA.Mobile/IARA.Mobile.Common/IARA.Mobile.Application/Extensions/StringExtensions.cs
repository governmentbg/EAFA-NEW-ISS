using System;
using System.Globalization;

namespace IARA.Mobile.Application.Extensions
{
    public static class StringExtensions
    {
        public static string CapitalizeFirstLetter(this string str)
        {
            return string.IsNullOrEmpty(str)
                ? str
                : char.ToUpper(str[0]) + str.Substring(1);
        }

        public static string ToApiDateTime(this DateTime date)
        {
            return date.ToString(CommonConstants.DateTimeFormat, CultureInfo.InvariantCulture);
        }

        public static string ToApiDate(this DateTime date)
        {
            return date.ToString(CommonConstants.DateFormat, CultureInfo.InvariantCulture);
        }
    }
}
