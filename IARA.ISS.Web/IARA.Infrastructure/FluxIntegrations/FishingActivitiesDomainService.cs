using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using IARA.Common.Enums;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.CatchesAndSales;
using IARA.EntityModels.Entities;
using IARA.Flux.Models;
using IARA.FluxModels;
using IARA.FluxModels.Enums;
using IARA.Infrastructure.FluxIntegrations.Helpers;
using IARA.Infrastructure.FluxIntegrations.Interfaces;
using IARA.Interfaces.Flux;
using IARA.Logging.Abstractions.Interfaces;
using TL.SysToSysSecCom;

namespace IARA.Infrastructure.FluxIntegrations
{
    public class FishingActivitiesDomainService : BaseService, IFishingActivitiesDomainService
    {
        private readonly IFluxFishingActivitiesReceiverService faQueueService;
        private readonly FishingActivityHelper helper;
        private readonly FishingActivityQueryHelper queryHelper;
        private readonly FishingActivityCreationHelper createFAHelper;

        private IExtendedLogger Logger { get; set; }

        private const string LOGGER_MSG_TYPE = "FLUX FA DOMAIN:";

        public FishingActivitiesDomainService(IARADbContext dbContext, IExtendedLogger logger, IFluxFishingActivitiesReceiverService faQueueService)
            : base(dbContext)
        {
            this.faQueueService = faQueueService;
            helper = new FishingActivityHelper(dbContext, logger);
            queryHelper = new FishingActivityQueryHelper(dbContext, logger);
            createFAHelper = new FishingActivityCreationHelper(dbContext, logger, helper);
            Logger = logger;
        }

        public FLUXFAReportMessageType ProcessFluxFAQuery(FLUXFAQueryMessageType query)
        {
            DateTime now = DateTime.Now;
            FLUXFAReportMessageType result = new FLUXFAReportMessageType();

            FishingActiviryQueryParameters queryParams = queryHelper.ParseQueryParameters(query);

            // Creating Flux Report Document

            result.FLUXReportDocument = new FLUXReportDocumentType
            {
                // ID -> should be set later before sending the message
                CreationDateTime = now,
                ReferencedID = query.FAQuery.ID,
                PurposeCode = CodeType.CreatePurpose(ReportPurposeCodes.Original),
                OwnerFLUXParty = new FLUXPartyType
                {
                    ID = new IDType[] { IDType.CreateParty(CountryCodes.BGR.ToString()) },
                    Name = TextType.CreateMultiText("Bulgaria")
                }
            };

            // Filling relevant fishing activity report documents

            List<FAReportDocumentType> faReportDocuments = new List<FAReportDocumentType>();

            if (queryParams.ForTrip) // All fishing activities for the specified fishing trip, ordered by date of occurrence ascending
            {
                faReportDocuments = queryHelper.GetRelatedFARepotDocuments(
                    new List<string>
                    {
                        queryParams.TripIdentifier
                    },
                    queryParams.IsConsolidated,
                    queryParams.PeriodStartDate,
                    queryParams.PeriodEndDate);
            }
            else if (queryParams.ForVessel) // All fishing activities from all trips for the specified vessel, ordered by date of occurrence ascending
            {
                // Get all related vessel ids

                List<int> relatedVesselIds = (from ship in Db.ShipsRegister
                                              where (string.IsNullOrEmpty(queryParams.VesselCfr) || ship.Cfr == queryParams.VesselCfr)
                                                    && (string.IsNullOrEmpty(queryParams.VesselIrcs) || ship.IrcscallSign == queryParams.VesselIrcs)
                                              select ship.Id).ToList();

                // Get all related trip identifiers for the vessel

                List<string> relatedTripIdentifiers = (from fluxFvmsActivityReport in Db.FvmsfishingActivityReports
                                                       join fishingActivityShipPage in Db.FvmsfishingActivityReportLogBookPages on fluxFvmsActivityReport.Id equals fishingActivityShipPage.FishingActivityReportId
                                                       where fluxFvmsActivityReport.VesselId.HasValue
                                                             && relatedVesselIds.Contains(fluxFvmsActivityReport.VesselId.Value)
                                                             && fluxFvmsActivityReport.IsActive
                                                             && fluxFvmsActivityReport.CreatedOn >= queryParams.PeriodStartDate.Value
                                                             && fluxFvmsActivityReport.CreatedOn <= queryParams.PeriodEndDate.Value
                                                       orderby fluxFvmsActivityReport.CreatedOn
                                                       group fishingActivityShipPage by fishingActivityShipPage.TripIdentifier into activityPage
                                                       select activityPage.Key).ToList();

                faReportDocuments = queryHelper.GetRelatedFARepotDocuments(relatedTripIdentifiers, queryParams.IsConsolidated); // no need to pass start and end dates, because the trip identifiers are already filtered
            }

            result.FAReportDocument = faReportDocuments.ToArray();

            return result;
        }

        public void ReceiveFishingActivitiesReport(FLUXFAReportMessageType report)
        {
            CodeType faReportType = report.FAReportDocument[0].TypeCode;
            CodeType faType = report.FAReportDocument[0].SpecifiedFishingActivity != null ? report.FAReportDocument[0].SpecifiedFishingActivity[0].TypeCode : null;
            FvmsfishingActivityReport fvmsFishingActivityReport = helper.AddFVMSFishingActivityReport(report, faReportType, faType);

            List<ShipLogBookPage> relatedPages = new List<ShipLogBookPage>();

            foreach (FAReportDocumentType faReportDocument in report.FAReportDocument)
            {
                switch (int.Parse(report.FLUXReportDocument.PurposeCode.Value))
                {
                    case (int)FluxPurposes.Original:
                        if (faReportDocument.TypeCode.Value == nameof(FaReportTypes.NOTIFICATION))
                        {
                            relatedPages = helper.MapFluxFAReportNotificationOriginal(faReportDocument);
                        }
                        else if (faReportDocument.TypeCode.Value == nameof(FaReportTypes.DECLARATION))
                        {
                            relatedPages = helper.MapFluxFAReportDeclarationOriginal(report.FLUXReportDocument.ID, faReportDocument, report.FLUXReportDocument.CreationDateTime.Item);
                        }
                        break;
                    case (int)FluxPurposes.Replace:
                        if (faReportDocument.TypeCode.Value == nameof(FaReportTypes.NOTIFICATION))
                        {
                            relatedPages = helper.MapFluxFAReportNotificationReplace(faReportDocument);
                        }
                        else if (faReportDocument.TypeCode.Value == nameof(FaReportTypes.DECLARATION))
                        {
                            relatedPages = helper.MapFluxFAReportDeclarationReplace(report.FLUXReportDocument.ID,
                                                                                    faReportDocument,
                                                                                    report.FLUXReportDocument.CreationDateTime.Item);
                        }
                        break;
                    case (int)FluxPurposes.Cancellation:
                        if (faReportDocument.TypeCode.Value == nameof(FaReportTypes.NOTIFICATION))
                        {
                            relatedPages = helper.MapFluxFAReportNotificationCancellation(faReportDocument);
                        }
                        else if (faReportDocument.TypeCode.Value == nameof(FaReportTypes.DECLARATION))
                        {
                            relatedPages = helper.MapFluxFAReportDeclarationCancellation(report.FLUXReportDocument.ID, faReportDocument, report.FLUXReportDocument.CreationDateTime.Item);
                        }
                        break;
                    case (int)FluxPurposes.Delete:
                        if (faReportDocument.TypeCode.Value == nameof(FaReportTypes.NOTIFICATION))
                        {
                            relatedPages = helper.MapFluxFAReportNotificationDelete(faReportDocument.RelatedFLUXReportDocument.ReferencedID);
                        }
                        else if (faReportDocument.TypeCode.Value == nameof(FaReportTypes.DECLARATION))
                        {
                            relatedPages = helper.MapFluxFAReportDeclarationDelete(report.FLUXReportDocument.ID, faReportDocument.RelatedFLUXReportDocument.ReferencedID, report.FLUXReportDocument.CreationDateTime.Item);
                        }
                        break;
                }
            }

            string specifiedFishingTrip = "NO_IDENTIFIER";

            if (report.FAReportDocument.Length > 0 && report.FAReportDocument[0].SpecifiedFishingActivity.Length > 0)
            {
                specifiedFishingTrip = helper.GetFishingTripIdentifier(report.FAReportDocument[0].SpecifiedFishingActivity[0].SpecifiedFishingTrip);
            }

            foreach (ShipLogBookPage logBookPage in relatedPages)
            {
                helper.AddFvmsFishingActivityReportLogBookPage(fvmsFishingActivityReport, logBookPage, specifiedFishingTrip);
            }

            Db.SaveChanges();
        }

        public Task<bool> ReportFishingActivities(ShipLogBookPageFLUXFieldsDTO logBookPageData)
        {
            var relatedMessageFishingTripIdentifiers = (from fvmsFishingActivityReportPage in Db.FvmsfishingActivityReportLogBookPages
                                                        join fvmsFishingActivityReport in Db.FvmsfishingActivityReports on fvmsFishingActivityReportPage.FishingActivityReportId equals fvmsFishingActivityReport.Id
                                                        join faReportType in Db.MdrFluxFaReportTypes on fvmsFishingActivityReport.MdrFluxFaReportTypeId equals faReportType.Id
                                                        join faType in Db.MdrFluxFaTypes on fvmsFishingActivityReport.MdrFluxFaTypeId equals faType.Id
                                                        where fvmsFishingActivityReport.IsActive
                                                              && fvmsFishingActivityReportPage.ShipLogBookPageId == logBookPageData.PageId
                                                        select new
                                                        {
                                                            FluxMessageId = fvmsFishingActivityReport.ResponseUuid,
                                                            FluxMessageReferenceId = fvmsFishingActivityReport.ReferencedResponseUuid,
                                                            FaReportType = Enum.Parse<FaReportTypes>(faReportType.Code),
                                                            FaType = Enum.Parse<FaTypes>(faType.Code),
                                                            CreatedOn = fvmsFishingActivityReport.CreatedOn,
                                                            TripIdentifier = fvmsFishingActivityReportPage.TripIdentifier
                                                        }).ToLookup(x =>
                                                        new FluxFaMessageData
                                                        {
                                                            FluxMessageId = x.FluxMessageId,
                                                            FluxMessageReferenceId = x.FluxMessageReferenceId,
                                                            FaReportType =x.FaReportType,
                                                            FaType = x.FaType,
                                                            CreatedOn = x.CreatedOn
                                                        },
                                                        y => y.TripIdentifier);

            FLUXFAReportMessageType report = createFAHelper.CreateFluxFAReportMessage(logBookPageData, relatedMessageFishingTripIdentifiers);

            return faQueueService.ReportFishingActivities(report);
        }
    }

    internal class ReportBasicData
    {
        public int ActivityReportId { get; set; }

        public Guid FluxFvmsRequestUUId { get; set; }

        public Guid? ReferencedFluxFvmsRequestUUId { get; set; }

        public string FluxFvmsRequestContent { get; set; }

        public int GPPurposeCode { get; set; }

        public DateTime CreatedOn { get; set; }
    }

    internal class FluxFaMessageData
    {
        public Guid FluxMessageId { get; set; }
        public Guid? FluxMessageReferenceId { get; set; }
        public FaReportTypes FaReportType { get; set; }
        public FaTypes FaType { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
