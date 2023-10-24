namespace IARA.Infrastructure.FluxIntegrations.FishingActivitesServices.Models
{
    public class FishingActivityQueryParameters
    {
        /// <summary>
        /// Indicates if the query is for a vessel
        /// </summary>
        public bool ForVessel { get; set; } = false;

        /// <summary>
        /// Indicates if the query is for a specified fishing trip only
        /// </summary>
        public bool ForTrip { get; set; } = false;

        /// <summary>
        /// CFR identifier of vessel
        /// </summary>
        public string VesselCfr { get; set; }

        /// <summary>
        /// IRCS identifier of vessel
        /// </summary>
        public string VesselIrcs { get; set; }

        /// <summary>
        /// ID of the trip
        /// </summary>
        public string TripIdentifier { get; set; }

        /// <summary>
        /// Start date of period for which is the query
        /// </summary>
        public DateTime? PeriodStartDate { get; set; }

        /// <summary>
        /// End date of period for which is the query
        /// </summary>
        public DateTime? PeriodEndDate { get; set; }

        /// <summary>
        /// Whether or not the answear should contain only the latest information or every message (including updates, cancellations etc.)
        /// </summary>
        public bool IsConsolidated { get; set; } = false;
    }
}
