using System;
using System.Linq;
using System.Collections.Generic;
using IARA.EntityModels.Entities;
using IARA.Flux.Models;
using IARA.FluxModels.Enums;
using TL.SysToSysSecCom;
using Microsoft.EntityFrameworkCore;
using IARA.Common.Enums;

namespace IARA.Infrastructure.FluxIntegrations.Helpers
{
    internal partial class FishingActivityHelper
    {
        private List<ShipLogBookPage> MapFluxFaReportDeclartionFishingOperation(FishingActivityType fishingActivity, IDType[] relatedReportsIdentifiers)
        {
            List<ShipLogBookPage> relatedPages = new List<ShipLogBookPage>();
            VesselActivityCodes vesselRelatedActivityCode;
            bool successfulVesselActivityCast = Enum.TryParse<VesselActivityCodes>(fishingActivity.VesselRelatedActivityCode.Value, out vesselRelatedActivityCode);

            if (successfulVesselActivityCast)
            {
                switch (vesselRelatedActivityCode)
                {
                    case VesselActivityCodes.FIS: // There is caugth fish on board AND [(gear set + gear retrieval) or (gear retrieval)]
                        {
                            relatedPages = MapFluxFaReportFishingOperationWithFish(fishingActivity, relatedReportsIdentifiers);
                        };
                        break;
                    case VesselActivityCodes.SET: // There is no caught fish, only gear set subactivity
                        {
                            relatedPages = MapFluxFaReportFishingOperationGearSet(fishingActivity);
                        }
                        break;
                }
            }
            else
            {
                string fishingTripIdentifier = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
                string activityCode = fishingActivity.VesselRelatedActivityCode.Value;
                string message = $"{LOGGER_MSG_TYPE} invalid vessel related activity code: {activityCode} for fishing trip: {fishingTripIdentifier}.";
                Logger.LogWarning(message);
            }

            return relatedPages;
        }

        private List<ShipLogBookPage> UpdateFluxFaReportDeclartionFishingOperation(FishingActivityType fishingActivity, IDType referenceId, IDType[] relatedReportsIdentifiers)
        {
            FvmsfishingActivityReport referencedReport = GetDocumentByUUID((Guid)referenceId);
            FLUXFAReportMessageType referencedMessage = CommonUtils.JsonDeserialize<FLUXFAReportMessageType>(referencedReport.ResponseMessage);
            FishingActivityType prevFishingActivity = referencedMessage.FAReportDocument.SelectMany(x => x.SpecifiedFishingActivity)
                                                                                        .Where(x => x.TypeCode.Value == nameof(FaTypes.FISHING_OPERATION))
                                                                                        .First();

            DeletePreviousCatchRecordsForRelatedFishingActivity(prevFishingActivity, relatedReportsIdentifiers);

            List<ShipLogBookPage> relatedPages = MapFluxFaReportDeclartionFishingOperation(fishingActivity, relatedReportsIdentifiers);

            return relatedPages;
        }

        private List<ShipLogBookPage> CancelFluxFaReportDeclartionFishingOperation(FishingActivityType fishingActivity, IDType[] relatedReportsIdentifiers)
        {
            List<ShipLogBookPage> relatedPages = DeletePreviousCatchRecordsForRelatedFishingActivity(fishingActivity, relatedReportsIdentifiers);
            return relatedPages;
        }

        private List<ShipLogBookPage> DeleteFluxFaReportDeclartionFishingOperation(FishingActivityType fishingActivity, IDType[] relatedReportsIdentifiers)
        {
            List<ShipLogBookPage> relatedPages = DeletePreviousCatchRecordsForRelatedFishingActivity(fishingActivity, relatedReportsIdentifiers);
            return relatedPages;
        }

        private List<ShipLogBookPage> MapFluxFaReportFishingOperationWithFish(FishingActivityType fishingActivity, IDType[] relatedReportsIdentifiers)
        {
            List<ShipLogBookPage> relatedPages = new List<ShipLogBookPage>();

            if (fishingActivity.OperationsQuantity != null && fishingActivity.OperationsQuantity.Value > 0)
            {
                string fishingTripIdentifier = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
                HashSet<int> relatedShipLogBookPageIds = GetShipLogBookPageIdsForTrip(fishingTripIdentifier);
                FishingOperationData fishingOperationData = GetFishingOperationData(fishingActivity, relatedReportsIdentifiers);

                if (fishingOperationData.FishingGearCatchRecord.FishingGearRegisterId.HasValue)
                {
                    int fishingGearRegisterId = fishingOperationData.FishingGearCatchRecord.FishingGearRegisterId.Value;
                    CatchRecord catchRecord = fishingOperationData.FishingGearCatchRecord.CatchRecord;

                    ShipLogBookPage shipLogBookPage = (from shipLBPage in Db.ShipLogBookPages
                                                       where relatedShipLogBookPageIds.Contains(shipLBPage.Id)
                                                             && shipLBPage.FishingGearRegisterId == fishingGearRegisterId
                                                       select shipLBPage).SingleOrDefault();

                    if (catchRecord != null)
                    {
                        if (relatedReportsIdentifiers != null && catchRecord.LogBookPageId != default) // Пускането на уреда е към страница от предишен рейс
                        {
                            shipLogBookPage = (from shipLBPage in Db.ShipLogBookPages
                                               where shipLBPage.Id == catchRecord.LogBookPageId
                                               select shipLBPage).Single();
                        }

                        catchRecord.LogBookPageId = shipLogBookPage.Id;

                        foreach (KeyValuePair<int, List<CatchRecordFish>> fishingGearCatchRecordFish in fishingOperationData.FishingGearRegisterCatchRecordFishes)
                        {
                            List<CatchRecordFish> catchRecordFishes = fishingGearCatchRecordFish.Value;
                            foreach (CatchRecordFish catchRecordFish in catchRecordFishes)
                            {
                                catchRecord.CatchRecordFishes.Add(catchRecordFish);
                            }
                        }

                        // Ако е имало вдигане на уред, който е бил пуснат в предишен рейс, ще има вече създаден CatchRecord запис,
                        // а ако Id-то е null, тогава трябва да се добави записът към CatchRecords таблицата, защото е нов
                        if (catchRecord.Id == default) 
                        {
                            Db.CatchRecords.Add(catchRecord);
                        }
                    }

                    relatedPages.Add(shipLogBookPage);
                }
            }

            return relatedPages;
        }

        private List<ShipLogBookPage> MapFluxFaReportFishingOperationGearSet(FishingActivityType fishingActivity)
        {
            List<ShipLogBookPage> relatedPages = new List<ShipLogBookPage>();

            if (fishingActivity.RelatedFishingActivity != null && fishingActivity.RelatedFishingActivity.Length > 0)
            {
                string fishingTripIdentifier = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
                HashSet<int> relatedShipLogBookPageIds = GetShipLogBookPageIdsForTrip(fishingTripIdentifier);
                FishingGearCatchRecord fishingGearCatchRecord = MapGearShot(fishingActivity.RelatedFishingActivity[0], fishingActivity);
                int fishingGearRegisterId = fishingGearCatchRecord.FishingGearRegisterId.Value;

                ShipLogBookPage shipLogBookPage = (from shipLBPage in Db.ShipLogBookPages
                                                   where relatedShipLogBookPageIds.Contains(shipLBPage.Id)
                                                         && shipLBPage.FishingGearRegisterId == fishingGearRegisterId
                                                   select shipLBPage).Single();

                fishingGearCatchRecord.CatchRecord.LogBookPageId = shipLogBookPage.Id;

                Db.CatchRecords.Add(fishingGearCatchRecord.CatchRecord); // Add catch record to DB

                relatedPages.Add(shipLogBookPage);
            }

            return relatedPages;
        }

        private List<ShipLogBookPage> DeletePreviousCatchRecordsForRelatedFishingActivity(FishingActivityType fishingActivity, IDType[] relatedReportsIdentifiers)
        {
            List<ShipLogBookPage> relatedPages = new List<ShipLogBookPage>();
            string fishingTripIdentifier = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
            HashSet<int> relatedShipLogBookPageIds = GetShipLogBookPageIdsForTrip(fishingTripIdentifier);

            List<ShipLogBookPage> tripShipLogBookPages = (from logBookPage in Db.ShipLogBookPages
                                                          where relatedShipLogBookPageIds.Contains(logBookPage.Id)
                                                          select logBookPage).ToList();

            if (fishingActivity.VesselRelatedActivityCode.Value == nameof(VesselActivityCodes.FIS))
            {
                List<CatchRecord> relatedDbCatches = (from catchRecord in Db.CatchRecords
                                                                            .AsSplitQuery()
                                                                            .Include(x => x.CatchRecordFishes)
                                                      where relatedShipLogBookPageIds.Contains(catchRecord.LogBookPageId)
                                                            && catchRecord.IsActive
                                                      select catchRecord).ToList();
                HashSet<int> relatedDbCatchIds = relatedDbCatches.Select(x => x.Id).ToHashSet();

                List<CatchRecordFish> relatedDbCatchFishes = (from catchRecordFish in Db.CatchRecordFish
                                                                                        .AsSplitQuery()
                                                                                        .Include(x => x.Fish)
                                                              where relatedDbCatchIds.Contains(catchRecordFish.CatchRecordId)
                                                                    && catchRecordFish.IsActive
                                                              select catchRecordFish).ToList();

                foreach (FACatchType prevFaCatch in fishingActivity.SpecifiedFACatch)
                {
                    FishingOperationData prevFishingOperationData = GetFishingOperationData(fishingActivity, relatedReportsIdentifiers);

                    if (prevFishingOperationData.FishingGearCatchRecord.CatchRecord != null)
                    {
                        HashSet<int> shipLogBookPageIds = (from lbPage in tripShipLogBookPages
                                                           where lbPage.FishingGearRegisterId == prevFishingOperationData.FishingGearCatchRecord.FishingGearRegisterId
                                                           select lbPage.Id).ToHashSet();
                        CatchRecord prevCatchRecord = prevFishingOperationData.FishingGearCatchRecord.CatchRecord;
                        CatchRecord dbCatchRecord = (from catchRecord in relatedDbCatches
                                                     where shipLogBookPageIds.Contains(catchRecord.LogBookPageId)
                                                            && catchRecord.CatchOperCount == prevCatchRecord.CatchOperCount
                                                            && catchRecord.Depth == prevCatchRecord.Depth
                                                            && catchRecord.GearEntryTime == prevCatchRecord.GearEntryTime
                                                            && catchRecord.GearExitTime == prevCatchRecord.GearExitTime
                                                     select catchRecord).First();

                        // Delete Catch Record
                        dbCatchRecord.IsActive = false;

                        // Delete Catch Record Fishes
                        CatchRecordFish dbCatchRecordFish = relatedDbCatchFishes.Where(x => x.CatchRecordId == dbCatchRecord.Id
                                                                                            && x.Quantity == prevFaCatch.WeightMeasure.Value
                                                                                            && x.Fish.Code == prevFaCatch.SpeciesCode.Value)
                                                                                .FirstOrDefault();

                        if (dbCatchRecordFish != null) // the catch record fish is found in DB
                        {
                            dbCatchRecordFish.IsActive = false;
                        }

                        ShipLogBookPage relatedPage = tripShipLogBookPages.Where(x => x.Id == dbCatchRecord.LogBookPageId).First();
                        tripShipLogBookPages.Add(relatedPage);
                    }
                }
            }

            // Delete data only for setting gear (if any)
            List<int> usedPageIds = DeleteSetGearFishingOperationData(fishingActivity, relatedShipLogBookPageIds);

            List<ShipLogBookPage> usedPages = tripShipLogBookPages.Where(x => usedPageIds.Contains(x.Id)).ToList();
            relatedPages.AddRange(usedPages);

            return relatedPages;
        }

        private List<int> DeleteSetGearFishingOperationData(FishingActivityType fishingActivity, HashSet<int> relatedShipLogBookPageIds)
        {
            List<int> usedPageIds = new List<int>();

            if (fishingActivity.VesselRelatedActivityCode.Value == nameof(VesselActivityCodes.SET))
            {
                FishingGearCatchRecord fishingGearCatchRecord = MapGearShot(fishingActivity.RelatedFishingActivity[0], fishingActivity);
                int setFishingGearRegisterId = fishingGearCatchRecord.FishingGearRegisterId.Value;

                fishingGearCatchRecord.CatchRecord.LogBookPageId = (from shipLBPage in Db.ShipLogBookPages
                                                                    where relatedShipLogBookPageIds.Contains(shipLBPage.Id)
                                                                          && shipLBPage.FishingGearRegisterId == setFishingGearRegisterId
                                                                    select shipLBPage.Id).Single();

                CatchRecord dbSetGearCatchRecord = (from cr in Db.CatchRecords
                                                    join shipLBPage in Db.ShipLogBookPages on cr.LogBookPageId equals shipLBPage.Id
                                                    where relatedShipLogBookPageIds.Contains(shipLBPage.Id)
                                                          && shipLBPage.FishingGearRegisterId == setFishingGearRegisterId
                                                          && cr.GearEntryTime == fishingGearCatchRecord.CatchRecord.GearEntryTime
                                                          && cr.Depth == fishingGearCatchRecord.CatchRecord.Depth
                                                          && cr.CatchOperCount == fishingGearCatchRecord.CatchRecord.CatchOperCount
                                                    select cr).First();

                dbSetGearCatchRecord.IsActive = false;
                usedPageIds.Add(dbSetGearCatchRecord.LogBookPageId);
            }

            return usedPageIds;
        }

        private FishingOperationData GetFishingOperationData(FishingActivityType fishingActivity, IDType[] relatedReportsIdentifiers)
        {
            IDictionary<int, List<CatchRecordFish>> fishingGearRegisterCatchRecordFishes = new Dictionary<int, List<CatchRecordFish>>();

            FishingGearCatchRecord fishingGearCatchRecord = GetSpecifiedCatchFishingGearRegisterId(fishingActivity, relatedReportsIdentifiers);

            if (fishingActivity.SpecifiedFACatch != null)
            {
                foreach (FACatchType specifiedCatch in fishingActivity.SpecifiedFACatch)
                {
                    if (specifiedCatch.AppliedAAPProcess == null || specifiedCatch.AppliedAAPProcess.Length == 0) // The catch is probably new
                    {
                        CatchRecordFish catchRecordFish = MapFACatchTypeToCatchRecordFish(specifiedCatch);

                        if (fishingGearCatchRecord.FishingGearRegisterId.HasValue)
                        {
                            if (fishingGearRegisterCatchRecordFishes.ContainsKey(fishingGearCatchRecord.FishingGearRegisterId.Value))
                            {
                                fishingGearRegisterCatchRecordFishes[fishingGearCatchRecord.FishingGearRegisterId.Value].Add(catchRecordFish);
                            }
                            else
                            {
                                fishingGearRegisterCatchRecordFishes.Add(fishingGearCatchRecord.FishingGearRegisterId.Value,
                                                                         new List<CatchRecordFish>
                                                                         {
                                                                             catchRecordFish
                                                                         });
                            }
                        }
                        else
                        {
                            Logger.LogWarning($"{LOGGER_MSG_TYPE} no fishing gear is found for catch with fish_id: {catchRecordFish.FishId}");
                        }

                    }
                    else
                    {
                        // TODO are there going to be any cases in which the fishing activity is just some kind of processed
                        // applied to an already caught fish in a previous message in the same trip ???
                    }
                }
            }

            FishingOperationData result = new FishingOperationData
            {
                FishingGearCatchRecord = fishingGearCatchRecord,
                FishingGearRegisterCatchRecordFishes = fishingGearRegisterCatchRecordFishes
            };

            return result;
        }

        private FishingGearCatchRecord GetSpecifiedCatchFishingGearRegisterId(FishingActivityType fishingActivity, IDType[] relatedReportsIdentifiers)
        {
            FishingGearCatchRecord fishingGearCatchRecord = new FishingGearCatchRecord();

            int? specifiedFishingGearRegisterId = default;

            // override fishing gear with specified fishing gear (if any)

            if (fishingActivity.SpecifiedFishingGear != null && fishingActivity.SpecifiedFishingGear.Length > 0)
            {
                FishingGearCharacteristics gearCharacteristics = GetFluxFishingGearCharacteristics(fishingActivity.SpecifiedFishingGear[0].ApplicableGearCharacteristic);
                specifiedFishingGearRegisterId = GetFishingGearData(fishingActivity.SpecifiedFishingGear[0], gearCharacteristics).FishingGearRegisterId;
            }

            if (specifiedFishingGearRegisterId.HasValue)
            {
                fishingGearCatchRecord.FishingGearRegisterId = specifiedFishingGearRegisterId.Value;
            }

            // Related fishing activities (if any) -> gear shot and/or gear retrieval

            if (fishingActivity.RelatedFishingActivity != null && fishingActivity.RelatedFishingActivity.Length > 0)
            {
                fishingGearCatchRecord = HandleRelatedFishingActivities(fishingActivity, fishingGearCatchRecord, relatedReportsIdentifiers);
            }

            // Specified delimited period (if any)

            if (fishingActivity.SpecifiedDelimitedPeriod != null && fishingActivity.SpecifiedDelimitedPeriod.Length > 0)
            {
                if (fishingActivity.SpecifiedDelimitedPeriod.Length > 1)
                {
                    string tripIdentifier = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
                    string message = $"{LOGGER_MSG_TYPE} More than one (exactly: {fishingActivity.SpecifiedDelimitedPeriod.Length} specified delimited period in the fishing acitivity for trip: {tripIdentifier}";
                    Logger.LogWarning(message);
                }

                fishingGearCatchRecord.CatchRecord.GearEntryTime = (DateTime)fishingActivity.SpecifiedDelimitedPeriod[0].StartDateTime;
                fishingGearCatchRecord.CatchRecord.GearExitTime = (DateTime)fishingActivity.SpecifiedDelimitedPeriod[0].EndDateTime;
            }

            return fishingGearCatchRecord;
        }

        private FishingGearCatchRecord HandleRelatedFishingActivities(FishingActivityType fishingActivity, FishingGearCatchRecord oldFishingGearCatchRecord, IDType[] relatedReportsIdentifiers)
        {
            FishingGearCatchRecord fishingGearCatchRecord = oldFishingGearCatchRecord;

            foreach (FishingActivityType subActivity in fishingActivity.RelatedFishingActivity) // Очаква се GEAR_SHOT винаги да идва преди GEAR_RETRIEVAL
            {
                string subActivityTypeCode = subActivity.TypeCode.Value;
                FaTypes subActivityType;
                bool successfulCastSubActivityType = Enum.TryParse<FaTypes>(subActivityTypeCode, out subActivityType);

                if (successfulCastSubActivityType)
                {
                    switch (subActivityType)
                    {
                        case FaTypes.GEAR_SHOT:
                            {
                                fishingGearCatchRecord = MapGearShot(subActivity, fishingActivity);
                            }
                            break;
                        case FaTypes.GEAR_RETRIEVAL:
                            {
                                if (relatedReportsIdentifiers != null && relatedReportsIdentifiers.Length > 0) // The current report is related to another - where the fishing gear is set probably
                                {
                                    Guid referencedId = (Guid)relatedReportsIdentifiers[0];
                                    FvmsfishingActivityReport fvmsRelatedActivityReport = GetDocumentByUUID(referencedId);
                                    FLUXFAReportMessageType faReportMessage = CommonUtils.JsonDeserialize<FLUXFAReportMessageType>(fvmsRelatedActivityReport.ResponseMessage);

                                    if (faReportMessage.FAReportDocument.Length > 0 && faReportMessage.FAReportDocument[0].SpecifiedFishingActivity.Length > 0)
                                    {
                                        MapGearRetrieval(fishingActivity, ref fishingGearCatchRecord, fishingActivity.SpecifiedFishingGear, faReportMessage.FAReportDocument[0].SpecifiedFishingActivity[0].SpecifiedFishingTrip);
                                    }
                                    else
                                    {
                                        string msg = $"{LOGGER_MSG_TYPE} Document from another trip does not have FaReports or just a specified fishing activity. For Document with id: {referencedId}";
                                        Logger.LogWarning(msg);
                                    }
                                }
                                else
                                {
                                    MapGearRetrieval(fishingActivity, ref fishingGearCatchRecord, fishingActivity.SpecifiedFishingGear, null);
                                }
                            }
                            break;
                    }
                }
            }

            return fishingGearCatchRecord;
        }

        private CatchRecordFish MapFACatchTypeToCatchRecordFish(FACatchType specifiedCatch)
        {
            string faCatchTypeCode = specifiedCatch.TypeCode.Value;
            int? catchTypeId = GetCatchTypeId(faCatchTypeCode);

            string aquaticOrganismTypeCode = specifiedCatch.SpeciesCode.Value;
            int aquaticOrganismId = GetAquaticOrganismId(aquaticOrganismTypeCode);

            int? zoneId = default;
            if (specifiedCatch.SpecifiedFLUXLocation != null)
            {
                zoneId = GetCatchLocationZoneId(specifiedCatch.SpecifiedFLUXLocation);
            }

            if (!zoneId.HasValue)
            {
                Logger.LogWarning($"{LOGGER_MSG_TYPE} the zone location of a catch is not found in our DB for species: {specifiedCatch.SpeciesCode.Value}"); // TODO add fishing trip id
            }

            int? catchSizeId = GetCatchSizeId(specifiedCatch.SpecifiedSizeDistribution);

            if (!catchSizeId.HasValue)
            {
                string fluxSizeDistributionCode = "NO_CODE";
                if (specifiedCatch.SpecifiedSizeDistribution != null && specifiedCatch.SpecifiedSizeDistribution.ClassCode != null && specifiedCatch.SpecifiedSizeDistribution.ClassCode.Length > 0)
                {
                    fluxSizeDistributionCode = specifiedCatch.SpecifiedSizeDistribution.ClassCode[0].Value;
                }

                Logger.LogWarning($"{LOGGER_MSG_TYPE} no catch size with code: {fluxSizeDistributionCode}, is found in our DB."); // TODO add fishing trip id
            }

            int? turbotCount = default;
            int? turbotSizeGroupId = default;
            if (aquaticOrganismTypeCode == nameof(FishCodesEnum.TUR))
            {
                turbotCount = specifiedCatch.UnitQuantity != null ? (int)specifiedCatch.UnitQuantity.Value : default(int?);
                turbotSizeGroupId = GetTurbotSizeGroup(specifiedCatch.SpecifiedSizeDistribution);
            }

            CatchRecordFish catchRecordFish = new CatchRecordFish
            {
                Quantity = specifiedCatch.WeightMeasure.Value,
                CatchZoneId = zoneId ?? -1,
                FishId = aquaticOrganismId,
                CatchTypeId = catchTypeId,
                CatchSizeId = catchSizeId,
                IsContinentalCatch = false, // TODO
                SturgeonGender = null, // TODO
                SturgeonSize = null, // TODO
                ThirdCountryCatchZone = null, // TODO
                TurbotCount = turbotCount,
                TurbotSizeGroupId = turbotSizeGroupId
            };

            return catchRecordFish;
        }

        private int? GetTurbotSizeGroup(SizeDistributionType specifiedSizeDistribution)
        {
            DateTime now = DateTime.Now;
            int? turbotSizeGroupId = default;

            if (specifiedSizeDistribution != null && specifiedSizeDistribution.CategoryCode != null)
            {
                string turbotSizeGroupCode = specifiedSizeDistribution.CategoryCode.Value;
                turbotSizeGroupId = (from turbotSizeGroup in Db.NturbotSizeGroups
                                     where turbotSizeGroup.Code == turbotSizeGroupCode
                                           && turbotSizeGroup.ValidFrom <= now
                                           && turbotSizeGroup.ValidTo > now
                                     select turbotSizeGroup.Id).Single();
            }

            return turbotSizeGroupId;
        }

    }

    internal class FishingOperationData
    {
        public FishingGearCatchRecord FishingGearCatchRecord { get; set; }

        public IDictionary<int, List<CatchRecordFish>> FishingGearRegisterCatchRecordFishes { get; set; }
    }
}
