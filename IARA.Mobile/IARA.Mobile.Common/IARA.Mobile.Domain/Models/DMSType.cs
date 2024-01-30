using System;
using System.Globalization;

namespace IARA.Mobile.Domain.Models
{
    public sealed class DMSType
    {
        public long Degrees { get; }
        public long Minutes { get; }
        public long Seconds { get; }
        public long Milliseconds { get; }

        private DMSType(long degrees, long minutes, long seconds, long milliseconds)
        {
            Degrees = degrees;
            Minutes = minutes;
            Seconds = seconds;
            Milliseconds = milliseconds;
        }

        public double ToDecimal()
        {
            double seconds = double.Parse($"{Seconds}.{Milliseconds}", CultureInfo.InvariantCulture);

            return Degrees + (Minutes / 60d) + (seconds / 3600d);
        }

        public override string ToString()
        {
            return $"{Degrees} {Minutes} {Seconds}.{Milliseconds}";
        }

        public static DMSType FromDouble(double decimalDegrees)
        {
            int degrees = (int)Math.Floor(decimalDegrees);
            int minutes = (int)Math.Floor((decimalDegrees - degrees) * 60d);
            double seconds = (double)((decimalDegrees - degrees - (minutes / 60d)) * 3600d);
            int finalSeconds = (int)Math.Floor(seconds);

            string strSeconds = seconds.ToString(CultureInfo.InvariantCulture);

            int milliseconds = int.TryParse(
                strSeconds.Substring(strSeconds.IndexOf('.') + 1),
                out int milis
            ) ? milis : 0;

            return new DMSType(degrees, minutes, finalSeconds, milliseconds);
        }

        public static double ConvertDegreeAngleToDouble(double degrees, double minutes, double seconds)
        {
            return degrees + (minutes / 60) + (seconds / 3600);
        }

        public static DMSType Parse(string degrees)
        {
            if (string.IsNullOrEmpty(degrees))
            {
                return null;
            }

            string[] values = degrees.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string[] split = values[2].Replace(',', '.').Split('.');

            return new DMSType(long.Parse(values[0]), long.Parse(values[1]), long.Parse(split[0]), split.Length == 1 ? 0 : long.Parse(split[1]));
        }
    }
}
