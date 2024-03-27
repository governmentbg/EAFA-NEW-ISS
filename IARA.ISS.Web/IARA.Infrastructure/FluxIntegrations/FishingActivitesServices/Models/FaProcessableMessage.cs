using IARA.Flux.Models;
using IARA.FluxModels.Enums;

namespace IARA.Infrastructure.FluxIntegrations.FishingActivitesServices.Models
{
    public class FaProcessableMessage
    {
        public FLUXFAReportMessageType FLUXFAReportMessage { get; set; }

        public FAReportDocumentType FaReportDocument { get; set; }

        public FishingActivityType FishingActivity { get; set; }

        public FvmsfishingActivityReport FvmsfishingActivityReport { get; set; }

        public FluxPurposes Purpose { get; set; }

        public DateTime OccurrenceDateTime
        {
            get
            {
                return FvmsfishingActivityReport.OccurenceDateTime2 ?? FvmsfishingActivityReport.OccurenceDateTime;
            }
        }

        public FaProcessableMessage()
        {
            FLUXFAReportMessage = new FLUXFAReportMessageType();
            FaReportDocument = new FAReportDocumentType();
            FishingActivity = new FishingActivityType();
            FvmsfishingActivityReport = new FvmsfishingActivityReport();
            Purpose = FluxPurposes.Original;
        }

        public FaProcessableMessage(FLUXFAReportMessageType fluxFaReportMessage,
                                    FAReportDocumentType faReportDocument, 
                                    FishingActivityType fishingActivity, 
                                    FvmsfishingActivityReport fvmsfishingActivityReport,
                                    FluxPurposes purpose)
        {
            FLUXFAReportMessage = fluxFaReportMessage;
            FaReportDocument = faReportDocument;
            FishingActivity = fishingActivity;
            FvmsfishingActivityReport = fvmsfishingActivityReport;
            Purpose = purpose;
        }
    }
}
