using System.Linq;
using System.Collections.Generic;
using IARA.EntityModels.Entities;
using IARA.Flux.Models;
using System;
using TL.SysToSysSecCom;
using IARA.FluxModels.Enums;

namespace IARA.Infrastructure.FluxIntegrations.Helpers
{
    internal partial class FishingActivityHelper
    {
        private List<ShipLogBookPage> MapFluxFAReportDeclarationGearShot(FishingActivityType fishingActivity)
        {
            List<ShipLogBookPage> relatedPages = new List<ShipLogBookPage>();

            GearShotData gearShotData = CreateCatchRecordFromGearShot(fishingActivity);

            Db.CatchRecords.Add(gearShotData.CatchRecord);

            relatedPages.Add(gearShotData.RelatedLogBookPage);

            return relatedPages;
        }

        private List<ShipLogBookPage> UpdateFluxFAReportDeclarationGearShot(FishingActivityType fishingActivity)
        {
            List<ShipLogBookPage> relatedPages = new List<ShipLogBookPage>();

            // Get previous fishing gear shot data

            (FishingGearCatchRecord, CatchRecord) prevCatchRecordData = GetPrevSetGearCatchRecordData(fishingActivity);

            // Get new fishing gear shot data

            GearShotData newGarShotData = CreateCatchRecordFromGearShot(fishingActivity);

            if (prevCatchRecordData.Item1.FishingGearRegisterId == newGarShotData.RelatedLogBookPage.FishingGearRegisterId) // Same gear was shot
            {
                // Update catch record data
                prevCatchRecordData.Item2.GearEntryTime = newGarShotData.CatchRecord.GearEntryTime;
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

        private List<ShipLogBookPage> CancelFluxFAReportDeclarationGearShot(FishingActivityType fishingActivity)
        {
            List<ShipLogBookPage> relatedPages = new List<ShipLogBookPage>();

            (FishingGearCatchRecord, CatchRecord) prevCatchRecordData = GetPrevSetGearCatchRecordData(fishingActivity);
            DeleteCatchRecordAndCatchRecordFishes(prevCatchRecordData.Item2);

            ShipLogBookPage relatedPage = (from page in Db.ShipLogBookPages
                                           where page.Id == prevCatchRecordData.Item2.LogBookPageId
                                           select page).First();

            relatedPages.Add(relatedPage);

            return relatedPages;
        }

        private List<ShipLogBookPage> DeleteFluxFAReportDeclarationGearShot(FishingActivityType fishingActivity)
        {
            List<ShipLogBookPage> relatedPages = new List<ShipLogBookPage>();

            (FishingGearCatchRecord, CatchRecord) prevCatchRecordData = GetPrevSetGearCatchRecordData(fishingActivity);
            DeleteCatchRecordAndCatchRecordFishes(prevCatchRecordData.Item2);

            ShipLogBookPage relatedPage = (from page in Db.ShipLogBookPages
                                           where page.Id == prevCatchRecordData.Item2.LogBookPageId
                                           select page).First();

            relatedPages.Add(relatedPage);

            return relatedPages;
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

        private (FishingGearCatchRecord, CatchRecord) GetPrevSetGearCatchRecordData(FishingActivityType fishingActivity)
        {
            string fisihingTripIdentifier = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
            HashSet<int> referencedShipLogBookPageIds = GetShipLogBookPageIdsForTrip(fisihingTripIdentifier);
            FishingGearCatchRecord prevFishingGearCatchRecord = MapGearShot(fishingActivity, fishingActivity);

            CatchRecord dbCatchRecord = (from catchRecord in Db.CatchRecords
                                         where referencedShipLogBookPageIds.Contains(catchRecord.LogBookPageId)
                                               && catchRecord.GearEntryTime == prevFishingGearCatchRecord.CatchRecord.GearEntryTime
                                               && catchRecord.Depth == prevFishingGearCatchRecord.CatchRecord.Depth
                                               && catchRecord.CatchOperCount == prevFishingGearCatchRecord.CatchRecord.CatchOperCount
                                         select catchRecord).First();

            return (prevFishingGearCatchRecord, dbCatchRecord);
        }

        private GearShotData CreateCatchRecordFromGearShot(FishingActivityType fishingActivity)
        {
            FishingGearCatchRecord fishingGearCatchRecord = MapGearShot(fishingActivity, fishingActivity);

            string fishingTrip = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
            HashSet<int> relatedLogBookPageIds = GetShipLogBookPageIdsForTrip(fishingTrip);

            ShipLogBookPage shipLogBookPage = (from logBookPage in Db.ShipLogBookPages
                                               where relatedLogBookPageIds.Contains(logBookPage.Id)
                                                     && logBookPage.FishingGearRegisterId == fishingGearCatchRecord.FishingGearRegisterId
                                               select logBookPage).Single();

            CatchRecord catchRecord = fishingGearCatchRecord.CatchRecord;
            catchRecord.LogBookPageId = shipLogBookPage.Id;

            GearShotData gearShotData = new GearShotData
            {
                CatchRecord = catchRecord,
                RelatedLogBookPage = shipLogBookPage
            };

            return gearShotData;
        }

        private FishingGearCatchRecord MapGearShot(FishingActivityType subActivity, FishingActivityType fishingActivity)
        {
            FishingGearCatchRecord fishingGearCatchRecord = new FishingGearCatchRecord();
            int? operationsCount = fishingActivity.OperationsQuantity != null ? (int?)fishingActivity.OperationsQuantity.Value : default;
            int? specifiedFishingGearId = MapSpecifiedFishingGear(fishingActivity.SpecifiedFishingGear);

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
                CatchOperCount = operationsCount,
                Depth = depth
            };

            if (subActivity.SpecifiedFishingGear != null)
            {
                FishingGearCharacteristics gearCharacteristics = GetFluxFishingGearCharacteristics(subActivity.SpecifiedFishingGear[0].ApplicableGearCharacteristic);
                FishingGearDataHelper fishingGearData = GetFishingGearData(subActivity.SpecifiedFishingGear[0], gearCharacteristics);

                fishingGearCatchRecord.FishingGearRegisterId = fishingGearData.FishingGearRegisterId;
            }
            else if (specifiedFishingGearId.HasValue)
            {
                fishingGearCatchRecord.FishingGearRegisterId = specifiedFishingGearId.Value;
            }
            else
            {
                string fishingTrip = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
                Logger.LogWarning($"{LOGGER_MSG_TYPE} no gear to shot is found in our DB for trip: {fishingTrip}");
            }

            fishingGearCatchRecord.CatchRecord = cr;

            return fishingGearCatchRecord;
        }

    }

    internal class GearShotData
    {
        public CatchRecord CatchRecord { get; set; }

        public ShipLogBookPage RelatedLogBookPage { get; set; }
    }
}
