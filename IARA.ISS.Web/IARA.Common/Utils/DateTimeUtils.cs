using System;
using System.Globalization;
using IARA.Common.Constants;

namespace IARA.Common.Utils
{
    public static class DateTimeUtils
    {
        public static DateTime? TryParseDate(string text)
        {
            DateTimeStyles dateTimeStyle = DateTimeStyles.AssumeLocal
                                                                     | DateTimeStyles.AllowLeadingWhite
                                                                     | DateTimeStyles.AllowTrailingWhite
                                                                     | DateTimeStyles.AllowInnerWhite;

            if (DateTime.TryParseExact(text, DateTimeFormats.DISPLAY_DATETIME_FORMAT, CultureInfo.InvariantCulture, dateTimeStyle, out DateTime result))
            {
                return result;
            }
            else if (DateTime.TryParseExact(text, DateTimeFormats.DISPLAY_DATE_FORMAT, CultureInfo.InvariantCulture, dateTimeStyle, out result))
            {
                return result;
            }
            else if (DateTime.TryParseExact(text, DateTimeFormats.SHORT_DATE_FORMAT, CultureInfo.InvariantCulture, dateTimeStyle, out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }
    }
}
