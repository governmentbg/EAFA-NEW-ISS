using IARA.FluxModels.Enums;

namespace IARA.Infrastructure.FluxIntegrations.FishingActivitesServices.Models
{
    public class FluxFaMessageData
    {
        public Guid FluxMessageId { get; set; }

        public Guid? FluxMessageReferenceId { get; set; }

        public FaReportTypes FaReportType { get; set; }

        public FaTypes FaType { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
