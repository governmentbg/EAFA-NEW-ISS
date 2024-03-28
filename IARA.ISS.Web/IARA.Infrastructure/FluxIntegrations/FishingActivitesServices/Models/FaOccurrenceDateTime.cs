namespace IARA.Infrastructure.FluxIntegrations.FishingActivitesServices.Models
{
    public class FaOccurrenceDateTime
    {
        public DateTime DateTime
        {
            get
            {
                return OccurrenceDateTime2 ?? OccurrenceDateTime;
            }
        }

        public DateTime OccurrenceDateTime { get; set; }

        public DateTime? OccurrenceDateTime2 { get; set; }
    }
}
