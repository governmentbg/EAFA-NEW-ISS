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

        public static bool IsEIKValid(string eik)
        {
            if (eik.Length != 9 || !long.TryParse(eik, out _))
            {
                return false;
            }

            int[] digits = new int[9];
            for (int i = 0; i < eik.Length; i++)
            {
                digits[i] = int.Parse(eik[i].ToString());
            }

            int sum1 = digits[0] * 1 + digits[1] * 2 + digits[2] * 3 + digits[3] * 4 + digits[4] * 5 + digits[5] * 6 + digits[6] * 7 + digits[7] * 8;
            int remainder1 = sum1 % 11;

            int controlDigit;
            if (remainder1 < 10)
            {
                controlDigit = remainder1;
            }
            else
            {
                int sum2 = digits[0] * 3 + digits[1] * 4 + digits[2] * 5 + digits[3] * 6 + digits[4] * 7 + digits[5] * 8 + digits[6] * 9 + digits[7] * 10;
                int remainder2 = sum2 % 11;

                controlDigit = remainder2 < 10 ? remainder2 : 0;
            }

            return controlDigit == digits[8];
        }

        private static bool IsDateValid(int year, int month, int day)
        {
            return DateTime.TryParseExact($"{year:D4}-{month:D2}-{day:D2}", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out _);
        }
    }
}
