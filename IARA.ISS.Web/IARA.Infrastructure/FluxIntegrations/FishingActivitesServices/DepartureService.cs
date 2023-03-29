using IARA.Common.Resources;
using IARA.Flux.Models;
using IARA.FluxModels.Enums;
using IARA.Infrastructure.FluxIntegrations.FishingActivitesServices.Models;
using IARA.Infrastructure.FluxIntegrations.Helpers;
using Microsoft.Extensions.Logging;
using TL.Logging.Abstractions.Interfaces;

namespace IARA.Infrastructure.FluxIntegrations.FishingActivitesServices
{
    internal class DepartureService : BaseFishingActivityService
    {
        public DepartureService(IARADbContext dbContext, IExtendedLogger logger)
            : base(dbContext, logger, "DepartureService.cs")
        { }

        public List<ShipLogBookPage> MapOriginal(FishingActivityType fishingActivity, VesselTransportMeansType specifiedVesselTransportMeans, IDType[] relatedReportIds = null)
        {
            List<int> shipIds = GetShipIds(specifiedVesselTransportMeans);

            FaDepartureReasonCodes departureReasonCode;
            List<ShipLogBookPage> newLogBookPages = new List<ShipLogBookPage>();

            bool successfulCast = Enum.TryParse<FaDepartureReasonCodes>(fishingActivity.ReasonCode.Value, out departureReasonCode);
            if (successfulCast && departureReasonCode == FaDepartureReasonCodes.FIS) // a fishing trip is started and log book page(s) must be added
            {
                foreach (FishingGearType fishingGear in fishingActivity.SpecifiedFishingGear) // Add log book page for each fishing gear on board
                {
                    ShipLogBookPage page = AddLogBookPagesByFishingGears(new AddShipLogBookPageParameters
                                                                         {
                                                                             FishingActivity = fishingActivity,
                                                                             FishingGear = fishingGear,
                                                                             ShipIds = shipIds
                                                                         });
                    newLogBookPages.Add(page);
                }
            }
            else
            {
                LogWarning($"Unexpected reparture reason code in departure message: {fishingActivity.ReasonCode.Value}", "MapOriginal");
            }

            return newLogBookPages;
        }

        public List<ShipLogBookPage> MapUpdate(FishingActivityType fishingActivity, IDType referenceId, IDType[] relatedReportsIdentifiers, VesselTransportMeansType specifiedVesselTransportMeans)
        {
            List<int> shipIds = GetShipIds(specifiedVesselTransportMeans);
            List<ShipLogBookPage> relatedPages = GetTripRelatedLogBookPages(fishingActivity);

            int portId = GetPortId(fishingActivity.RelatedFLUXLocation);
            DateTime fishTripStartDateTime = (DateTime)fishingActivity.OccurrenceDateTime;

            HashSet<int> updatedLogBookPages = new HashSet<int>();
            List<ShipLogBookPage> newLogBookPages = new List<ShipLogBookPage>();

            foreach (FishingGearType fishingGear in fishingActivity.SpecifiedFishingGear)
            {
                DepartureData departureData = GetDepartureData(new AddShipLogBookPageParameters
                {
                    FishingActivity = fishingActivity,
                    FishingGear = fishingGear,
                    ShipIds = shipIds
                });

                ShipLogBookPage page = relatedPages.Find(x => x.LogBookId == departureData.LogBookId 
                                                              && x.LogBookPermitLicenceId == departureData.LogBookPermitLicenseId
                                                              && x.FishingGearRegisterId == departureData.FishingGearRegisterId);

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
                    ShipLogBookPage newLogBookPage = CreateNewLogBookPage((DateTime)fishingActivity.OccurrenceDateTime, departureData, portId);
                    newLogBookPages.Add(newLogBookPage);
                }
            }

            List<ShipLogBookPage> logBookPagesToCancel = relatedPages.Where(x => !updatedLogBookPages.Contains(x.Id)).ToList();

            foreach (ShipLogBookPage logBookPage in logBookPagesToCancel)
            {
                logBookPage.Status = nameof(LogBookPageStatusesEnum.Canceled);
                logBookPage.CancelationReason = AppResources.shipAnnulFluxDepartureUpdateReason;
            }

            Db.ShipLogBookPages.AddRange(newLogBookPages);

            return relatedPages;
        }

        public List<ShipLogBookPage> MapCancel(IDType referenceId)
        {
            // Get related ship log book pages
            List<ShipLogBookPage> relatedPages = GetReferenceIdRelatedLogBookPages((Guid)referenceId);
            
            // Cancel all pages, started from this departure message

            foreach (var page in relatedPages)
            {
                page.Status = nameof(LogBookPageStatusesEnum.Canceled);
                page.CancelationReason = AppResources.shipAnnulFluxDepartureCancelReason;
            }

            return relatedPages;
        }

        public List<ShipLogBookPage> DeleteFluxFAReportDeclarationDeparture(IDType referenceId)
        {
            // Get related ship log book pages
            List<ShipLogBookPage> relatedPages = GetReferenceIdRelatedLogBookPages((Guid)referenceId);

            // Delete all related log book pages

            foreach (var page in relatedPages)
            {
                page.IsActive = false;
            }

            return relatedPages;
        }

        // Helper methods

        private ShipLogBookPage AddLogBookPagesByFishingGears(AddShipLogBookPageParameters parameters)
        {
            int portId = GetPortId(parameters.FishingActivity.RelatedFLUXLocation);

            DepartureData departureData = GetDepartureData(parameters);

            ShipLogBookPage newLogBookPage = CreateNewLogBookPage((DateTime)parameters.FishingActivity.OccurrenceDateTime, departureData, portId);

            Db.ShipLogBookPages.Add(newLogBookPage);

            return newLogBookPage;
        }

        private DepartureData GetDepartureData(AddShipLogBookPageParameters parameters)
        {
            DateTime now = DateTime.Now;

            FishingGearCharacteristics gearCharacteristics = GetFluxFishingGearCharacteristics(parameters.FishingGear.ApplicableGearCharacteristic);
            FishingGearData fishingGearData = GetFishingGearData(parameters.FishingGear, gearCharacteristics, parameters.FishingActivity.SpecifiedFishingTrip);

            int permitLicenseId = GetPermitLicenseId(gearCharacteristics.PermitLicenseNumber);
            HashSet<int> logBookPermitLicenseIds = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                                    join logBookPermitLicense in Db.LogBookPermitLicenses on permitLicense.Id equals logBookPermitLicense.PermitLicenseRegisterId
                                                    where permitLicense.Id == permitLicenseId
                                                          && parameters.ShipIds.Contains(permitLicense.ShipId)
                                                          && logBookPermitLicense.LogBookValidFrom <= now
                                                          && logBookPermitLicense.LogBookValidTo > now
                                                    orderby logBookPermitLicense.LogBookValidTo descending
                                                    select logBookPermitLicense.Id).ToHashSet();

            string fishingTrip = GetFishingTripIdentifier(parameters.FishingActivity.SpecifiedFishingTrip);
            if (logBookPermitLicenseIds.Count > 1)
            {
                string msg = $"found: {logBookPermitLicenseIds.Count} log book permit license id in our DB for fishing trip identifier: {fishingTrip} and fishing gear register id: {fishingGearData.FishingGearRegisterId}";
                LogWarning(msg, "GetDepartureData");
            }
            else if (logBookPermitLicenseIds.Count == 0)
            {
                throw new ArgumentException($"No log book permit license found for fishing trip identifier: {fishingTrip} and fishing gear register id: {fishingGearData.FishingGearRegisterId}");
            }

            int logBookPermitLicenseId = logBookPermitLicenseIds.First();

            HashSet<int> logBookIds = (from logBookPermitLicense in Db.LogBookPermitLicenses
                                       where logBookPermitLicense.Id == logBookPermitLicenseId
                                       orderby logBookPermitLicense.LogBookValidTo descending
                                       select logBookPermitLicense.LogBookId).ToHashSet();

            if (logBookIds.Count != 1)
            {
                string msg = $"found: {logBookIds.Count} log book ids in our DB (in LogBookPermitLicenses table) for fishing trip: {fishingTrip} and fishing gear id: {fishingGearData.FishingGearRegisterId}";
                LogWarning(msg, "GetDepartureData");
            }
            else if (logBookIds.Count == 0)
            {
                throw new ArgumentException($"No log book ids found for fishing trip identifier: {fishingTrip} and fishing gear register id: {fishingGearData.FishingGearRegisterId}");
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

        private ShipLogBookPage CreateNewLogBookPage(DateTime occurrenceDate, DepartureData departureData, int? portId)
        {
            string pageNumber = GenerateOnlineLogBookPageNumber(departureData.LogBookId);

            ShipLogBookPage newLogBookPage = new ShipLogBookPage
            {
                PageFillDate = occurrenceDate,
                DepartPortId = portId,
                FishingGearRegisterId = departureData.FishingGearRegisterId,
                FishingGearCount = departureData.FishingGearCount,
                FishingGearHooksCount = departureData.FishingGearHookCount,
                FishTripStartDateTime = occurrenceDate,
                LogBookId = departureData.LogBookId,
                LogBookPermitLicenceId = departureData.LogBookPermitLicenseId,
                PageNum = pageNumber,
                Status = nameof(LogBookPageStatusesEnum.InProgress)
            };

            return newLogBookPage;
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
}
