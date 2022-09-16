using System;
using System.Linq;
using System.Collections.Generic;
using IARA.EntityModels.Entities;
using IARA.Flux.Models;
using IARA.FluxModels.Enums;
using IARA.Common.Enums;
using TL.SysToSysSecCom;

namespace IARA.Infrastructure.FluxIntegrations.Helpers
{
    internal partial class FishingActivityHelper
    {
        private List<ShipLogBookPage> MapFluxFAReportDeclarationLanding(FishingActivityType fishingActivity, DateTime documentOccurrenceDateTime)
        {
            List<ShipLogBookPage> relatedPages = new List<ShipLogBookPage>();

            string fishingTrip = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
            HashSet<int> shipLogBookPageIds = GetShipLogBookPageIdsForTrip(fishingTrip);
            List<FACatchType> unloadedFaCatches = fishingActivity.SpecifiedFACatch.Where(x => x.TypeCode.Value == nameof(FaCatchTypes.UNLOADED)).ToList();

            MapFaCatchToOriginDeclaration(fishingActivity, unloadedFaCatches, UnloadingTypesEnum.UNL, documentOccurrenceDateTime);

            relatedPages = SubmitShipLogBookPages(shipLogBookPageIds);

            return relatedPages;
        }

        private List<ShipLogBookPage> UpdateFluxFAReportDeclarationLanding(FishingActivityType fishingActivity,
                                                                           DateTime documentOccurrenceDateTime,
                                                                           IDType referenceId)
        {
            // Delete from DB unloaded catches from the related document, which is being updated

            FvmsfishingActivityReport referencedReport = GetDocumentByUUID((Guid)referenceId);
            FLUXFAReportMessageType referencedMessage = CommonUtils.JsonDeserialize<FLUXFAReportMessageType>(referencedReport.ResponseMessage);
            FishingActivityType prevFishingActivity = referencedMessage.FAReportDocument.SelectMany(x => x.SpecifiedFishingActivity)
                                                                                        .Where(y => y.TypeCode.Value == nameof(FaTypes.LANDING))
                                                                                        .First();

            DeletePreviouslyUnloadedCatches(prevFishingActivity, referencedMessage.FLUXReportDocument.CreationDateTime.Item);

            // Map and add to DB new unloaded catches

            List<FACatchType> newUnloadedFaCatches = fishingActivity.SpecifiedFACatch
                                                                    .Where(x => x.TypeCode.Value == nameof(FaCatchTypes.UNLOADED))
                                                                    .ToList();

            OriginDeclarationData originDeclartionsData = MapFaCatchToOriginDeclaration(fishingActivity, newUnloadedFaCatches, UnloadingTypesEnum.UNL, documentOccurrenceDateTime);

            string fishingTrip = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
            HashSet<int> relatedShipLogBookPageIds = originDeclartionsData.LogBookPageOriginDeclarationFishes.Keys.ToHashSet();

            List<ShipLogBookPage> relatedPages = (from logBookPage in Db.ShipLogBookPages
                                                  where relatedShipLogBookPageIds.Contains(logBookPage.Id)
                                                  select logBookPage).ToList();

            return relatedPages;
        }

        private List<ShipLogBookPage> CancelFluxFAReportDeclarationLanding(FishingActivityType fishingActivity, DateTime fluxReportCreationDate)
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

        private List<ShipLogBookPage> DeleteFluxFAReportDeclarationLanding(FishingActivityType fishingActivity, DateTime fluxReportCreationDate)
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

            OriginDeclarationData previousOriginDeclartionsData = GetOriginDeclarationsData(fishingActivity,
                                                                                            previouslyUnloadedFaCatches,
                                                                                            UnloadingTypesEnum.UNL,
                                                                                            fluxReportCreationDate);


            List<OriginDeclarationFish> dbOriginDeclarationFishes = (from originDeclarationFish in Db.OriginDeclarationFish
                                                                     join originDeclaration in Db.OriginDeclarations on originDeclarationFish.OriginDeclarationId equals originDeclaration.Id
                                                                     where prevRelatedLogBookPageIds.Contains(originDeclaration.LogBookPageId)
                                                                     select originDeclarationFish).ToList();

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
                        dbOriginDeclarationFish.IsActive = false;
                    }
                }

                if (previousOriginDeclaration != null) // Delete previous origin declaration
                {
                    previousOriginDeclaration.IsActive = false;
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
    }
}
