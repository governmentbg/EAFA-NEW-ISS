using System;
using System.Globalization;

namespace IARA.Common
{
    public struct DMSType
    {
        private DMSType(int degrees, int minutes, float seconds)
        {
            this.Degrees = degrees;
            this.Minutes = minutes;
            this.Seconds = seconds;
        }

        public DMSType(double decimalDegrees)
        {
            Degrees = (int)Math.Floor(decimalDegrees);
            Minutes = (int)Math.Floor((decimalDegrees - (double)Degrees) * 60f);
            Seconds = (float)((decimalDegrees - (double)Degrees - ((double)Minutes / 60)) * 3600f);
        }

        public int Degrees { get; }
        public int Minutes { get; }
        public float Seconds { get; }

        public override string ToString()
        {
            return $"{Degrees} {Minutes} {Seconds}";
        }

        /// <summary>
        /// Converts DMS(Degrees Minuts Seconds) to DD(Decimal Degrees)
        /// dd = d + m/60 + s/3600
        /// </summary>
        /// <returns>Decimal Degrees</returns>
        public double ToDecimal()
        {
            return ((double)Degrees) + ((double)Minutes) / 60f + ((double)Seconds) / 3600f;
        }

        public static DMSType Parse(string degrees)
        {
            string[] values = degrees.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            values[2] = values[2].Replace(',', '.');

            return new DMSType(int.Parse(values[0]), int.Parse(values[1]), float.Parse(values[2], CultureInfo.InvariantCulture));
        }
    }
}
