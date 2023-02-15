using System;
using System.Globalization;

namespace IARA.Mobile.Domain.Models
{
    public sealed class DMSType
    {
        public int Degrees { get; }
        public int Minutes { get; }
        public int Seconds { get; }
        public int Milliseconds { get; }

        private DMSType(int degrees, int minutes, int seconds, int milliseconds)
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

            return new DMSType(int.Parse(values[0]), int.Parse(values[1]), int.Parse(split[0]), split.Length == 1 ? 0 : int.Parse(split[1]));
        }
    }
}
