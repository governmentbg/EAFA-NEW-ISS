using System;
using System.Globalization;

namespace IARA.Mobile.Insp.Helpers
{
    public static class ParseHelper
    {
        public static decimal? ParseDecimal(string str)
        {
            return decimal.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal num)
                ? new decimal?(num)
                : null;
        }

        public static int? ParseInteger(string str)
        {
            return int.TryParse(str, out int num)
                ? new int?(num)
                : null;
        }

        public static short? ParseShort(string str)
        {
            return short.TryParse(str, out short num)
                ? new short?(num)
                : null;
        }

        public static T? ParseEnum<T>(string str)
            where T : struct, Enum
        {
            return Enum.TryParse(str, out T @enum)
                ? new T?(@enum)
                : null;
        }
    }
}
