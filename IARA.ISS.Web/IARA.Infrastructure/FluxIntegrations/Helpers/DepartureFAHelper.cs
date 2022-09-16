using System;
using System.Linq;
using System.Collections.Generic;
using IARA.EntityModels.Entities;
using IARA.Flux.Models;
using IARA.FluxModels.Enums;
using IARA.Common.Enums;
using IARA.Common.Resources;

namespace IARA.Infrastructure.FluxIntegrations.Helpers
{
    internal partial class FishingActivityHelper
    {
        private List<ShipLogBookPage> MapFluxFAReportDeclarationDeparture(FishingActivityType fishingActivity, List<int> shipIds)
        {
            FaDepartureReasonCodes departureReasonCode;
            List<ShipLogBookPage> newLogBookPages = new List<ShipLogBookPage>();
            string fishingTrip = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);

            bool successfulCast = Enum.TryParse<FaDepartureReasonCodes>(fishingActivity.ReasonCode.Value, out departureReasonCode);

            if (successfulCast && departureReasonCode == FaDepartureReasonCodes.FIS) // a fishing trip is started (maybe?) and log book page(s) must be added
            {
                int? portId = GetPortId(fishingActivity.RelatedFLUXLocation);

                if (!portId.HasValue)
                {
                    Logger.LogWarning($"{LOGGER_MSG_TYPE} Unknown departure port, in trip: {fishingTrip}");
                }

                foreach (FishingGearType fishingGear in fishingActivity.SpecifiedFishingGear) // Add log book page for each fishing gear
                {
                    newLogBookPages.Add(AddLogBookPagesByFishingGears(fishingActivity, fishingGear, shipIds, portId));
                }
            }
            else
            {
                Logger.LogWarning($"{LOGGER_MSG_TYPE} Unexpected reparture reason code in departure message: {fishingActivity.ReasonCode.Value}");
            }

            return newLogBookPages;
        }

        private List<ShipLogBookPage> UpdateFluxFAReportDeclarationDeparture(FishingActivityType fishingActivity, List<int> shipIds)
        {
            string fishingTrip = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
            HashSet<int> relatedLogBookPageIds = GetShipLogBookPageIdsForTrip(fishingTrip);
            List<ShipLogBookPage> relatedPages = (from logBookPage in Db.ShipLogBookPages
                                                  where relatedLogBookPageIds.Contains(logBookPage.Id)
                                                  select logBookPage).ToList();

            int? portId = GetPortId(fishingActivity.RelatedFLUXLocation);
            DateTime fishTripStartDateTime = (DateTime)fishingActivity.OccurrenceDateTime;

            if (!portId.HasValue)
            {
                Logger.LogWarning($"{LOGGER_MSG_TYPE} Unknown departure port, in trip: {fishingTrip}");
            }

            HashSet<int> updatedLogBookPages = new HashSet<int>();
            List<ShipLogBookPage> newLogBookPages = new List<ShipLogBookPage>();

            foreach (FishingGearType fishingGear in fishingActivity.SpecifiedFishingGear)
            {
                DepartureData departureData = GetDepartureData(fishingActivity, fishingGear, shipIds);
                ShipLogBookPage page = relatedPages.Find(x => x.LogBookId == departureData.LogBookId && x.LogBookPermitLicenceId == departureData.LogBookPermitLicenseId);

                if (page != null) // the log book is the same, so only update data
                {
                    page.DepartPortId = portId;
                    page.FishTripStartDateTime = fishTripStartDateTime;
                    page.FishingGearRegisterId = departureData.FishingGearRegisterId;
                    page.FishingGearCount = departureData.FishingGearCount;
                    page.FishingGearHooksCount = departureData.FishingGearHookCount;

                    updatedLogBookPages.Add(page.Id);
                }
                else // the log book has changed, so new log book page
                {
                    ShipLogBookPage newLogBookPage = CreateNewLogBookPage(fishingActivity, departureData, portId);
                    newLogBookPages.Add(newLogBookPage);
                }
            }

            List<ShipLogBookPage> logBookPagesToCancel = relatedPages.Where(x => !updatedLogBookPages.Contains(x.Id)).ToList();

            foreach (ShipLogBookPage logBookPage in logBookPagesToCancel)
            {
                logBookPage.Status = nameof(LogBookPageStatusesEnum.Canceled);
                logBookPage.CancelationReason = AppResources.ShipAnnulFluxUpdateReason;
            }

            Db.ShipLogBookPages.AddRange(newLogBookPages);

            return relatedPages;
        }

        private List<ShipLogBookPage> CancelFluxFAReportDeclarationDeparture(IDType referenceId)
        {
            // Get related ship log book pages

            FvmsfishingActivityReport referencedReport = GetDocumentByUUID((Guid)referenceId);
            HashSet<int> referencedShipLogBookPageIds = GetRelatedShipLogBookPagesByFVMSFAReportId(referencedReport.Id);
            List<ShipLogBookPage> relatedPages = (from logBookPage in Db.ShipLogBookPages
                                                  where referencedShipLogBookPageIds.Contains(logBookPage.Id)
                                                  select logBookPage).ToList();

            // Cancel all pages, started from this departure message

            foreach (var page in relatedPages)
            {
                page.Status = nameof(LogBookPageStatusesEnum.Canceled);
            }

            return relatedPages;
        }

        private List<ShipLogBookPage> DeleteFluxFAReportDeclarationDeparture(IDType referenceId)
        {
            // Get related ship log book pages

            FvmsfishingActivityReport referencedReport = GetDocumentByUUID((Guid)referenceId);
            HashSet<int> referencedShipLogBookPageIds = GetRelatedShipLogBookPagesByFVMSFAReportId(referencedReport.Id);
            List<ShipLogBookPage> relatedPages = (from logBookPage in Db.ShipLogBookPages
                                                  where referencedShipLogBookPageIds.Contains(logBookPage.Id)
                                                  select logBookPage).ToList();

            // Delete all related log book pages

            foreach (var page in relatedPages)
            {
                page.IsActive = false;
            }

            return relatedPages;
        }

        private ShipLogBookPage AddLogBookPagesByFishingGears(FishingActivityType fishingActivity, FishingGearType fishingGear, List<int> shipIds, int? portId)
        {
            DepartureData departureData = GetDepartureData(fishingActivity, fishingGear, shipIds);

            ShipLogBookPage newLogBookPage = CreateNewLogBookPage(fishingActivity, departureData, portId);

            Db.ShipLogBookPages.Add(newLogBookPage);

            return newLogBookPage;
        }

        private DepartureData GetDepartureData(FishingActivityType fishingActivity, FishingGearType fishingGear, List<int> shipIds)
        {
            DateTime now = DateTime.Now;

            FishingGearCharacteristics gearCharacteristics = GetFluxFishingGearCharacteristics(fishingGear.ApplicableGearCharacteristic);
            FishingGearDataHelper fishingGearData = GetFishingGearData(fishingGear, gearCharacteristics);

            int permitLicenseId = GetPermitLicenseId(gearCharacteristics.PermitLicenseNumber);
            HashSet<int> logBookPermitLicenseIds = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                                    join logBookPermitLicense in Db.LogBookPermitLicenses on permitLicense.Id equals logBookPermitLicense.PermitLicenseRegisterId
                                                    where permitLicense.Id == permitLicenseId
                                                          && shipIds.Contains(permitLicense.ShipId)
                                                          && logBookPermitLicense.LogBookValidFrom <= now
                                                          && logBookPermitLicense.LogBookValidTo > now
                                                    orderby logBookPermitLicense.LogBookValidTo descending
                                                    select logBookPermitLicense.Id).ToHashSet();

            string fishingTrip = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
            if (logBookPermitLicenseIds.Count != 1)
            {
                string msg = $"{LOGGER_MSG_TYPE} found more than one log book permit license id in our DB for fishing trip: {fishingTrip} and fishing gear id: {fishingGearData.FishingGearRegisterId}";
                Logger.LogWarning(msg);
            }

            int logBookPermitLicenseId = logBookPermitLicenseIds.First();

            HashSet<int> logBookIds = (from logBookPermitLicense in Db.LogBookPermitLicenses
                                       where logBookPermitLicense.Id == logBookPermitLicenseId
                                       orderby logBookPermitLicense.LogBookValidTo descending
                                       select logBookPermitLicense.LogBookId).ToHashSet();

            if (logBookIds.Count != 1)
            {
                string msg = $"{LOGGER_MSG_TYPE} found more than one log book id in our DB (in LogBookPermitLicenses table) for fishing trip: {fishingTrip} and fishing gear id: {fishingGearData.FishingGearRegisterId}";
                Logger.LogWarning(msg);
            }

            int logBookId = logBookIds.First();

            DepartureData result = new DepartureData
            {
                FishingGearCount = gearCharacteristics.GearCount ?? 1,
                FishingGearHookCount = fishingGearData.HasHooks ? gearCharacteristics.HooksCount : default,
                FishingGearRegisterId = fishingGearData.FishingGearRegisterId,
                LogBookId = logBookId,
                LogBookPermitLicenseId = logBookPermitLicenseId
            };

            return result;
        }

        private ShipLogBookPage CreateNewLogBookPage(FishingActivityType fishingActivity, DepartureData departureData, int? portId)
        {
            string pageNumber = GenerateOnlineLogBookPageNumber(departureData.LogBookId);

            ShipLogBookPage newLogBookPage = new ShipLogBookPage
            {
                PageFillDate = (DateTime)fishingActivity.OccurrenceDateTime,
                DepartPortId = portId,
                FishingGearRegisterId = departureData.FishingGearRegisterId,
                FishingGearCount = departureData.FishingGearCount,
                FishingGearHooksCount = departureData.FishingGearHookCount,
                FishTripStartDateTime = (DateTime)fishingActivity.OccurrenceDateTime,
                LogBookId = departureData.LogBookId,
                LogBookPermitLicenceId = departureData.LogBookPermitLicenseId,
                PageNum = pageNumber,
                Status = nameof(LogBookPageStatusesEnum.InProgress)
            };

            return newLogBookPage;
        }

        private int GetPermitLicenseId(string permitLicenseNumber)
        {
            int permitLicenseId = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                   where permitLicense.RegistrationNum == permitLicenseNumber
                                         && permitLicense.IsActive
                                         && permitLicense.RecordType == nameof(RecordTypesEnum.Register)
                                   select permitLicense.Id).Single();

            return permitLicenseId;
        }

        private string GenerateOnlineLogBookPageNumber(int logBookId)
        {
            LogBook logBook = (from lb in Db.LogBooks
                               where lb.Id == logBookId
                               select lb).Single();

            logBook.LastPageNum++;
            string logBookPageNumber = $"{logBook.LogNum}-{logBook.LastPageNum.ToString("D5")}";

            return logBookPageNumber;
        }
    }

    internal class DepartureData
    {
        public int FishingGearRegisterId { get; set; }

        public int? FishingGearHookCount { get; set; }

        public int? FishingGearCount { get; set; }

        public int LogBookId { get; set; }

        public int LogBookPermitLicenseId { get; set; }
    }
}
