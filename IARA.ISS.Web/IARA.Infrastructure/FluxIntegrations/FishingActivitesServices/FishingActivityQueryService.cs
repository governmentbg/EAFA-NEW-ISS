using IARA.Flux.Models;
using IARA.FluxModels.Enums;
using IARA.FluxModels;
using IARA.Infrastructure.FluxIntegrations.Helpers;
using Microsoft.Extensions.Logging;
using TL.Logging.Abstractions.Interfaces;

namespace IARA.Infrastructure.FluxIntegrations.FishingActivitesServices
{
    internal class FishingActivityQueryService : BaseFishingActivityService
    {
        public FishingActivityQueryService(IARADbContext dbContext, IExtendedLogger logger) 
            : base(dbContext, logger, "FishingActivityQueryService.cs")
        { }

        public FishingActiviryQueryParameters ParseQueryParameters(FLUXFAQueryMessageType query)
        {
            FishingActiviryQueryParameters queryParams = new FishingActiviryQueryParameters();

            switch (query.FAQuery.TypeCode.Value)
            {
                case nameof(FaQueryTypeCodes.VESSEL):
                    queryParams.ForVessel = true;
                    break;
                case nameof(FaQueryTypeCodes.TRIP):
                    queryParams.ForTrip = true;
                    break;
            }

            if (queryParams.ForVessel) // there must be a Specified_Delimited_Period provided
            {
                queryParams.PeriodStartDate = (DateTime)query.FAQuery.SpecifiedDelimitedPeriod.StartDateTime;
                queryParams.PeriodEndDate = (DateTime)query.FAQuery.SpecifiedDelimitedPeriod.EndDateTime;
            }
            else // if data is for a specified fishing trip, the dates may by null
            {
                if (query.FAQuery.SpecifiedDelimitedPeriod != null && query.FAQuery.SpecifiedDelimitedPeriod.StartDateTime != null)
                {
                    queryParams.PeriodStartDate = (DateTime)query.FAQuery.SpecifiedDelimitedPeriod.StartDateTime;
                }

                if (query.FAQuery.SpecifiedDelimitedPeriod != null && query.FAQuery.SpecifiedDelimitedPeriod.EndDateTime != null)
                {
                    queryParams.PeriodEndDate = (DateTime)query.FAQuery.SpecifiedDelimitedPeriod.EndDateTime;
                }
            }

            foreach (FAQueryParameterType queryParam in query.FAQuery.SimpleFAQueryParameter)
            {
                switch (queryParam.TypeCode.Value)
                {
                    case nameof(FaQueryParameterTypeCodes.VESSELID):
                        if (queryParam.ValueID.schemeID == IDTypes.CFR)
                        {
                            queryParams.VesselCfr = queryParam.ValueID.Value;
                        }
                        else if (queryParam.ValueID.schemeID == IDTypes.IRCS)
                        {
                            queryParams.VesselIrcs = queryParam.ValueID.Value;
                        }
                        break;
                    case nameof(FaQueryParameterTypeCodes.TRIPID):
                        queryParams.TripIdentifier = queryParam.ValueID.Value;
                        break;
                    case nameof(FaQueryParameterTypeCodes.CONSOLIDATED):
                        queryParams.IsConsolidated = queryParam.ValueCode.Value == "Y";
                        break;
                }
            }

            return queryParams;
        }

        public List<FAReportDocumentType> GetRelatedFARepotDocuments(List<string> tripIdentifiers,
                                                                     bool isConsolidated,
                                                                     DateTime? fishingTripStartDate = null,
                                                                     DateTime? fishingTripEndDate = null)
        {
            List<FAReportDocumentType> faReportDocuments = new List<FAReportDocumentType>();

            // Get related messages data for specified trip identifier
            List<ReportBasicData> relatedRequestsPages = (from fluxFvmsActivityReport in Db.FvmsfishingActivityReports
                                                          join fishingActivityShipPage in Db.FvmsfishingActivityReportLogBookPages on fluxFvmsActivityReport.Id equals fishingActivityShipPage.FishingActivityReportId
                                                          where tripIdentifiers.Contains(fishingActivityShipPage.TripIdentifier)
                                                                && fluxFvmsActivityReport.IsActive
                                                                && (!fishingTripStartDate.HasValue || fluxFvmsActivityReport.CreatedOn >= fishingTripStartDate.Value)
                                                                && (!fishingTripEndDate.HasValue || fluxFvmsActivityReport.CreatedOn <= fishingTripEndDate.Value)
                                                          orderby fluxFvmsActivityReport.CreatedOn
                                                          group fluxFvmsActivityReport by new
                                                          {
                                                              fluxFvmsActivityReport.Id,
                                                              fluxFvmsActivityReport.ResponseUuid,
                                                              fluxFvmsActivityReport.ReferencedResponseUuid,
                                                              fluxFvmsActivityReport.ResponseMessage,
                                                              fluxFvmsActivityReport.MdrFluxGpPurposeId,
                                                              fluxFvmsActivityReport.CreatedOn
                                                          } into fluxReport
                                                          select new ReportBasicData
                                                          {
                                                              ActivityReportId = fluxReport.Key.Id,
                                                              FluxFvmsRequestUUId = fluxReport.Key.ResponseUuid,
                                                              ReferencedFluxFvmsRequestUUId = fluxReport.Key.ReferencedResponseUuid,
                                                              FluxFvmsRequestContent = fluxReport.Key.ResponseMessage,
                                                              GPPurposeCode = fluxReport.Key.MdrFluxGpPurposeId,
                                                              CreatedOn = fluxReport.Key.CreatedOn
                                                          }).ToList();

            if (isConsolidated) // Only the latest information about the fishing activities
            {
                var originalMessages = (from relatedRequestsPage in relatedRequestsPages
                                        where relatedRequestsPage.GPPurposeCode == (int)ReportPurposeCodes.Original
                                        select relatedRequestsPage).ToList();

                foreach (var message in originalMessages)
                {
                    var latestMessageData = GetLatestMessage(message, relatedRequestsPages);
                    string latestMessageContent = latestMessageData.FluxFvmsRequestContent;
                    FLUXFAReportMessageType report = CommonUtils.Deserialize<FLUXFAReportMessageType>(latestMessageContent);
                    faReportDocuments.AddRange(report.FAReportDocument);
                }
            }
            else // Every message counts, including: updates, cancellations and deletions
            {
                foreach (var message in relatedRequestsPages)
                {
                    FLUXFAReportMessageType report = CommonUtils.Deserialize<FLUXFAReportMessageType>(message.FluxFvmsRequestContent);
                    faReportDocuments.AddRange(report.FAReportDocument);
                }
            }

            return faReportDocuments;
        }

        public void UpdateNotificationReportDocument(HashSet<int> shipLogBookPages, FAReportDocumentType faReportDocument, string tripIdentifier)
        {
            foreach (FishingActivityType fishingActivity in faReportDocument.SpecifiedFishingActivity)
            {
                FaTypes faType;
                bool isSuccessfulFaTypeCast = Enum.TryParse<FaTypes>(fishingActivity.TypeCode.Value, out faType);
                if (isSuccessfulFaTypeCast)
                {
                    switch (faType)
                    {
                        case FaTypes.ARRIVAL:
                            UpdateFluxFAReportNotificationArrival(shipLogBookPages, fishingActivity);
                            break;
                    }
                }
                else
                {
                    LogWarning($"Unknown FLUX_FA_TYPE ({fishingActivity.TypeCode.Value}), in trip: {tripIdentifier}", "UpdateNotificationReportDocument");
                }
            }
        }

        private void UpdateFluxFAReportNotificationArrival(HashSet<int> shipLogBookPages, FishingActivityType fishingActivity)
        {
            DateTime now = DateTime.Now;

            int shipLogBookPageId = shipLogBookPages.First();
            var shipLogBookPageData = (from logBookPage in Db.ShipLogBookPages
                                       where logBookPage.Id == shipLogBookPageId
                                       select new
                                       {
                                           ArrivalPortId = logBookPage.ArrivePortId,
                                           ArrivalDateTime = logBookPage.FishTripEndDateTime
                                       }).First();

            // Update arrival port
            foreach (FLUXLocationType fluxLocation in fishingActivity.RelatedFLUXLocation)
            {
                if (fluxLocation.TypeCode != null)
                {
                    if (fluxLocation.TypeCode.Value == nameof(FluxLocationTypes.LOCATION))
                    {
                        if (fluxLocation.ID != null && fluxLocation.ID.schemeID == nameof(FluxLocationIdentifierTypes.LOCATION))
                        {
                            if (shipLogBookPageData.ArrivalPortId.HasValue && shipLogBookPageData.ArrivalPortId.Value != -1)
                            {
                                string portCode = (from port in Db.Nports
                                                   where port.Id == shipLogBookPageData.ArrivalPortId.Value
                                                         && port.ValidFrom <= now
                                                         && port.ValidTo > now
                                                   select port.Code).First();

                                fluxLocation.ID.Value = portCode;
                            }
                        }
                    }
                }
            }

            // Update notification of arrival date time
            // (should update only the declaration of arrival dateTime, but we don't receive these messages...)
            if (shipLogBookPageData.ArrivalDateTime.HasValue)
            {
                fishingActivity.OccurrenceDateTime = DateTimeType.BuildDateTime(shipLogBookPageData.ArrivalDateTime.Value);
            }
        }

        /// <summary>
        /// Looks for a message, which reffered to the parameter `message`
        /// </summary>
        /// <param name="message">The original message</param>
        /// <param name="allMessages">Lookup with all messages and their related ship log book page ids</param>
        /// <returns>the latest message of an original one, otherwise - the original message itself</returns>
        private ReportBasicData GetLatestMessage(ReportBasicData message, List<ReportBasicData> allMessages)
        {
            ReportBasicData referencedMessage = allMessages.Where(x => x.ReferencedFluxFvmsRequestUUId == message.FluxFvmsRequestUUId)
                                                           .OrderByDescending(x => x.CreatedOn)
                                                           .First();

            if (referencedMessage != null) // There was some kind of modification to the original message
            {
                return referencedMessage;
            }
            else
            {
                return message;
            }
        }
    }
}
