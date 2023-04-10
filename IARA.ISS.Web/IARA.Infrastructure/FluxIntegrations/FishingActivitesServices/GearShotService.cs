using IARA.Flux.Models;
using IARA.Infrastructure.FluxIntegrations.FishingActivitesServices.Models;
using IARA.Infrastructure.FluxIntegrations.Helpers;
using Microsoft.Extensions.Logging;
using TL.Logging.Abstractions.Interfaces;

namespace IARA.Infrastructure.FluxIntegrations.FishingActivitesServices
{
    internal class GearShotService : BaseFishingActivityService
    { 
        public GearShotService(IARADbContext dbContext, IExtendedLogger logger)
            : base(dbContext, logger, "GearShotService.cs")
        { }

        public List<ShipLogBookPage> MapOriginal(FishingActivityType fishingActivity)
        {
            List<ShipLogBookPage> relatedPages = new List<ShipLogBookPage>();

            GearShotData gearShotData = CreateCatchRecordFromGearShot(fishingActivity);

            Db.CatchRecords.Add(gearShotData.CatchRecord);

            relatedPages.Add(gearShotData.RelatedLogBookPage);

            return relatedPages;
        }

        public List<ShipLogBookPage> MapUpdate(FishingActivityType fishingActivity)
        {
            List<ShipLogBookPage> relatedPages = new List<ShipLogBookPage>();

            // Get previous fishing gear shot data

            (FishingGearCatchRecordData, CatchRecord) prevCatchRecordData = GetPrevSetGearCatchRecordData(fishingActivity);

            // Get new fishing gear shot data

            GearShotData newGarShotData = CreateCatchRecordFromGearShot(fishingActivity);

            if (prevCatchRecordData.Item1.FishingGearRegisterId == newGarShotData.RelatedLogBookPage.FishingGearRegisterId) // Same gear was shot
            {
                // Update catch record data
                prevCatchRecordData.Item2.GearEntryTime = newGarShotData.CatchRecord.GearEntryTime;
                prevCatchRecordData.Item2.HasGearEntry = prevCatchRecordData.Item2.GearEntryTime.HasValue;
                prevCatchRecordData.Item2.CatchOperCount = newGarShotData.CatchRecord.CatchOperCount;
                prevCatchRecordData.Item2.Depth = newGarShotData.CatchRecord.Depth;
            }
            else // A different gear was shot
            {
                // Delete previous catch record and catch record fishes (if any)

                DeleteCatchRecordAndCatchRecordFishes(prevCatchRecordData.Item2);

                // Add new catch record
                Db.CatchRecords.Add(newGarShotData.CatchRecord);
            }

            relatedPages.Add(newGarShotData.RelatedLogBookPage);

            return relatedPages;
        }

        public List<ShipLogBookPage> MapCancel(FishingActivityType fishingActivity)
        {
            List<ShipLogBookPage> relatedPages = new List<ShipLogBookPage>();

            (FishingGearCatchRecordData, CatchRecord) prevCatchRecordData = GetPrevSetGearCatchRecordData(fishingActivity);
            DeleteCatchRecordAndCatchRecordFishes(prevCatchRecordData.Item2);

            ShipLogBookPage relatedPage = (from page in Db.ShipLogBookPages
                                           where page.Id == prevCatchRecordData.Item2.LogBookPageId
                                           select page).First();

            relatedPages.Add(relatedPage);

            return relatedPages;
        }

        public List<ShipLogBookPage> MapDelete(FishingActivityType fishingActivity)
        {
            List<ShipLogBookPage> relatedPages = new List<ShipLogBookPage>();

            (FishingGearCatchRecordData, CatchRecord) prevCatchRecordData = GetPrevSetGearCatchRecordData(fishingActivity);
            DeleteCatchRecordAndCatchRecordFishes(prevCatchRecordData.Item2);

            ShipLogBookPage relatedPage = (from page in Db.ShipLogBookPages
                                           where page.Id == prevCatchRecordData.Item2.LogBookPageId
                                           select page).First();

            relatedPages.Add(relatedPage);

            return relatedPages;
        }

        public FishingGearCatchRecordData MapGearShot(FishingActivityType subActivity, FishingActivityType fishingActivity)
        {
            FishingGearCatchRecordData fishingGearCatchRecord = new FishingGearCatchRecordData();
            int? operationsCount = fishingActivity.OperationsQuantity != null ? (int?)fishingActivity.OperationsQuantity.Value : default;
            int? specifiedFishingGearId = MapSpecifiedFishingGear(fishingActivity.SpecifiedFishingGear, fishingActivity);

            DateTime gearEntryDateTime;

            if (subActivity.SpecifiedDelimitedPeriod != null && subActivity.SpecifiedDelimitedPeriod.Length > 0)
            {
                gearEntryDateTime = (DateTime)subActivity.SpecifiedDelimitedPeriod[0].StartDateTime;
            }
            else
            {
                gearEntryDateTime = (DateTime)subActivity.OccurrenceDateTime;
            }

            int? depth = (int?)GetFishingGearDepth(subActivity.SpecifiedFLUXCharacteristic);

            CatchRecord cr = new CatchRecord
            {
                GearEntryTime = gearEntryDateTime,
                HasGearEntry = true,
                HasGearExit = false,
                CatchOperCount = operationsCount,
                Depth = depth
            };

            if (subActivity.SpecifiedFishingGear != null) // if there is a gear here, use to override the others above
            {
                FishingGearCharacteristics gearCharacteristics = GetFluxFishingGearCharacteristics(subActivity.SpecifiedFishingGear[0].ApplicableGearCharacteristic);
                FishingGearData fishingGearData = GetFishingGearData(subActivity.SpecifiedFishingGear[0], gearCharacteristics, fishingActivity.SpecifiedFishingTrip);

                fishingGearCatchRecord.FishingGearRegisterId = fishingGearData.FishingGearRegisterId;
            }
            else if (specifiedFishingGearId.HasValue)
            {
                fishingGearCatchRecord.FishingGearRegisterId = specifiedFishingGearId.Value;
            }
            else
            {
                string fishingTrip = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
                LogWarning($"No gear to shot is found in our DB for trip: {fishingTrip}", "MapGearShot");
            }

            fishingGearCatchRecord.CatchRecord = cr;

            return fishingGearCatchRecord;
        }

        public GearShotData CreateCatchRecordFromGearShot(FishingActivityType fishingActivity)
        {
            FishingGearCatchRecordData fishingGearCatchRecord = MapGearShot(fishingActivity, fishingActivity);

            string fishingTrip = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
            HashSet<int> relatedLogBookPageIds = GetShipLogBookPageIdsForTrip(fishingTrip);

            if (relatedLogBookPageIds.Count == 0)
            {
                throw new ArgumentException($"No related ship log book pages found for FO gear shot for trip identifier: {fishingTrip}");
            }

            ShipLogBookPage shipLogBookPage = (from logBookPage in Db.ShipLogBookPages
                                               where relatedLogBookPageIds.Contains(logBookPage.Id)
                                                     && logBookPage.FishingGearRegisterId == fishingGearCatchRecord.FishingGearRegisterId
                                               select logBookPage).SingleOrDefault();

            if (shipLogBookPage == null)
            {
                throw new ArgumentException($"No ship log book page found for fishing gear register id: {fishingGearCatchRecord.FishingGearRegisterId} for fishing trip identifier: {fishingTrip}");
            }

            CatchRecord catchRecord = fishingGearCatchRecord.CatchRecord;
            catchRecord.LogBookPageId = shipLogBookPage.Id;

            GearShotData gearShotData = new GearShotData
            {
                CatchRecord = catchRecord,
                RelatedLogBookPage = shipLogBookPage
            };

            return gearShotData;
        }

        // Helper methods

        private (FishingGearCatchRecordData, CatchRecord) GetPrevSetGearCatchRecordData(FishingActivityType fishingActivity)
        {
            string fisihingTripIdentifier = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
            HashSet<int> referencedShipLogBookPageIds = GetShipLogBookPageIdsForTrip(fisihingTripIdentifier);
            FishingGearCatchRecordData prevFishingGearCatchRecord = MapGearShot(fishingActivity, fishingActivity);

            CatchRecord dbCatchRecord = (from catchRecord in Db.CatchRecords
                                         where referencedShipLogBookPageIds.Contains(catchRecord.LogBookPageId)
                                               && catchRecord.GearEntryTime == prevFishingGearCatchRecord.CatchRecord.GearEntryTime
                                               && catchRecord.Depth == prevFishingGearCatchRecord.CatchRecord.Depth
                                               && catchRecord.CatchOperCount == prevFishingGearCatchRecord.CatchRecord.CatchOperCount
                                         select catchRecord).First();

            return (prevFishingGearCatchRecord, dbCatchRecord);
        }

        private void DeleteCatchRecordAndCatchRecordFishes(CatchRecord dbCatchRecord)
        {
            dbCatchRecord.IsActive = false;

            // Delete caught fish if any

            List<CatchRecordFish> dbCatchRecordFishes = (from catchRecordFish in Db.CatchRecordFish
                                                         where catchRecordFish.CatchRecordId == dbCatchRecord.Id
                                                         select catchRecordFish).ToList();

            foreach (CatchRecordFish caughtFish in dbCatchRecordFishes)
            {
                caughtFish.IsActive = false;
            }
        }
    }
}
