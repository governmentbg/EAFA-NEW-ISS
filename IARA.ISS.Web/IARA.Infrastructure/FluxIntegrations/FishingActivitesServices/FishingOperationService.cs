using IARA.Flux.Models;
using IARA.FluxModels.Enums;
using IARA.Infrastructure.FluxIntegrations.FishingActivitesServices.Models;
using IARA.Infrastructure.FluxIntegrations.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TL.Logging.Abstractions.Interfaces;

namespace IARA.Infrastructure.FluxIntegrations.FishingActivitesServices
{
    internal class FishingOperationService : BaseFishingActivityService
    {
        private readonly GearShotService _gearShotService;
        private readonly GearRetrievalService _gearRetrievalService;

        public FishingOperationService(IARADbContext dbContext, IExtendedLogger logger)
            : base(dbContext, logger, nameof(FishingOperationService))
        {
            _gearShotService = new GearShotService(dbContext, logger);
            _gearRetrievalService = new GearRetrievalService(dbContext, logger);
        }

        public List<ShipLogBookPage> MapOriginal(FishingActivityType fishingActivity, VesselTransportMeansType specifiedVesselTransportMeans, IDType[] relatedReportIds = null)
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
                            relatedPages = MapFluxFaReportFishingOperationWithFish(fishingActivity, relatedReportIds);
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
                string message = $"unknown vessel related activity code: {activityCode} for fishing trip: {fishingTripIdentifier}.";
                LogWarning(message, nameof(MapOriginal));
            }

            return relatedPages;
        }

        public List<ShipLogBookPage> MapUpdate(FishingActivityType fishingActivity, IDType previousReportId, IDType[] relatedReportsIdentifiers, VesselTransportMeansType specifiedVesselTransportMeans)
        {
            FvmsfishingActivityReport referencedReport = GetDocumentByUUID((Guid)previousReportId);
            FLUXFAReportMessageType referencedMessage = CommonUtils.Deserialize<FLUXFAReportMessageType>(referencedReport.ResponseMessage);

            FishingActivityType prevFishingActivity = referencedMessage.FAReportDocument.SelectMany(x => x.SpecifiedFishingActivity)
                                                                                        .Where(x => x.TypeCode.Value == nameof(FaTypes.FISHING_OPERATION))
                                                                                        .First();

            // Delete previous fishing operations and catches, according to the specified fishing activity

            DeletePreviousCatchRecordsForRelatedFishingActivity(prevFishingActivity, relatedReportsIdentifiers);

            // Map them again - with the new data from the new fishing activity

            List<ShipLogBookPage> relatedPages = MapOriginal(fishingActivity, specifiedVesselTransportMeans, relatedReportsIdentifiers);

            return relatedPages;
        }

        public List<ShipLogBookPage> MapCancel(FishingActivityType fishingActivity, IDType previousReportId, IDType[] relatedReportsIdentifiers)
        {
            FvmsfishingActivityReport referencedReport = GetDocumentByUUID((Guid)previousReportId);
            FLUXFAReportMessageType referencedMessage = CommonUtils.Deserialize<FLUXFAReportMessageType>(referencedReport.ResponseMessage);

            FishingActivityType prevFishingActivity = referencedMessage.FAReportDocument.SelectMany(x => x.SpecifiedFishingActivity)
                                                                                        .Where(x => x.TypeCode.Value == nameof(FaTypes.FISHING_OPERATION))
                                                                                        .First();

            // Delete previous fishing operations and catches, according to the specified fishing activity

            List<ShipLogBookPage> relatedPages = DeletePreviousCatchRecordsForRelatedFishingActivity(prevFishingActivity, relatedReportsIdentifiers);

            return relatedPages;
        }

        public List<ShipLogBookPage> MapDelete(FishingActivityType fishingActivity, IDType previousReportId, IDType[] relatedReportsIdentifiers)
        {
            FvmsfishingActivityReport referencedReport = GetDocumentByUUID((Guid)previousReportId);
            FLUXFAReportMessageType referencedMessage = CommonUtils.Deserialize<FLUXFAReportMessageType>(referencedReport.ResponseMessage);

            FishingActivityType prevFishingActivity = referencedMessage.FAReportDocument.SelectMany(x => x.SpecifiedFishingActivity)
                                                                                        .Where(x => x.TypeCode.Value == nameof(FaTypes.FISHING_OPERATION))
                                                                                        .First();

            // Delete previous fishing operations and catches, according to the specified fishing activity

            List<ShipLogBookPage> relatedPages = DeletePreviousCatchRecordsForRelatedFishingActivity(prevFishingActivity, relatedReportsIdentifiers);

            return relatedPages;
        }

        // Helper methods

        private List<ShipLogBookPage> MapFluxFaReportFishingOperationWithFish(FishingActivityType fishingActivity, IDType[] relatedReportsIdentifiers)
        {
            List<ShipLogBookPage> relatedPages = new List<ShipLogBookPage>();
            string fishingTripIdentifier = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);

            if (fishingActivity.OperationsQuantity != null && fishingActivity.OperationsQuantity.Value > 0)
            {
                HashSet<int> relatedShipLogBookPageIds = GetShipLogBookPageIdsForTrip(fishingTripIdentifier);

                if (relatedShipLogBookPageIds.Count == 0)
                {
                    throw new ArgumentException($"No related ship log book page ids found for Fishing Operation for fishing trip identifier: {fishingTripIdentifier}");
                }

                FishingOperationData fishingOperationData = GetFishingOperationData(fishingActivity, relatedReportsIdentifiers);

                if (fishingOperationData.FishingGearCatchRecord.FishingGearRegisterId.HasValue) // add catch records and fishes to DB
                {
                    int fishingGearRegisterId = fishingOperationData.FishingGearCatchRecord.FishingGearRegisterId.Value;
                    CatchRecord catchRecord = fishingOperationData.FishingGearCatchRecord.CatchRecord;

                    ShipLogBookPage shipLogBookPage = (from shipLBPage in Db.ShipLogBookPages
                                                       where relatedShipLogBookPageIds.Contains(shipLBPage.Id)
                                                             && shipLBPage.FishingGearRegisterId == fishingGearRegisterId
                                                       orderby shipLBPage.Id descending
                                                       select shipLBPage).FirstOrDefault();

                    if (catchRecord != null)
                    {
                        if (relatedReportsIdentifiers != null && catchRecord.LogBookPageId != default) // Пускането на уреда е към страница от предишен рейс
                        {
                            shipLogBookPage = (from shipLBPage in Db.ShipLogBookPages
                                               where shipLBPage.Id == catchRecord.LogBookPageId
                                               select shipLBPage).SingleOrDefault();

                            if (shipLogBookPage == null)
                            {
                                throw new ArgumentException($"No previous ship log book page found for fishing trip identifier: {fishingTripIdentifier}");
                            }
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
            else
            {
                LogWarning($"Zero fishing operations quantity provided for fishing trip identifier: ${fishingTripIdentifier}", nameof(MapFluxFaReportFishingOperationWithFish)); // "FishingOperationFAHelper", "MapFluxFaReportFishingOperationWithFish"
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

                if (relatedShipLogBookPageIds.Count == 0)
                {
                    throw new ArgumentException($"No related ship log book pages found for fishing trip identifier: {fishingTripIdentifier}");
                }

                FishingGearCatchRecordData fishingGearCatchRecord = _gearShotService.MapGearShot(fishingActivity.RelatedFishingActivity[0], fishingActivity);
                
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

            List<ShipLogBookPage> tripShipLogBookPages = GetTripRelatedLogBookPages(fishingActivity);
            HashSet<int> relatedShipLogBookPageIds = tripShipLogBookPages.Select(x => x.Id).ToHashSet();

            if (fishingActivity.VesselRelatedActivityCode.Value == nameof(VesselActivityCodes.FIS))
            {
                List<CatchRecord> relatedDbCatches = (from catchRecord in Db.CatchRecords
                                                                            .Include(x => x.CatchRecordFishes)
                                                                            .AsSplitQuery()
                                                      where relatedShipLogBookPageIds.Contains(catchRecord.LogBookPageId)
                                                            && catchRecord.IsActive
                                                      select catchRecord).ToList();
                HashSet<int> relatedDbCatchIds = relatedDbCatches.Select(x => x.Id).ToHashSet();

                List<CatchRecordFish> relatedDbCatchFishes = (from catchRecordFish in Db.CatchRecordFish
                                                                                        .Include(x => x.Fish)
                                                                                        .AsSplitQuery()
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

                        CatchRecord prevCatchRecordData = prevFishingOperationData.FishingGearCatchRecord.CatchRecord;

                        CatchRecord dbCatchRecord = (from catchRecord in relatedDbCatches
                                                     where shipLogBookPageIds.Contains(catchRecord.LogBookPageId)
                                                            && catchRecord.CatchOperCount == prevCatchRecordData.CatchOperCount
                                                            && catchRecord.Depth == prevCatchRecordData.Depth
                                                            && catchRecord.GearEntryTime == prevCatchRecordData.GearEntryTime
                                                            && catchRecord.HasGearEntry == prevCatchRecordData.HasGearEntry
                                                            && catchRecord.GearExitTime == prevCatchRecordData.GearExitTime
                                                            && catchRecord.HasGearExit == prevCatchRecordData.HasGearExit
                                                     select catchRecord).FirstOrDefault();

                        if (dbCatchRecord != null)
                        {
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
                            relatedPages.Add(relatedPage);
                        }
                        else
                        {
                            string fishingTrip = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
                            string msg = $"No catch record found to delete for trip identifier: {fishingTrip} for fishing gear id: {prevFishingOperationData.FishingGearCatchRecord.FishingGearRegisterId}";
                            LogWarning(msg, nameof(DeletePreviousCatchRecordsForRelatedFishingActivity));
                        }
                    }
                }
            }
            else if (fishingActivity.VesselRelatedActivityCode.Value == nameof(VesselActivityCodes.SET)) // Delete data only for setting gear
            {
                List<int> usedPageIds = DeleteSetGearFishingOperationData(fishingActivity, relatedShipLogBookPageIds);
                List<ShipLogBookPage> usedPages = tripShipLogBookPages.Where(x => usedPageIds.Contains(x.Id)).ToList();
                relatedPages.AddRange(usedPages);
            }

            return relatedPages;
        }

        private FishingOperationData GetFishingOperationData(FishingActivityType fishingActivity, IDType[] relatedReportsIdentifiers)
        {
            IDictionary<int, List<CatchRecordFish>> fishingGearRegisterCatchRecordFishes = new Dictionary<int, List<CatchRecordFish>>();

            FishingGearCatchRecordData fishingGearCatchRecord = GetSpecifiedCatchFishingGearRegisterId(fishingActivity, relatedReportsIdentifiers);

            if (fishingActivity.SpecifiedFACatch != null) // map catch record fishes
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
                            LogWarning($"no fishing gear is found for catch with fish_id: {catchRecordFish.FishId}", nameof(GetFishingOperationData));
                        }

                    }
                    else
                    {
                        string fishingTrip = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
                        string msg = $"Unhandled case, when the SpecifiedFACatch as already an applied process to it when caugth? For TripIdentifier: {fishingTrip}";
                        LogWarning(msg, nameof(GetFishingOperationData));
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

        private List<int> DeleteSetGearFishingOperationData(FishingActivityType fishingActivity, HashSet<int> relatedShipLogBookPageIds)
        {
            List<int> usedPageIds = new List<int>();

            if (fishingActivity.VesselRelatedActivityCode.Value == nameof(VesselActivityCodes.SET))
            {
                FishingGearCatchRecordData fishingGearCatchRecord = _gearShotService.MapGearShot(fishingActivity.RelatedFishingActivity[0], fishingActivity);
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

        private FishingGearCatchRecordData GetSpecifiedCatchFishingGearRegisterId(FishingActivityType fishingActivity, IDType[] relatedReportsIdentifiers)
        {
            FishingGearCatchRecordData fishingGearCatchRecord = new FishingGearCatchRecordData();

            int? specifiedFishingGearRegisterId = default;

            // override fishing gear with specified fishing gear (if any)

            if (fishingActivity.SpecifiedFishingGear != null && fishingActivity.SpecifiedFishingGear.Length > 0)
            {
                FishingGearCharacteristics gearCharacteristics = GetFluxFishingGearCharacteristics(fishingActivity.SpecifiedFishingGear[0].ApplicableGearCharacteristic);
                specifiedFishingGearRegisterId = GetFishingGearData(fishingActivity.SpecifiedFishingGear[0], gearCharacteristics, fishingActivity.SpecifiedFishingTrip).FishingGearRegisterId;
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
                    string message = $"More than one (exactly: {fishingActivity.SpecifiedDelimitedPeriod.Length} specified delimited periods in the fishing acitivity for trip: {tripIdentifier}";
                    LogWarning(message, nameof(GetSpecifiedCatchFishingGearRegisterId));
                }

                if (fishingGearCatchRecord.CatchRecord.HasGearEntry.HasValue && fishingGearCatchRecord.CatchRecord.HasGearEntry.Value)
                {
                    fishingGearCatchRecord.CatchRecord.GearEntryTime = (DateTime)fishingActivity.SpecifiedDelimitedPeriod[0].StartDateTime;
                }

                if (fishingGearCatchRecord.CatchRecord.HasGearExit.HasValue && fishingGearCatchRecord.CatchRecord.HasGearExit.Value)
                {
                    fishingGearCatchRecord.CatchRecord.GearExitTime = (DateTime)fishingActivity.SpecifiedDelimitedPeriod[0].EndDateTime;
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
                LogWarning($"the zone location of a catch is not found in our DB for species: {specifiedCatch.SpeciesCode.Value}", nameof(MapFACatchTypeToCatchRecordFish)); // TODO add fishing trip id
            }

            int? catchSizeId = GetCatchSizeId(specifiedCatch.SpecifiedSizeDistribution);

            if (!catchSizeId.HasValue)
            {
                string fluxSizeDistributionCode = "NO_CODE";
                if (specifiedCatch.SpecifiedSizeDistribution != null && specifiedCatch.SpecifiedSizeDistribution.ClassCode != null && specifiedCatch.SpecifiedSizeDistribution.ClassCode.Length > 0)
                {
                    fluxSizeDistributionCode = specifiedCatch.SpecifiedSizeDistribution.ClassCode[0].Value;
                }

                LogWarning($"no catch size with code: {fluxSizeDistributionCode}, is found in our DB.", nameof(MapFACatchTypeToCatchRecordFish)); // TODO add fishing trip id
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
                FishId = aquaticOrganismId,
                CatchTypeId = catchTypeId,
                CatchSizeId = catchSizeId,
                CatchZoneId = zoneId ?? -1,
                SturgeonGender = null,
                SturgeonSize = null,
                ThirdCountryCatchZone = null,
                IsContinentalCatch = false,
                TurbotCount = turbotCount,
                TurbotSizeGroupId = turbotSizeGroupId
            };

            return catchRecordFish;
        }

        private FishingGearCatchRecordData HandleRelatedFishingActivities(FishingActivityType fishingActivity, FishingGearCatchRecordData oldFishingGearCatchRecord, IDType[] relatedReportsIdentifiers)
        {
            FishingGearCatchRecordData fishingGearCatchRecord = oldFishingGearCatchRecord;

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
                                fishingGearCatchRecord = _gearShotService.MapGearShot(subActivity, fishingActivity);
                            }
                            break;
                        case FaTypes.GEAR_RETRIEVAL:
                            {
                                if (relatedReportsIdentifiers != null && relatedReportsIdentifiers.Length > 0) // The current report is related to another - where the fishing gear is SET probably present
                                {
                                    Guid referencedId = (Guid)relatedReportsIdentifiers[0];
                                    FvmsfishingActivityReport fvmsRelatedActivityReport = GetDocumentByUUID(referencedId);
                                    FLUXFAReportMessageType faReportMessage = TL.SysToSysSecCom.CommonUtils.JsonDeserialize<FLUXFAReportMessageType>(fvmsRelatedActivityReport.ResponseMessage);

                                    if (faReportMessage.FAReportDocument.Length > 0 && faReportMessage.FAReportDocument[0].SpecifiedFishingActivity.Length > 0)
                                    {
                                        _gearRetrievalService.MapGearRetrieval(fishingActivity,
                                                                               ref fishingGearCatchRecord,
                                                                               fishingActivity.SpecifiedFishingGear,
                                                                               faReportMessage.FAReportDocument[0].SpecifiedFishingActivity[0].SpecifiedFishingTrip,
                                                                               fishingActivity);
                                    }
                                    else
                                    {
                                        string msg = $"Document from another trip does not have FaReports or just a specified fishing activity. For Document with id: {referencedId}";
                                        LogWarning(msg, nameof(HandleRelatedFishingActivities));
                                    }
                                }
                                else
                                {
                                    _gearRetrievalService.MapGearRetrieval(fishingActivity, ref fishingGearCatchRecord, fishingActivity.SpecifiedFishingGear, null, fishingActivity);
                                }
                            }
                            break;
                    }
                }
                else
                {
                    LogWarning($"Unknown subActivityTypeCode: ${subActivityTypeCode}", nameof(HandleRelatedFishingActivities));
                }
            }

            return fishingGearCatchRecord;
        }
    }
}
