using System;
using System.Linq;
using System.Collections.Generic;
using IARA.EntityModels.Entities;
using IARA.Flux.Models;
using TL.SysToSysSecCom;
using IARA.FluxModels.Enums;

namespace IARA.Infrastructure.FluxIntegrations.Helpers
{
    internal partial class FishingActivityHelper
    {
        private List<ShipLogBookPage> MapFluxFAReportDeclarationGearRetrieval(IDType[] relatedReportIdentifiers, FishingActivityType fishingActivity)
        {
            FishingGearCatchRecord fishingGearCatchRecord = new FishingGearCatchRecord();

            string specifiedFishingTrip = MapGearRetrievalData(fishingActivity, relatedReportIdentifiers, ref fishingGearCatchRecord);

            List<ShipLogBookPage> shipLogBookPages = SaveGearRetrievalData(specifiedFishingTrip, fishingActivity, fishingGearCatchRecord);

            return shipLogBookPages;
        }

        private List<ShipLogBookPage> UpdateFluxFAReportDeclarationGearRetrieval(FishingActivityType fishingActivity, IDType referenceId, IDType[] relatedReportIdentifiers)
        {
            // Map previous gear retrieval fishing activity report data
            FishingGearCatchRecord prevFishingGearCatchRecord = new FishingGearCatchRecord();
            FvmsfishingActivityReport referencedReport = GetDocumentByUUID((Guid)referenceId);
            HashSet<int> prevReferencedShipLogBookPageIds = GetRelatedShipLogBookPagesByFVMSFAReportId(referencedReport.Id);
            FLUXFAReportMessageType referencedMessage = CommonUtils.JsonDeserialize<FLUXFAReportMessageType>(referencedReport.ResponseMessage);
            FishingActivityType prevFishingActivity = referencedMessage.FAReportDocument.SelectMany(x => x.SpecifiedFishingActivity)
                                                                                        .Where(x => x.TypeCode.Value == nameof(FaTypes.GEAR_RETRIEVAL))
                                                                                        .First();
            IDType[] prevRelatedReportIdentifiers = referencedMessage.FAReportDocument.Where(x => x.SpecifiedFishingActivity
                                                                                                   .Any(y => y.SpecifiedFishingTrip == prevFishingActivity.SpecifiedFishingTrip))
                                                                                      .SelectMany(x => x.RelatedReportID)
                                                                                      .ToArray();
            string prevSpecifiedFishingTrip = MapGearRetrievalData(prevFishingActivity, prevRelatedReportIdentifiers, ref prevFishingGearCatchRecord);

            // Map fishingGearCatchRecord object with data for the gear retrieval from the new activity

            FishingGearCatchRecord fishingGearCatchRecord = new FishingGearCatchRecord();
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

                prevCatchRecord.GearExitTime = null; // delete the gear retrieval of the old fishing gear
            }

            // Update the catch record in the DB with the new data

            List<ShipLogBookPage> shipLogBookPages = SaveGearRetrievalData(specifiedFishingTrip, fishingActivity, prevFishingGearCatchRecord);

            return shipLogBookPages;
        }

        private List<ShipLogBookPage> CancelFluxFAReportDeclarationGearRetrieval(FishingActivityType fishingActivity, IDType[] relatedReportIdentifiers)
        {
            List<ShipLogBookPage> relatedPages = new List<ShipLogBookPage>();
            FishingGearCatchRecord prevFishingGearCatchRecord = new FishingGearCatchRecord();

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

            prevCatchRecord.GearExitTime = null; // delete the gear retrieval of the old fishing gear

            return relatedPages;
        }

        private List<ShipLogBookPage> DeleteFluxFAReportDeclarationGearRetrieval(FishingActivityType fishingActivity, IDType[] relatedReportIdentifiers)
        {
            List<ShipLogBookPage> relatedPages = new List<ShipLogBookPage>();
            FishingGearCatchRecord prevFishingGearCatchRecord = new FishingGearCatchRecord();

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

            prevCatchRecord.GearExitTime = null; // delete the gear retrieval of the old fishing gear

            return relatedPages;
        }

        /// <summary>
        /// Maps gear retrieval data into the fishingGearCatchRecord parameter (overrides its reference if needed)
        /// </summary>
        /// <param name="fishingActivity">The fishing activity in which the gear retrieval data are stored</param>
        /// <param name="relatedReportIdentifiers">Related identifiers if the gear shot was in another trip</param>
        /// <param name="fishingGearCatchRecord">Fishing gear catch record data passed as a `ref` parameter (probably empty object at first)</param>
        /// <returns>Returns new specified fishing trip, if the gear shot was in another trip. Updates fishing gear catch record object!</returns>
        private string MapGearRetrievalData(FishingActivityType fishingActivity, IDType[] relatedReportIdentifiers, ref FishingGearCatchRecord fishingGearCatchRecord)
        {
            string specifiedFishingTrip = null;

            if (relatedReportIdentifiers != null && relatedReportIdentifiers.Length > 0) // The current report is related to another - where the fishing gear is set probably
            {
                Guid referencedId = (Guid)relatedReportIdentifiers[0];
                FvmsfishingActivityReport fvmsRelatedActivityReport = GetDocumentByUUID(referencedId);
                FLUXFAReportMessageType faReportMessage = CommonUtils.JsonDeserialize<FLUXFAReportMessageType>(fvmsRelatedActivityReport.ResponseMessage);

                if (faReportMessage.FAReportDocument.Length > 0 && faReportMessage.FAReportDocument[0].SpecifiedFishingActivity.Length > 0)
                {
                    specifiedFishingTrip = GetFishingTripIdentifier(faReportMessage.FAReportDocument[0].SpecifiedFishingActivity[0].SpecifiedFishingTrip);
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

            return specifiedFishingTrip;
        }

        private List<ShipLogBookPage> SaveGearRetrievalData(string specifiedFishingTrip, FishingActivityType fishingActivity, FishingGearCatchRecord fishingGearCatchRecord)
        {
            List<ShipLogBookPage> shipLogBookPages = new List<ShipLogBookPage>();

            if (fishingGearCatchRecord.FishingGearRegisterId.HasValue)
            {
                if (!string.IsNullOrEmpty(specifiedFishingTrip))
                {
                    specifiedFishingTrip = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
                }

                HashSet<int> tripLogBookPageIds = GetShipLogBookPageIdsForTrip(specifiedFishingTrip);

                CatchRecord catchRecord = (from logBookPage in Db.ShipLogBookPages
                                           join cr in Db.CatchRecords on logBookPage.Id equals cr.LogBookPageId
                                           where tripLogBookPageIds.Contains(logBookPage.Id)
                                                 && logBookPage.FishingGearRegisterId == fishingGearCatchRecord.FishingGearRegisterId.Value
                                                 && !cr.GearExitTime.HasValue
                                                 && cr.IsActive
                                                 && logBookPage.IsActive
                                           select cr).FirstOrDefault();

                ShipLogBookPage shipLogBookPage = GetShipLogBookPageById(catchRecord.LogBookPageId);

                shipLogBookPages.Add(shipLogBookPage);

                if (catchRecord != null)
                {
                    catchRecord.GearExitTime = fishingGearCatchRecord.CatchRecord.GearExitTime;
                }
                else
                {
                    string msg = $"{LOGGER_MSG_TYPE} Catch record that doesn't have gearExitTime is not present in this trip: ({specifiedFishingTrip}) " +
                                 $"for a specified fishing gear register id: {fishingGearCatchRecord.FishingGearRegisterId}";
                    Logger.LogWarning(msg);
                }
            }

            return shipLogBookPages;
        }

        /// <summary>
        /// Maps gear retrieval data to DB models, updates fishing gear catch record data, if the catch was started in a previous trip
        /// </summary>
        /// <param name="subActivity">the activity, holding information about the gear retrieval</param>
        /// <param name="fishingGearCatchRecord">Fishing gear and catch record information</param>
        /// <param name="specifiedFishingGears">Gears, used in the activity (if different than those in the subActivity)</param>
        /// <param name="gearShotTripIdentifier">Pass this param if the gear shot is made in a previous trip</param>
        private void MapGearRetrieval(FishingActivityType subActivity,
                                     ref FishingGearCatchRecord fishingGearCatchRecord,
                                     FishingGearType[] specifiedFishingGears,
                                     FishingTripType gearShotTripIdentifier)
        {
            int? fishingGearRegisterId = default;
            int? specifiedFishingGearRegisterId = MapSpecifiedFishingGear(specifiedFishingGears);
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

            if (subActivity.SpecifiedFishingGear != null)
            {
                FishingGearCharacteristics gearCharacteristics = GetFluxFishingGearCharacteristics(subActivity.SpecifiedFishingGear[0].ApplicableGearCharacteristic);
                FishingGearDataHelper fishingGearData = GetFishingGearData(subActivity.SpecifiedFishingGear[0], gearCharacteristics);
                fishingGearRegisterId = fishingGearData.FishingGearRegisterId;
            }
            else if (specifiedFishingGearRegisterId.HasValue)
            {
                fishingGearRegisterId = specifiedFishingGearRegisterId;
            }

            if (gearShotTripIdentifier != null) // Вдига се различен уред от този, който е използван за улова на рибата
            {
                FishingGearCatchRecord fishingGearCatchRecordFromPreviousTrip = new FishingGearCatchRecord();
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
                    fishingGearCatchRecord.CatchRecord.Depth = depth;
                }
            }
        }

        /// <summary>
        /// Finds the catchRecord from another trip
        /// </summary>
        /// <param name="specifiedFishingTrip"></param>
        /// <param name="fishingGearCatchRecord"></param>
        /// <param name="gearExitDateTime"></param>
        /// <param name="fishingGearRegisterId"></param>
        /// <returns></returns>
        private FishingGearCatchRecord UpdateGearRetrievalForCatchRecordFromPreviousTrip(FishingTripType specifiedFishingTrip,
                                                                                         FishingGearCatchRecord fishingGearCatchRecord,
                                                                                         DateTime gearExitDateTime,
                                                                                         int? depth,
                                                                                         int fishingGearRegisterId)
        {
            string fishingTrip = GetFishingTripIdentifier(specifiedFishingTrip);
            Logger.LogWarning($"{LOGGER_MSG_TYPE} the gear retrieval reported one trip is probably from another trip: {fishingTrip}");

            Dictionary<int, int> shipPageFishingGearRegisterIds = GetShipLogBookPageFishingGearRegisterForTrip(fishingTrip);
            CatchRecord catchRecord = (from cr in Db.CatchRecords
                                       where shipPageFishingGearRegisterIds.ContainsKey(cr.LogBookPageId)
                                             && !cr.GearExitTime.HasValue
                                             && cr.IsActive
                                             && shipPageFishingGearRegisterIds[cr.LogBookPageId] == fishingGearRegisterId
                                             && cr.GearEntryTime < gearExitDateTime
                                       select cr).First();

            catchRecord.GearExitTime = gearExitDateTime;
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
                                                       select new
                                                       {
                                                           fishingActivityReportLogBookPage.ShipLogBookPageId,
                                                           shipLogBookPage.FishingGearRegisterId
                                                       }).ToDictionary(x => x.ShipLogBookPageId, y => y.FishingGearRegisterId.Value);

            return shipLogBookPageIds;
        }


    }
}
