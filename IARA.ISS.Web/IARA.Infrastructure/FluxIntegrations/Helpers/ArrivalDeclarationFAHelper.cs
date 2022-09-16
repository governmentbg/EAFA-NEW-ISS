using System;
using System.Linq;
using System.Collections.Generic;
using IARA.EntityModels.Entities;
using IARA.Flux.Models;

namespace IARA.Infrastructure.FluxIntegrations.Helpers
{
    internal partial class FishingActivityHelper
    {
        private List<ShipLogBookPage> MapFluxFAReportDeclarationArrival(FishingActivityType fishingActivity)
        {
            List<ShipLogBookPage> relatedPages = MapArrivalData(fishingActivity);
            return relatedPages;
        }

        private List<ShipLogBookPage> UpdateFluxFAReportDeclarationArrival(FishingActivityType fishingActivity)
        {
            List<ShipLogBookPage> relatedPages = MapArrivalData(fishingActivity);
            return relatedPages;
        }

        private List<ShipLogBookPage> CancelFluxFAReportDeclarationArrival(IDType referenceId)
        {
            // Clear arrival data fields from all log books, related to the fishing trip
            List<ShipLogBookPage> relatedPages = ClearArrivalData(referenceId);

            return relatedPages;
        }

        private List<ShipLogBookPage> DeleteFluxFAReportDeclarationArrival(IDType referenceId)
        {
            // Clear arrival data fields from all log books, related to the fishing trip
            List<ShipLogBookPage> relatedPages = ClearArrivalData(referenceId);

            return relatedPages;
        }

        private List<ShipLogBookPage> ClearArrivalData(IDType referenceId)
        {
            FvmsfishingActivityReport referencedReport = GetDocumentByUUID((Guid)referenceId);
            HashSet<int> prevReferencedShipLogBookPageIds = (from fvmsReportShipPage in Db.FvmsfishingActivityReportLogBookPages
                                                             where fvmsReportShipPage.FishingActivityReportId == referencedReport.Id
                                                                   && fvmsReportShipPage.IsActive
                                                             select fvmsReportShipPage.ShipLogBookPageId).ToHashSet();

            List<ShipLogBookPage> relatedPages = (from lbPage in Db.ShipLogBookPages
                                                  where prevReferencedShipLogBookPageIds.Contains(lbPage.Id)
                                                        && lbPage.IsActive
                                                  select lbPage).ToList();

            foreach (var page in relatedPages)
            {
                page.ArrivePortId = null;
                page.FishTripEndDateTime = null;
            }

            return relatedPages;
        }

        /// <summary>
        /// Maps arrival port and Fishing trip end date for a specified fishing trip and all its log book pages
        /// (Used for notification of arrival and then replaces in declaration of arrival if there is a declaration at all)
        /// </summary>
        /// <param name="fishingActivity">Fishing activity of the operation</param>
        private List<ShipLogBookPage> MapArrivalData(FishingActivityType fishingActivity)
        {
            List<ShipLogBookPage> relatedPages = new List<ShipLogBookPage>();
            int? arrivalPortId = GetPortId(fishingActivity.RelatedFLUXLocation);
            string fishingTrip = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);

            if (!arrivalPortId.HasValue)
            {
                Logger.LogWarning($"{LOGGER_MSG_TYPE} the arrival port for trip: {fishingTrip} is not found in our DB.");
            }

            DateTime arrivalDateTime = (DateTime)fishingActivity.OccurrenceDateTime;

            HashSet<int> shipLogBookPageIds = GetShipLogBookPageIdsForTrip(fishingTrip);
            List<ShipLogBookPage> logBookPages = (from shipLogBookPage in Db.ShipLogBookPages
                                                  where shipLogBookPageIds.Contains(shipLogBookPage.Id)
                                                  select shipLogBookPage).ToList();

            foreach (ShipLogBookPage logBookPage in logBookPages)
            {
                relatedPages.Add(logBookPage);
                logBookPage.ArrivePortId = arrivalPortId;
                logBookPage.FishTripEndDateTime = arrivalDateTime;
            }

            return relatedPages;
        }
    }
}
