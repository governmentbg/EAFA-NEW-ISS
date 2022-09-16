using System.Collections.Generic;
using IARA.EntityModels.Entities;
using IARA.Flux.Models;

namespace IARA.Infrastructure.FluxIntegrations.Helpers
{
    internal partial class FishingActivityHelper
    {
        private List<ShipLogBookPage> MapFluxFAReportNotificationArrival(FishingActivityType fishingActivity)
        {
            List<ShipLogBookPage> relatedPages = MapArrivalData(fishingActivity);
            return relatedPages;
        }

        private List<ShipLogBookPage> CancelFluxFAReportNotificationArrival(IDType referenceId)
        {
            // Clear arrival data fields from all log books, related to the fishing trip
            List<ShipLogBookPage> relatedPages = ClearArrivalData(referenceId);

            return relatedPages;
        }

        private List<ShipLogBookPage> DeleteFluxFAReportNotificationArrival(IDType referenceId)
        { 
            // Delete arrival data fields from all log books, related to the fishing trip
            List<ShipLogBookPage> relatedPages = ClearArrivalData(referenceId);

            return relatedPages;
        }
    }
}
