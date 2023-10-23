namespace IARA.Infrastructure.FluxIntegrations.FishingActivitesServices.Models
{
    public class ReportBasicData
    {
        public int ActivityReportId { get; set; }

        public Guid FluxFvmsRequestUUId { get; set; }

        public Guid? ReferencedFluxFvmsRequestUUId { get; set; }

        public string FluxFvmsRequestContent { get; set; }

        public int GPPurposeCode { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
