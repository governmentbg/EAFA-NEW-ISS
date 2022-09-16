using System;

namespace IARA.RegixAbstractions.Models
{
    public class RegixConclusionContext
    {
        private const int START_PERIOD_SECONDS = 5;
        private const int END_PERIOD_SECONDS = 120;

        public int ApplicationId { get; set; }
        public int ApplicationHistoryId { get; set; }
        private TimeSpan delay = TimeSpan.Zero;

        public TimeSpan CalculateDelay()
        {
            if (delay == TimeSpan.Zero)
            {
                delay = TimeSpan.FromSeconds(START_PERIOD_SECONDS);
            }
            else if (delay < TimeSpan.FromSeconds(END_PERIOD_SECONDS))
            {
                delay *= 2;
            }
            else
            {
                delay = TimeSpan.Zero;
            }

            return delay;
        }

        public override int GetHashCode()
        {
            return ApplicationId.GetHashCode() ^ ApplicationHistoryId.GetHashCode();
        }
    }
}
