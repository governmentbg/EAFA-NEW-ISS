using System;
using System.Globalization;

namespace IARA.Mobile.Domain.Models
{
    public sealed class DMSType
    {
        public int Degrees { get; }
        public int Minutes { get; }
        public int Seconds { get; }
        public long Milliseconds { get; }

        private DMSType(int degrees, int minutes, int seconds, long milliseconds)
        {
            Degrees = degrees;
            Minutes = minutes;
            Seconds = seconds;
            Milliseconds = milliseconds;
        }

        public static DMSType FromDouble(double decimalDegrees)
        {
            int degrees = (int)Math.Floor(decimalDegrees);
            int minutes = (int)Math.Floor((decimalDegrees - degrees) * 60d);
            double seconds = (double)((decimalDegrees - degrees - (minutes / 60d)) * 3600d);
            int finalSeconds = (int)Math.Floor(seconds);

            string strSeconds = seconds.ToString(CultureInfo.InvariantCulture);

            long milliseconds = long.TryParse(
                strSeconds.Substring(strSeconds.IndexOf('.') + 1),
                out long milis
            ) ? milis : 0L;

            return new DMSType(degrees, minutes, finalSeconds, milliseconds);
        }

        public static double ConvertDegreeAngleToDouble(double degrees, double minutes, double seconds)
        {
            return degrees + (minutes / 60) + (seconds / 3600);
        }
    }
}
