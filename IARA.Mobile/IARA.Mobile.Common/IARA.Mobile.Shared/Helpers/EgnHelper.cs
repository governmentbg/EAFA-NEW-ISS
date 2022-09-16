using System;
using System.Globalization;
using System.Linq;

namespace IARA.Mobile.Shared.Helpers
{
    public static class EgnHelper
    {
        public static bool IsEgnValid(string egn)
        {
            if (string.IsNullOrEmpty(egn))
            {
                return true;
            }

            if (egn.Length != 10 || egn.Any(f => !char.IsDigit(f)))
            {
                return false;
            }

            int[] weights = new[] { 2, 4, 8, 5, 10, 9, 7, 3, 6 };

            int year = int.Parse(egn.Substring(0, 2));
            int month = int.Parse(egn.Substring(2, 2));
            int day = int.Parse(egn.Substring(4, 2));

            if (month > 40)
            {
                if (!IsDateValid(year + 2000, month - 40, day))
                {
                    return false;
                }
            }
            else if (month > 20)
            {
                if (!IsDateValid(year + 1800, month - 20, day))
                {
                    return false;
                }
            }
            else
            {
                if (!IsDateValid(year + 1900, month, day))
                {
                    return false;
                }
            }

            int sum = 0;

            for (int i = 0; i < 9; i++)
            {
                sum += int.Parse(egn[i].ToString()) * weights[i];
            }

            int validChecksum = sum % 11;
            if (validChecksum == 10)
            {
                validChecksum = 0;
            }

            int checkSum = int.Parse(egn.Substring(9, 1));
            return checkSum == validChecksum;
        }

        private static bool IsDateValid(int year, int month, int day)
        {
            return DateTime.TryParseExact($"{year:D4}-{month:D2}-{day:D2}", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out _);
        }
    }
}
