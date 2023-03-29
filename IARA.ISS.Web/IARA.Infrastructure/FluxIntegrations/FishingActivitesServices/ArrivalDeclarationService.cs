using IARA.Flux.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TL.Logging.Abstractions.Interfaces;

namespace IARA.Infrastructure.FluxIntegrations.FishingActivitesServices
{
    internal class ArrivalDeclarationService : BaseFishingActivityService
    {
        public ArrivalDeclarationService(IARADbContext dbContext, IExtendedLogger logger)
            : base(dbContext, logger, "ArrivalDeclarationService.cs")
        { }

        public List<ShipLogBookPage> MapOriginal(FishingActivityType fishingActivity)
        {
            List<ShipLogBookPage> relatedPages = MapArrivalData(fishingActivity);
            return relatedPages;
        }

        public List<ShipLogBookPage> MapUpdate(FishingActivityType fishingActivity)
        {
            List<ShipLogBookPage> relatedPages = MapArrivalData(fishingActivity);
            return relatedPages;
        }

        public List<ShipLogBookPage> MapCancel(IDType referenceId)
        {
            // Clear arrival data fields from all log books, related to the fishing trip
            List<ShipLogBookPage> relatedPages = ClearArrivalData(referenceId);

            return relatedPages;
        }

        public List<ShipLogBookPage> MapDelete(IDType referenceId)
        {
            // Clear arrival data fields from all log books, related to the fishing trip
            List<ShipLogBookPage> relatedPages = ClearArrivalData(referenceId);

            return relatedPages;
        }

        /// <summary>
        /// Maps arrival port and Fishing trip end date for a specified fishing trip and all its log book pages
        /// (Used for notification of arrival and then replaces in declaration of arrival if there is a declaration at all)
        /// </summary>
        /// <param name="fishingActivity">Fishing activity of the operation</param>
        public List<ShipLogBookPage> MapArrivalData(FishingActivityType fishingActivity)
        {
            List<ShipLogBookPage> relatedPages = new List<ShipLogBookPage>();

            int arrivalPortId = GetPortId(fishingActivity.RelatedFLUXLocation);
            string fishingTrip = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);

            DateTime arrivalDateTime = (DateTime)fishingActivity.OccurrenceDateTime;

            HashSet<int> shipLogBookPageIds = GetShipLogBookPageIdsForTrip(fishingTrip);
            List<ShipLogBookPage> logBookPages = (from shipLogBookPage in Db.ShipLogBookPages
                                                  where shipLogBookPageIds.Contains(shipLogBookPage.Id)
                                                  select shipLogBookPage).ToList();

            foreach (ShipLogBookPage logBookPage in logBookPages)
            {
                logBookPage.ArrivePortId = arrivalPortId;
                logBookPage.FishTripEndDateTime = arrivalDateTime;

                relatedPages.Add(logBookPage);
            }

            return relatedPages;
        }

        public List<ShipLogBookPage> ClearArrivalData(IDType referenceId)
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
    }
}
