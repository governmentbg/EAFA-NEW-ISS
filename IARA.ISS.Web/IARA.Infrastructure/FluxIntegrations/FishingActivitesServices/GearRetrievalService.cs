using IARA.Flux.Models;
using IARA.FluxModels.Enums;
using IARA.Infrastructure.FluxIntegrations.FishingActivitesServices.Models;
using IARA.Infrastructure.FluxIntegrations.Helpers;
using Microsoft.Extensions.Logging;
using TL.Logging.Abstractions.Interfaces;

namespace IARA.Infrastructure.FluxIntegrations.FishingActivitesServices
{
    internal class GearRetrievalService : BaseFishingActivityService
    {
        public GearRetrievalService(IARADbContext dbContext, IExtendedLogger logger)
            : base(dbContext, logger, nameof(GearRetrievalService))
        { }

        public List<ShipLogBookPage> MapOriginal(IDType[] relatedReportIdentifiers, FishingActivityType fishingActivity)
        {
            FishingGearCatchRecordData fishingGearCatchRecord = new FishingGearCatchRecordData();

            string specifiedFishingTrip = MapGearRetrievalData(fishingActivity, relatedReportIdentifiers, ref fishingGearCatchRecord);

            List<ShipLogBookPage> shipLogBookPages = SaveGearRetrievalData(specifiedFishingTrip, fishingActivity, fishingGearCatchRecord);

            return shipLogBookPages;
        }

        public List<ShipLogBookPage> MapUpdate(FishingActivityType fishingActivity, IDType referenceId, IDType[] relatedReportIdentifiers)
        {
            // Map previous gear retrieval fishing activity report data

            FishingGearCatchRecordData prevFishingGearCatchRecord = new FishingGearCatchRecordData();
            
            FvmsfishingActivityReport referencedReport = GetDocumentByUUID((Guid)referenceId);
            HashSet<int> prevReferencedShipLogBookPageIds = GetRelatedShipLogBookPagesByFVMSFAReportId(referencedReport.Id);

            FLUXFAReportMessageType referencedMessage = CommonUtils.Deserialize<FLUXFAReportMessageType>(referencedReport.ResponseMessage);

            FishingActivityType prevFishingActivity = referencedMessage.FAReportDocument.SelectMany(x => x.SpecifiedFishingActivity)
                                                                                        .Where(x => x.TypeCode.Value == nameof(FaTypes.GEAR_RETRIEVAL))
                                                                                        .First();

            IDType[] prevRelatedReportIdentifiers = referencedMessage.FAReportDocument.Where(x => x.SpecifiedFishingActivity
                                                                                                   .Any(y => y.SpecifiedFishingTrip == prevFishingActivity.SpecifiedFishingTrip))
                                                                                      .SelectMany(x => x.RelatedReportID)
                                                                                      .ToArray();
            string prevSpecifiedFishingTrip = MapGearRetrievalData(prevFishingActivity, prevRelatedReportIdentifiers, ref prevFishingGearCatchRecord);

            // Map fishingGearCatchRecord object with data for the gear retrieval from the new activity

            FishingGearCatchRecordData fishingGearCatchRecord = new FishingGearCatchRecordData();
            string specifiedFishingTrip = MapGearRetrievalData(fishingActivity, relatedReportIdentifiers, ref fishingGearCatchRecord);

            if (prevFishingGearCatchRecord.FishingGearRegisterId != fishingGearCatchRecord.FishingGearRegisterId) // the new gear is different
            {
                CatchRecord prevCatchRecord = (from cr in Db.CatchRecords
                                               join page in Db.ShipLogBookPages on cr.LogBookPageId equals page.Id
                                               where prevReferencedShipLogBookPageIds.Contains(cr.LogBookPageId)
                                                     && page.FishingGearRegisterId == prevFishingGearCatchRecord.FishingGearRegisterId
                                                     && cr.GearExitTime == prevFishingGearCatchRecord.CatchRecord.GearExitTime
                                                     && cr.IsActive
                                               select cr).First();

                // delete the gear retrieval of the old fishing gear

                prevCatchRecord.GearExitTime = null;
                prevCatchRecord.HasGearExit = false;
            }

            // Update the catch record in the DB with the new data

            List<ShipLogBookPage> shipLogBookPages = SaveGearRetrievalData(specifiedFishingTrip, fishingActivity, prevFishingGearCatchRecord);

            return shipLogBookPages;
        }

        public List<ShipLogBookPage> MapCancel(FishingActivityType fishingActivity, IDType[] relatedReportIdentifiers)
        {
            List<ShipLogBookPage> relatedPages = new List<ShipLogBookPage>();
            FishingGearCatchRecordData prevFishingGearCatchRecord = new FishingGearCatchRecordData();

            string prevSpecifiedFishingTrip = MapGearRetrievalData(fishingActivity, relatedReportIdentifiers, ref prevFishingGearCatchRecord);

            string tripIdentifier = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
            HashSet<int> prevReferencedShipLogBookPageIds = GetShipLogBookPageIdsForTrip(tripIdentifier);

            // Clear gear retrieval data from db

            CatchRecord prevCatchRecord = (from cr in Db.CatchRecords
                                           join page in Db.ShipLogBookPages on cr.LogBookPageId equals page.Id
                                           where prevReferencedShipLogBookPageIds.Contains(cr.LogBookPageId)
                                                 && page.FishingGearRegisterId == prevFishingGearCatchRecord.FishingGearRegisterId
                                                 && cr.GearExitTime == prevFishingGearCatchRecord.CatchRecord.GearExitTime
                                                 && cr.IsActive
                                           select cr).First();

            // delete the gear retrieval of the old fishing gear

            prevCatchRecord.GearExitTime = null;
            prevCatchRecord.HasGearExit = false;

            return relatedPages;
        }

        public List<ShipLogBookPage> MapDelete(FishingActivityType fishingActivity, IDType[] relatedReportIdentifiers)
        {
            List<ShipLogBookPage> relatedPages = new List<ShipLogBookPage>();
            FishingGearCatchRecordData prevFishingGearCatchRecord = new FishingGearCatchRecordData();

            string prevSpecifiedFishingTrip = MapGearRetrievalData(fishingActivity, relatedReportIdentifiers, ref prevFishingGearCatchRecord);

            string tripIdentifier = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
            HashSet<int> prevReferencedShipLogBookPageIds = GetShipLogBookPageIdsForTrip(tripIdentifier);

            // Clear gear retrieval data from db

            CatchRecord prevCatchRecord = (from cr in Db.CatchRecords
                                           join page in Db.ShipLogBookPages on cr.LogBookPageId equals page.Id
                                           where prevReferencedShipLogBookPageIds.Contains(cr.LogBookPageId)
                                                 && page.FishingGearRegisterId == prevFishingGearCatchRecord.FishingGearRegisterId
                                                 && cr.GearExitTime == prevFishingGearCatchRecord.CatchRecord.GearExitTime
                                                 && cr.IsActive
                                           select cr).First();

            // delete the gear retrieval of the old fishing gear

            prevCatchRecord.GearExitTime = null;
            prevCatchRecord.HasGearExit = false;

            return relatedPages;
        }

        // Helper methods

        /// <summary>
        /// Maps gear retrieval data into the fishingGearCatchRecord parameter (overrides its reference if needed)
        /// </summary>
        /// <param name="fishingActivity">The fishing activity in which the gear retrieval data are stored</param>
        /// <param name="relatedReportIdentifiers">Related identifiers if the gear shot was in another trip</param>
        /// <param name="fishingGearCatchRecord">Fishing gear catch record data passed as a `ref` parameter (probably empty object at first)</param>
        /// <returns>Returns new specified fishing trip, if the gear shot was in another trip. Updates fishing gear catch record object!</returns>
        private string MapGearRetrievalData(FishingActivityType fishingActivity, IDType[] relatedReportIdentifiers, ref FishingGearCatchRecordData fishingGearCatchRecord)
        {
            string specifiedFishingTrip = null;

            if (relatedReportIdentifiers != null && relatedReportIdentifiers.Length > 0) // The current report is related to another - where the fishing gear is set probably
            {
                Guid referencedId = (Guid)relatedReportIdentifiers[0];
                FvmsfishingActivityReport fvmsRelatedActivityReport = GetDocumentByUUID(referencedId);
                FLUXFAReportMessageType faReportMessage = CommonUtils.Deserialize<FLUXFAReportMessageType>(fvmsRelatedActivityReport.ResponseMessage);

                if (faReportMessage.FAReportDocument.Length > 0 && faReportMessage.FAReportDocument[0].SpecifiedFishingActivity.Length > 0)
                {
                    specifiedFishingTrip = GetFishingTripIdentifier(faReportMessage.FAReportDocument[0].SpecifiedFishingActivity[0].SpecifiedFishingTrip);
                    MapGearRetrieval(fishingActivity,
                                     ref fishingGearCatchRecord,
                                     fishingActivity.SpecifiedFishingGear,
                                     faReportMessage.FAReportDocument[0].SpecifiedFishingActivity[0].SpecifiedFishingTrip,
                                     fishingActivity);
                }
                else
                {
                    string msg = $"Document from another trip does not have FaReports or just a specified fishing activity. For Document with id: {referencedId}";
                    LogWarning(msg, nameof(MapGearRetrievalData));
                }
            }
            else
            {
                MapGearRetrieval(fishingActivity, ref fishingGearCatchRecord, fishingActivity.SpecifiedFishingGear, null, fishingActivity);
            }

            return specifiedFishingTrip;
        }

        /// <summary>
        /// Maps gear retrieval data to DB models, updates fishing gear catch record data, if the catch was started in a previous trip
        /// </summary>
        /// <param name="subActivity">the activity, holding information about the gear retrieval</param>
        /// <param name="fishingGearCatchRecord">Fishing gear and catch record information</param>
        /// <param name="specifiedFishingGears">Gears, used in the activity (if different than those in the subActivity)</param>
        /// <param name="gearShotTripIdentifier">Pass this param if the gear shot is made in a previous trip</param>
        public void MapGearRetrieval(FishingActivityType subActivity,
                                     ref FishingGearCatchRecordData fishingGearCatchRecord,
                                     FishingGearType[] specifiedFishingGears,
                                     FishingTripType gearShotTripIdentifier,
                                     FishingActivityType fishingActivity)
        {
            int? fishingGearRegisterId = default;
            int? specifiedFishingGearRegisterId = MapSpecifiedFishingGear(specifiedFishingGears, fishingActivity);
            int? depth = (int?)GetFishingGearDepth(subActivity.SpecifiedFLUXCharacteristic);

            if (specifiedFishingGearRegisterId.HasValue)
            {
                fishingGearCatchRecord.FishingGearRegisterId = specifiedFishingGearRegisterId.Value;
            }

            DateTime gearExitDateTime;
            if (subActivity.SpecifiedDelimitedPeriod != null && subActivity.SpecifiedDelimitedPeriod.Length > 0)
            {
                gearExitDateTime = (DateTime)subActivity.SpecifiedDelimitedPeriod[0].EndDateTime;
            }
            else
            {
                gearExitDateTime = (DateTime)subActivity.OccurrenceDateTime;
            }

            if (subActivity.SpecifiedFishingGear != null) // if any, use to override the gears above
            {
                FishingGearCharacteristics gearCharacteristics = GetFluxFishingGearCharacteristics(subActivity.SpecifiedFishingGear[0].ApplicableGearCharacteristic);
                FishingGearData fishingGearData = GetFishingGearData(subActivity.SpecifiedFishingGear[0], gearCharacteristics, fishingActivity.SpecifiedFishingTrip);
                fishingGearRegisterId = fishingGearData.FishingGearRegisterId;
            }
            else if (specifiedFishingGearRegisterId.HasValue)
            {
                fishingGearRegisterId = specifiedFishingGearRegisterId;
            }

            if (gearShotTripIdentifier != null) // Вдига се различен уред от този, който е използван за улова на рибата
            {
                FishingGearCatchRecordData fishingGearCatchRecordFromPreviousTrip = new FishingGearCatchRecordData();
                fishingGearCatchRecordFromPreviousTrip.FishingGearRegisterId = fishingGearRegisterId.Value;

                fishingGearCatchRecord = UpdateGearRetrievalForCatchRecordFromPreviousTrip(gearShotTripIdentifier,
                                                                                           fishingGearCatchRecordFromPreviousTrip,
                                                                                           gearExitDateTime,
                                                                                           depth,
                                                                                           fishingGearRegisterId.Value);
            }
            else
            {
                if (fishingGearCatchRecord.CatchRecord != null)
                {
                    fishingGearCatchRecord.CatchRecord.GearExitTime = gearExitDateTime;

                    if (fishingGearCatchRecord.CatchRecord.GearExitTime.HasValue)
                    {
                        fishingGearCatchRecord.CatchRecord.HasGearExit = true;
                    }

                    fishingGearCatchRecord.CatchRecord.Depth = depth;
                }
            }
        }

        private List<ShipLogBookPage> SaveGearRetrievalData(string specifiedFishingTrip, FishingActivityType fishingActivity, FishingGearCatchRecordData fishingGearCatchRecord)
        {
            List<ShipLogBookPage> shipLogBookPages = new List<ShipLogBookPage>();

            if (fishingGearCatchRecord.FishingGearRegisterId.HasValue)
            {
                if (string.IsNullOrEmpty(specifiedFishingTrip))
                {
                    specifiedFishingTrip = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
                }

                HashSet<int> tripLogBookPageIds = GetShipLogBookPageIdsForTrip(specifiedFishingTrip);

                if (tripLogBookPageIds.Count == 0)
                {
                    throw new ArgumentException($"No related ship log book pages found for gear retrieval save data for fishing trip identifier: {specifiedFishingTrip}");
                }

                CatchRecord catchRecord = (from logBookPage in Db.ShipLogBookPages
                                           join cr in Db.CatchRecords on logBookPage.Id equals cr.LogBookPageId
                                           where tripLogBookPageIds.Contains(logBookPage.Id)
                                                 && logBookPage.FishingGearRegisterId == fishingGearCatchRecord.FishingGearRegisterId.Value
                                                 && !cr.GearExitTime.HasValue
                                                 && cr.IsActive
                                                 && logBookPage.IsActive
                                           select cr).FirstOrDefault();

                if (catchRecord == null)
                {
                    throw new ArgumentException($"No catch record found for fishing gear register id: {fishingGearCatchRecord.FishingGearRegisterId.Value} for trip identifier: {specifiedFishingTrip}");
                }

                ShipLogBookPage shipLogBookPage = GetShipLogBookPageById(catchRecord.LogBookPageId);

                shipLogBookPages.Add(shipLogBookPage);

                if (catchRecord != null)
                {
                    catchRecord.GearExitTime = fishingGearCatchRecord.CatchRecord.GearExitTime;

                    if (catchRecord.GearExitTime.HasValue)
                    {
                        catchRecord.HasGearExit = true;
                    }
                }
                else
                {
                    string msg = $"Catch record that doesn't have gearExitTime is not present in this trip: ({specifiedFishingTrip}) " +
                                 $"for a specified fishing gear register id: {fishingGearCatchRecord.FishingGearRegisterId}";
                    LogWarning(msg, "SaveGearRetrievalData");
                }
            }

            return shipLogBookPages;
        }

        /// <summary>
        /// Finds the catchRecord from another trip
        /// </summary>
        /// <param name="specifiedFishingTrip"></param>
        /// <param name="fishingGearCatchRecord"></param>
        /// <param name="gearExitDateTime"></param>
        /// <param name="fishingGearRegisterId"></param>
        /// <returns></returns>
        private FishingGearCatchRecordData UpdateGearRetrievalForCatchRecordFromPreviousTrip(FishingTripType specifiedFishingTrip,
                                                                                         FishingGearCatchRecordData fishingGearCatchRecord,
                                                                                         DateTime gearExitDateTime,
                                                                                         int? depth,
                                                                                         int fishingGearRegisterId)
        {
            string fishingTrip = GetFishingTripIdentifier(specifiedFishingTrip);
            LogWarning($"the gear retrieval reported one trip is probably from another trip: {fishingTrip}", "UpdateGearRetrievalForCatchRecordFromPreviousTrip");

            Dictionary<int, int> shipPageFishingGearRegisterIds = GetShipLogBookPageFishingGearRegisterForTrip(fishingTrip);

            if (shipPageFishingGearRegisterIds.Count == 0)
            {
                LogWarning($"No ship page fishing gear register ids found for fishing trip identifier: {fishingTrip}", "UpdateGearRetrievalForCatchRecordFromPreviousTrip");
            }

            List<int> logBookPageIds = shipPageFishingGearRegisterIds.Keys.ToList();

            CatchRecord catchRecord = (from cr in Db.CatchRecords
                                       join shipPage in Db.ShipLogBookPages on cr.LogBookPageId equals shipPage.Id
                                       where logBookPageIds.Contains(cr.LogBookPageId)
                                             && shipPage.FishingGearRegisterId == fishingGearRegisterId
                                             && !cr.HasGearExit.Value
                                             && cr.IsActive
                                             && cr.GearEntryTime < gearExitDateTime
                                       select cr).FirstOrDefault();

            if (catchRecord == null)
            {
                throw new ArgumentException($"Cannot find catch record for fishing gear register id: {fishingGearRegisterId} for trip identifier: {fishingTrip}");
            }

            catchRecord.GearExitTime = gearExitDateTime;

            if (catchRecord.GearExitTime.HasValue)
            {
                catchRecord.HasGearExit = true;
            }
            
            catchRecord.Depth = depth;
            fishingGearCatchRecord.CatchRecord = catchRecord;
            fishingGearCatchRecord.FishingTripIdentifier = fishingTrip;

            return fishingGearCatchRecord;
        }

        private Dictionary<int, int> GetShipLogBookPageFishingGearRegisterForTrip(string fishingTripIdentifier)
        {
            Dictionary<int, int> shipLogBookPageIds = (from fishingActivityReportLogBookPage in Db.FvmsfishingActivityReportLogBookPages
                                                       join shipLogBookPage in Db.ShipLogBookPages on fishingActivityReportLogBookPage.ShipLogBookPageId equals shipLogBookPage.Id
                                                       join logBook in Db.LogBooks on shipLogBookPage.LogBookId equals logBook.Id
                                                       where fishingActivityReportLogBookPage.TripIdentifier == fishingTripIdentifier
                                                             && fishingActivityReportLogBookPage.IsActive
                                                             && shipLogBookPage.IsActive
                                                             && logBook.IsActive
                                                             && shipLogBookPage.FishingGearRegisterId.HasValue
                                                       group shipLogBookPage by new
                                                       {
                                                           PageId = shipLogBookPage.Id,
                                                           GearRegisterId = shipLogBookPage.FishingGearRegisterId
                                                       } into shipPageGrouped
                                                       select new
                                                       {
                                                           shipPageGrouped.Key.PageId,
                                                           shipPageGrouped.Key.GearRegisterId
                                                       }).ToDictionary(x => x.PageId, y => y.GearRegisterId.Value);

            return shipLogBookPageIds;
        }
    }
}
