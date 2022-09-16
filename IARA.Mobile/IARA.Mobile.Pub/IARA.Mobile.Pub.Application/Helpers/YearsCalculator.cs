using System;

namespace IARA.Mobile.Pub.Application.Helpers
{
    public static class YearsCalculator
    {
        public static int YearsDifferenceFromNow(DateTime date)
        {
            DateTime now = DateTime.Now;
            return now.Year - date.Year - 1 +
                (((now.Month > date.Month) ||
                ((now.Month == date.Month) && (now.Day >= date.Day))) ? 1 : 0);
        }
    }
}
