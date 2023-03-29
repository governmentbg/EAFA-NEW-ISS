using IARA.Flux.Models;
using IARA.FluxModels.Enums;
using IARA.Infrastructure.FluxIntegrations.FishingActivitesServices.Models;
using IARA.Infrastructure.FluxIntegrations.Helpers;
using Microsoft.Extensions.Logging;
using TL.Logging.Abstractions.Interfaces;

namespace IARA.Infrastructure.FluxIntegrations.FishingActivitesServices
{
    internal class LandingService : BaseFishingActivityService
    {
        private readonly UnloadingHelperService _unloadingHelperService;

        public LandingService(IARADbContext dbContext, IExtendedLogger logger) 
            : base(dbContext, logger, nameof(LandingService))
        {
            _unloadingHelperService = new UnloadingHelperService(dbContext, logger);
        }

        public List<ShipLogBookPage> MapOriginal(FishingActivityType fishingActivity, DateTime documentOccurrenceDateTime)
        {
            List<ShipLogBookPage> relatedPages = new List<ShipLogBookPage>();

            string fishingTrip = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
            HashSet<int> shipLogBookPageIds = GetShipLogBookPageIdsForTrip(fishingTrip);
            List<FACatchType> unloadedFaCatches = fishingActivity.SpecifiedFACatch.Where(x => x.TypeCode.Value == nameof(FaCatchTypes.UNLOADED)).ToList();

            if (unloadedFaCatches.Count == 0)
            {
                LogWarning($"Zero unloaded catches for fishing trip identifier: {fishingTrip}", nameof(MapOriginal));

                AddEmptyOriginDeclaration(fishingTrip);
            }
            else
            {
                _unloadingHelperService.MapFaCatchToOriginDeclaration(fishingActivity, unloadedFaCatches, UnloadingTypesEnum.UNL, documentOccurrenceDateTime);
            }

            UpdateArrivalDataIfEmpty(fishingActivity, documentOccurrenceDateTime); // this is done in order to be able to Submit the pages afterwards in case no notification/declaration of arrival is present

            relatedPages = SubmitShipLogBookPages(shipLogBookPageIds);

            return relatedPages;
        }

        public List<ShipLogBookPage> MapUpdate(FishingActivityType fishingActivity,
                                                                           DateTime documentOccurrenceDateTime,
                                                                           IDType referenceId)
        {
            // Delete from DB unloaded catches from the related document, which is being updated

            FvmsfishingActivityReport referencedReport = GetDocumentByUUID((Guid)referenceId);
            FLUXFAReportMessageType referencedMessage = CommonUtils.Deserialize<FLUXFAReportMessageType>(referencedReport.ResponseMessage);
            FishingActivityType prevFishingActivity = referencedMessage.FAReportDocument.SelectMany(x => x.SpecifiedFishingActivity)
                                                                                        .Where(y => y.TypeCode.Value == nameof(FaTypes.LANDING))
                                                                                        .First();

            DeletePreviouslyUnloadedCatches(prevFishingActivity, referencedMessage.FLUXReportDocument.CreationDateTime.Item);

            // Map and add to DB new unloaded catches

            List<FACatchType> newUnloadedFaCatches = fishingActivity.SpecifiedFACatch
                                                                    .Where(x => x.TypeCode.Value == nameof(FaCatchTypes.UNLOADED))
                                                                    .ToList();

            OriginDeclarationsData originDeclartionsData = _unloadingHelperService.MapFaCatchToOriginDeclaration(fishingActivity, newUnloadedFaCatches, UnloadingTypesEnum.UNL, documentOccurrenceDateTime);

            string fishingTrip = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
            HashSet<int> relatedShipLogBookPageIds = originDeclartionsData.LogBookPageOriginDeclarationFishes.Keys.ToHashSet();

            List<ShipLogBookPage> relatedPages = (from logBookPage in Db.ShipLogBookPages
                                                  where relatedShipLogBookPageIds.Contains(logBookPage.Id)
                                                  select logBookPage).ToList();

            return relatedPages;
        }

        public List<ShipLogBookPage> MapCancel(FishingActivityType fishingActivity, DateTime fluxReportCreationDate)
        {
            List<ShipLogBookPage> relatedPages = new List<ShipLogBookPage>();

            DeletePreviouslyUnloadedCatches(fishingActivity, fluxReportCreationDate);

            string tripIdentifier = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
            HashSet<int> relatedPageIds = GetShipLogBookPageIdsForTrip(tripIdentifier);

            relatedPages = (from page in Db.ShipLogBookPages
                            where relatedPageIds.Contains(page.Id)
                                  && page.IsActive
                            select page).ToList();

            return relatedPages;
        }

        public List<ShipLogBookPage> MapDelete(FishingActivityType fishingActivity, DateTime fluxReportCreationDate)
        {
            List<ShipLogBookPage> relatedPages = new List<ShipLogBookPage>();

            DeletePreviouslyUnloadedCatches(fishingActivity, fluxReportCreationDate);

            string tripIdentifier = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
            HashSet<int> relatedPageIds = GetShipLogBookPageIdsForTrip(tripIdentifier);

            relatedPages = (from page in Db.ShipLogBookPages
                            where relatedPageIds.Contains(page.Id)
                                  && page.IsActive
                            select page).ToList();

            return relatedPages;
        }

        // Helper methods

        private void AddEmptyOriginDeclaration(string tripIdentifier)
        {
            HashSet<int> shipLogBookPageIds = GetShipLogBookPageIdsForTrip(tripIdentifier);
            List<OriginDeclaration> originDeclarations = new List<OriginDeclaration>();

            foreach (int logBookPageId in shipLogBookPageIds)
            {
                originDeclarations.Add(new OriginDeclaration
                {
                    LogBookPageId = logBookPageId
                });
            }

            Db.OriginDeclarations.AddRange(originDeclarations);
        }

        private void UpdateArrivalDataIfEmpty(FishingActivityType fishingActivity, DateTime documentOccurrenceDateTime)
        {
            int? portId = GetPortId(fishingActivity.RelatedFLUXLocation);
            string fishingTrip = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);

            if (!portId.HasValue || portId == -1)
            {
                LogWarning($"the port of LADNING for trip: {fishingTrip} is not found in our DB.", nameof(UpdateArrivalDataIfEmpty));
            }

            HashSet<int> shipLogBookPageIds = GetShipLogBookPageIdsForTrip(fishingTrip);
            List<ShipLogBookPage> logBookPages = (from shipLogBookPage in Db.ShipLogBookPages
                                                  where shipLogBookPageIds.Contains(shipLogBookPage.Id)
                                                        && (!shipLogBookPage.ArrivePortId.HasValue
                                                            || !shipLogBookPage.FishTripEndDateTime.HasValue)
                                                  select shipLogBookPage).ToList();

            foreach (ShipLogBookPage logBookPage in logBookPages)
            {
                if (!logBookPage.ArrivePortId.HasValue)
                {
                    logBookPage.ArrivePortId = portId;
                }

                if (!logBookPage.FishTripEndDateTime.HasValue)
                {
                    logBookPage.FishTripEndDateTime = documentOccurrenceDateTime;
                }
            }
        }

        private List<ShipLogBookPage> SubmitShipLogBookPages(HashSet<int> shipLogBookPageIds)
        {
            List<ShipLogBookPage> shipLogBookPages = (from logBookPage in Db.ShipLogBookPages
                                                      where shipLogBookPageIds.Contains(logBookPage.Id)
                                                      select logBookPage).ToList();

            foreach (ShipLogBookPage shipLogBookPage in shipLogBookPages)
            {
                shipLogBookPage.Status = nameof(LogBookPageStatusesEnum.Submitted);
            }

            return shipLogBookPages;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="referenceId"></param>
        /// <returns>Returns the related landing fishing activity type object</returns>
        private void DeletePreviouslyUnloadedCatches(FishingActivityType fishingActivity, DateTime fluxReportCreationDate)
        {
            // Get previous Langing Fishing Activity unloaded catches

            List<FACatchType> previouslyUnloadedFaCatches = fishingActivity.SpecifiedFACatch
                                                                           .Where(x => x.TypeCode.Value == nameof(FaCatchTypes.UNLOADED))
                                                                           .ToList();

            string prevTripIdentifier = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
            HashSet<int> prevRelatedLogBookPageIds = GetShipLogBookPageIdsForTrip(prevTripIdentifier);

            OriginDeclarationsData previousOriginDeclartionsData = _unloadingHelperService.GetOriginDeclarationsData(fishingActivity,
                                                                                            previouslyUnloadedFaCatches,
                                                                                            UnloadingTypesEnum.UNL,
                                                                                            fluxReportCreationDate);


            List<OriginDeclarationFish> dbOriginDeclarationFishes = (from originDeclarationFish in Db.OriginDeclarationFish
                                                                     join originDeclaration in Db.OriginDeclarations on originDeclarationFish.OriginDeclarationId equals originDeclaration.Id
                                                                     where prevRelatedLogBookPageIds.Contains(originDeclaration.LogBookPageId)
                                                                     select originDeclarationFish).ToList();

            Dictionary<int, decimal> usedCatchRecordFishUnloadedQuantities = new Dictionary<int, decimal>(); // used unloaded quantities, which must be removed

            // Delete previous catches from the origin declaration(s)

            foreach (var pageOriginDeclarationFish in previousOriginDeclartionsData.LogBookPageOriginDeclarationFishes)
            {
                List<OriginDeclarationFish> originDeclarationFishes = pageOriginDeclarationFish.Value.ToList();

                OriginDeclaration previousOriginDeclaration = (from originDeclaration in Db.OriginDeclarations
                                                               where originDeclaration.LogBookPageId == pageOriginDeclarationFish.Key
                                                                     && originDeclaration.IsActive
                                                               select originDeclaration).FirstOrDefault();

                foreach (var originDeclarationFish in originDeclarationFishes) // Delete each previous origin declaration fish
                {
                    var dbOriginDeclarationFish = dbOriginDeclarationFishes.Where(x => x.UnloadDateTime == originDeclarationFish.UnloadDateTime
                                                                                      && x.UnloadPortId == originDeclarationFish.UnloadPortId
                                                                                      && x.Quantity == originDeclarationFish.Quantity
                                                                                      && x.CatchFishFreshnessId == originDeclarationFish.CatchFishFreshnessId
                                                                                      && x.CatchFishPresentationId == originDeclarationFish.CatchFishPresentationId
                                                                                      && x.IsProcessedOnBoard == originDeclarationFish.IsProcessedOnBoard
                                                                                      && x.FishId == originDeclarationFish.FishId
                                                                                      && x.CatchRecordFishId == originDeclarationFish.CatchRecordFishId)
                                                                           .FirstOrDefault();
                    if (dbOriginDeclarationFish != null)
                    {
                        if (dbOriginDeclarationFish.CatchRecordFishId.HasValue)
                        {
                            if (usedCatchRecordFishUnloadedQuantities.ContainsKey(dbOriginDeclarationFish.CatchRecordFishId.Value))
                            {
                                usedCatchRecordFishUnloadedQuantities[dbOriginDeclarationFish.CatchRecordFishId.Value] += dbOriginDeclarationFish.Quantity;
                            }
                            else
                            {
                                usedCatchRecordFishUnloadedQuantities.Add(dbOriginDeclarationFish.CatchRecordFishId.Value, dbOriginDeclarationFish.Quantity);
                            }
                        }

                        dbOriginDeclarationFish.IsActive = false;
                    }
                }

                // Delete previous origin declaration

                if (previousOriginDeclaration != null)
                {
                    previousOriginDeclaration.IsActive = false;
                }

                // Update CatchRecordFish unloaded quantities

                List<int> usedCatchRecordFishIds = usedCatchRecordFishUnloadedQuantities.Keys.ToList();

                List<CatchRecordFish> dbUsedCatchRecordFishes = (from catchRecordFish in Db.CatchRecordFish
                                                                 where usedCatchRecordFishIds.Contains(catchRecordFish.Id)
                                                                 select catchRecordFish).ToList();

                foreach (CatchRecordFish dbUsedCatchRecordFish in dbUsedCatchRecordFishes)
                {
                    if (dbUsedCatchRecordFish.UnloadedQuantity > 0) // safety check - not to remove any more quantity, if the unloaded quantity is already zero
                    {
                        dbUsedCatchRecordFish.UnloadedQuantity -= usedCatchRecordFishUnloadedQuantities[dbUsedCatchRecordFish.Id];

                        if (dbUsedCatchRecordFish.UnloadedQuantity < 0) // the unloaded quantity was more than the caught
                        {
                            dbUsedCatchRecordFish.UnloadedQuantity = 0; // the unloaded should be just zero, no need to be negative
                        }
                    }
                }
            }
        }
    }
}
