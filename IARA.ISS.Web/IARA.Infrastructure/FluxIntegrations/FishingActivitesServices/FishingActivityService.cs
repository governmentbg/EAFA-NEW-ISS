using IARA.DomainModels.DTOModels.CatchesAndSales;
using IARA.Flux.Models;
using IARA.FluxModels.Enums;
using IARA.Infrastructure.FluxIntegrations.FishingActivitesServices.Models;
using TL.Logging.Abstractions.Interfaces;
using CommonUtils = TL.SysToSysSecCom.Abstractions.Utils.CommonUtils;

namespace IARA.Infrastructure.FluxIntegrations.FishingActivitesServices
{
    public class FishingActivityService : BaseFishingActivityService
    {
        private readonly FishingOperationService fishingOperationService;
        private readonly GearShotService gearShotService;
        private readonly GearRetrievalService gearRetrievalService;
        private readonly TranshipmentService transhipmentService;
        private readonly DiscardService discardService;
        private readonly ArrivalNotificationService arrivalNotificationService;
        private readonly ArrivalDeclarationService arrivalDeclarationService;
        private readonly LandingService landingService;

        public FishingActivityService(IARADbContext dbContext, IExtendedLogger logger)
            : base(dbContext, logger, nameof(FishingActivityService))
        {
            this.fishingOperationService = new FishingOperationService(dbContext, logger);
            this.gearShotService = new GearShotService(dbContext, logger);
            this.gearRetrievalService = new GearRetrievalService(dbContext, logger);
            this.transhipmentService = new TranshipmentService(dbContext, logger);
            this.discardService = new DiscardService(dbContext, logger);
            this.arrivalNotificationService = new ArrivalNotificationService(dbContext, logger);
            this.arrivalDeclarationService = new ArrivalDeclarationService(dbContext, logger);
            this.landingService = new LandingService(dbContext, logger);
        }

        public FLUXFAReportMessageType CreateFluxFAReportMessage(ShipLogBookPageFLUXFieldsDTO data, ILookup<FluxFaMessageData, string> fluxMessageIdentifiers)
        {
            FAReportDocumentType[] faReportDocuments = Array.Empty<FAReportDocumentType>();
            ReportPurposeCodes purposeCode;

            if (data.IsCancelled.HasValue && data.IsCancelled.Value)
            {
                purposeCode = ReportPurposeCodes.Cancellation;

                var departureGrouping = fluxMessageIdentifiers.Where(x => x.Key.FaType == FaTypes.DEPARTURE).OrderByDescending(x => x.Key.CreatedOn).First();
                FvmsfishingActivityReport referencedReport = GetDocumentByUUID(departureGrouping.Key.FluxMessageId);
                FLUXFAReportMessageType referencedMessage = CommonUtils.JsonDeserialize<FLUXFAReportMessageType>(referencedReport.ResponseMessage);
            }
            else if (data.IsActive.HasValue && data.IsActive.Value)
            {
                purposeCode = ReportPurposeCodes.Delete;
            }
            else
            {
                purposeCode = ReportPurposeCodes.Replace;
            }

            FLUXFAReportMessageType report = new()
            {
                FLUXReportDocument = CreateFluxReportDocument(purposeCode),
                FAReportDocument = faReportDocuments
            };

            return report;
        }

        public List<RelatedPageData> MapNotificationOriginal(FishingActivityType fishingActivity)
        {
            List<RelatedPageData> relatedPages = new();

            if (Enum.TryParse(fishingActivity.TypeCode.Value, out FaTypes faType))
            {
                switch (faType)
                {
                    case FaTypes.ARRIVAL:
                        relatedPages = arrivalNotificationService.MapOriginal(fishingActivity);
                        break;
                }
            }
            else
            {
                string fishingTrip = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
                LogWarning($"unknown FLUX_FA_TYPE ({fishingActivity.TypeCode.Value}), in trip: {fishingTrip}", nameof(MapNotificationOriginal));
            }

            return relatedPages;
        }

        public List<RelatedPageData> MapNotificationReplace(FishingActivityType fishingActivity)
        {
            List<RelatedPageData> relatedPages = MapNotificationOriginal(fishingActivity);
            return relatedPages;
        }

        public List<RelatedPageData> MapNotificationCancel(IDType previousReportId, FishingActivityType fishingActivity)
        {
            List<RelatedPageData> relatedPages = new();

            if (previousReportId == null)
            {
                throw new ArgumentNullException("Reference ID in RelatedFLUXReportDocument must be present, when the message is a cancellation of a previous one");
            }

            if (Enum.TryParse(fishingActivity.TypeCode.Value, out FaTypes faType))
            {
                switch (faType)
                {
                    case FaTypes.ARRIVAL:
                        relatedPages = arrivalNotificationService.MapCancel(previousReportId);
                        break;
                }
            }
            else
            {
                string fishingTrip = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
                LogWarning($"unknown FLUX_FA_TYPE ({fishingActivity.TypeCode.Value}), in trip: {fishingTrip}", nameof(MapNotificationCancel));
            }

            return relatedPages;
        }

        public List<RelatedPageData> MapNotificationDelete(IDType referenceId, FishingActivityType fishingActivity)
        {
            List<RelatedPageData> relatedPages = new();

            if (referenceId == null)
            {
                throw new ArgumentNullException("Reference ID in RelatedFLUXReportDocument must be present, when the message is a deletion of a previous one");
            }

            FvmsfishingActivityReport referencedReport = GetDocumentByUUID((Guid)referenceId);
            FLUXFAReportMessageType referencedMessage = CommonUtils.JsonDeserialize<FLUXFAReportMessageType>(referencedReport.ResponseMessage);

            if (Enum.TryParse(fishingActivity.TypeCode.Value, out FaTypes faType))
            {
                switch (faType)
                {
                    case FaTypes.ARRIVAL:
                        relatedPages = arrivalNotificationService.MapDelete(referenceId);
                        break;
                }
            }
            else
            {
                string fishingTrip = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
                LogWarning($"unknown FLUX_FA_TYPE ({fishingActivity.TypeCode.Value}), in trip: {fishingTrip}", nameof(MapNotificationDelete));
            }

            return relatedPages;
        }

        public List<RelatedPageData> MapDeclarationOriginal(IDType[] documentId,
                                                            FAReportDocumentType faReportDocument,
                                                            FishingActivityType fishingActivity,
                                                            DateTime documentOccurrenceDateTime)
        {
            List<RelatedPageData> relatedPages = new();

            IDType[] relatedReportId = null;

            if (faReportDocument.RelatedReportID != null && !CompareIDTypes(faReportDocument.RelatedReportID, documentId))
            {
                relatedReportId = faReportDocument.RelatedReportID;
            }

            if (Enum.TryParse(fishingActivity.TypeCode.Value, out FaTypes faType))
            {
                switch (faType)
                {
                    case FaTypes.DEPARTURE:
                    case FaTypes.AREA_ENTRY:
                    case FaTypes.AREA_EXIT:
                    case FaTypes.JOINT_FISHING_OPERATION:
                    case FaTypes.START_ACTIVITY:
                    case FaTypes.RELOCATION:
                        // not relevant to our db for mapping
                        break;
                    case FaTypes.FISHING_OPERATION:
                        relatedPages.AddRange(fishingOperationService.MapOriginal(fishingActivity, faReportDocument.SpecifiedVesselTransportMeans, relatedReportId));
                        break;
                    case FaTypes.GEAR_SHOT:
                        relatedPages = gearShotService.MapOriginal(fishingActivity, faReportDocument.SpecifiedVesselTransportMeans);
                        break;
                    case FaTypes.GEAR_RETRIEVAL:
                        relatedPages = gearRetrievalService.MapOriginal(relatedReportId, fishingActivity);
                        break;
                    case FaTypes.TRANSHIPMENT:
                        {
                            string vesselRoleCode = faReportDocument.SpecifiedVesselTransportMeans.RoleCode.Value;

                            if (Enum.TryParse(vesselRoleCode, out FaVesselRoleCodes vesselRole))
                            {
                                relatedPages = transhipmentService.MapOriginal(fishingActivity, vesselRole, documentOccurrenceDateTime);
                            }
                            else
                            {
                                LogWarning($"invalid role of vessel in a transhipment fa type report message: {vesselRoleCode}", nameof(MapDeclarationOriginal));
                            }
                        }
                        break;
                    case FaTypes.DISCARD:
                        relatedPages = discardService.MapOriginal(relatedReportId, fishingActivity);
                        break;
                    case FaTypes.ARRIVAL:
                        relatedPages = arrivalDeclarationService.MapOriginal(fishingActivity);
                        break;
                    case FaTypes.LANDING:
                        relatedPages = landingService.MapOriginal(fishingActivity, documentOccurrenceDateTime);
                        break;
                }
            }
            else
            {
                string fishingTrip = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
                LogWarning($"unknown FLUX_FA_TYPE ({fishingActivity.TypeCode.Value}), in trip: {fishingTrip}", nameof(MapDeclarationOriginal));
            }

            return relatedPages;
        }

        public List<RelatedPageData> MapDeclarationReplace(IDType[] documentId,
                                                           FAReportDocumentType faReportDocument,
                                                           FishingActivityType fishingActivity,
                                                           DateTime documentOccurrenceDateTime)
        {
            List<RelatedPageData> relatedPages = new();

            IDType[] relatedReportId = null;

            if (faReportDocument.RelatedReportID != null && !CompareIDTypes(faReportDocument.RelatedReportID, documentId))
            {
                relatedReportId = faReportDocument.RelatedReportID;
            }

            IDType previousReportId = faReportDocument.RelatedFLUXReportDocument.ReferencedID;

            if (previousReportId == null)
            {
                throw new ArgumentNullException("Reference ID in RelatedFLUXReportDocument must be present, when the message is an update of a previous one");
            }

            if (Enum.TryParse(fishingActivity.TypeCode.Value, out FaTypes faType))
            {
                switch (faType)
                {
                    case FaTypes.DEPARTURE:
                    case FaTypes.AREA_ENTRY:
                    case FaTypes.AREA_EXIT:
                    case FaTypes.JOINT_FISHING_OPERATION:
                    case FaTypes.START_ACTIVITY:
                    case FaTypes.RELOCATION:
                        // not relevant to our db for mapping
                        break;
                    case FaTypes.FISHING_OPERATION:
                        relatedPages = fishingOperationService.MapUpdate(fishingActivity, previousReportId, relatedReportId, faReportDocument.SpecifiedVesselTransportMeans);
                        break;
                    case FaTypes.GEAR_SHOT:
                        relatedPages = gearShotService.MapUpdate(fishingActivity);
                        break;
                    case FaTypes.GEAR_RETRIEVAL:
                        relatedPages = gearRetrievalService.MapUpdate(fishingActivity, previousReportId, relatedReportId);
                        break;
                    case FaTypes.TRANSHIPMENT:
                        {
                            string vesselRoleCode = faReportDocument.SpecifiedVesselTransportMeans.RoleCode.Value;

                            if (Enum.TryParse(vesselRoleCode, out FaVesselRoleCodes vesselRole))
                            {
                                relatedPages = transhipmentService.MapUpdate(fishingActivity, vesselRole, documentOccurrenceDateTime);
                            }
                            else
                            {
                                LogWarning($"invalid role of vessel in a transhipment fa type report message: {vesselRoleCode}", nameof(MapDeclarationReplace));
                            }
                        }
                        break;
                    case FaTypes.DISCARD:
                        relatedPages = discardService.MapUpdate(fishingActivity, relatedReportId, previousReportId);
                        break;
                    case FaTypes.ARRIVAL:
                        relatedPages = arrivalDeclarationService.MapUpdate(fishingActivity);
                        break;
                    case FaTypes.LANDING:
                        relatedPages = landingService.MapUpdate(fishingActivity, documentOccurrenceDateTime, previousReportId);
                        break;
                }
            }
            else
            {
                string fishingTrip = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
                LogWarning($"unknown FLUX_FA_TYPE ({fishingActivity.TypeCode.Value}), in trip: {fishingTrip}", nameof(MapDeclarationReplace));
            }

            return relatedPages;
        }

        public List<RelatedPageData> MapDeclarationCancel(IDType[] documentId,
                                                          FAReportDocumentType faReportDocument,
                                                          FishingActivityType fishingActivity,
                                                          DateTime documentOccurrenceDateTime)
        {
            List<RelatedPageData> relatedPages = new();

            IDType referencedIdType = faReportDocument.RelatedFLUXReportDocument.ReferencedID;

            if (referencedIdType == null)
            {
                throw new ArgumentNullException("Reference ID in RelatedFLUXReportDocument must be present, when the message is a cancellation of a previous one");
            }

            FvmsfishingActivityReport referencedReport = GetDocumentByUUID((Guid)referencedIdType);
            FLUXFAReportMessageType referencedMessage = CommonUtils.JsonDeserialize<FLUXFAReportMessageType>(referencedReport.ResponseMessage);

            IDType[] relatedReportId = null;

            if (faReportDocument.RelatedReportID != null && !CompareIDTypes(faReportDocument.RelatedReportID, documentId))
            {
                relatedReportId = faReportDocument.RelatedReportID;
            }

            if (Enum.TryParse(fishingActivity.TypeCode.Value, out FaTypes faType))
            {
                switch (faType)
                {
                    case FaTypes.DEPARTURE:
                    case FaTypes.AREA_ENTRY:
                    case FaTypes.AREA_EXIT:
                    case FaTypes.JOINT_FISHING_OPERATION:
                    case FaTypes.START_ACTIVITY:
                    case FaTypes.RELOCATION:
                        // not relevant to our db for mapping
                        break;
                    case FaTypes.FISHING_OPERATION:
                        relatedPages = fishingOperationService.MapCancel(fishingActivity, referencedIdType, relatedReportId);
                        break;
                    case FaTypes.GEAR_SHOT:
                        relatedPages = gearShotService.MapCancel(fishingActivity);
                        break;
                    case FaTypes.GEAR_RETRIEVAL:
                        relatedPages = gearRetrievalService.MapCancel(fishingActivity, relatedReportId);
                        break;
                    case FaTypes.TRANSHIPMENT:
                        {
                            string vesselRoleCode = faReportDocument.SpecifiedVesselTransportMeans.RoleCode.Value;

                            if (Enum.TryParse(vesselRoleCode, out FaVesselRoleCodes vesselRole))
                            {
                                relatedPages = transhipmentService.MapCancel(fishingActivity, vesselRole, documentOccurrenceDateTime);
                            }
                            else
                            {
                                LogWarning($"invalid role of vessel in a transhipment fa type report message: {vesselRoleCode}", nameof(MapDeclarationCancel));
                            }
                        }
                        break;
                    case FaTypes.DISCARD:
                        relatedPages = discardService.MapCancel(fishingActivity, relatedReportId);
                        break;
                    case FaTypes.ARRIVAL:
                        relatedPages = arrivalDeclarationService.MapCancel(referencedIdType);
                        break;
                    case FaTypes.LANDING:
                        relatedPages = landingService.MapCancel(fishingActivity, documentOccurrenceDateTime);
                        break;
                }
            }
            else
            {
                string fishingTrip = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
                LogWarning($"unknown FLUX_FA_TYPE ({fishingActivity.TypeCode.Value}), in trip: {fishingTrip}", nameof(MapDeclarationCancel));
            }

            return relatedPages;
        }

        public List<RelatedPageData> MapDeclarationDelete(IDType[] documentId,
                                                          FAReportDocumentType faReportDocument,
                                                          FishingActivityType fishingActivity,
                                                          DateTime documentOccurrenceDateTime)
        {
            List<RelatedPageData> relatedPages = new();

            IDType referencedReportId = faReportDocument.RelatedFLUXReportDocument?.ReferencedID;
            if (referencedReportId == null)
            {
                throw new ArgumentNullException("Reference ID in RelatedFLUXReportDocument must be present, when the message is a deletion of a previous one");
            }

            FvmsfishingActivityReport referencedReport = GetDocumentByUUID((Guid)referencedReportId);
            FLUXFAReportMessageType referencedMessage = CommonUtils.JsonDeserialize<FLUXFAReportMessageType>(referencedReport.ResponseMessage);

            IDType[] relatedReportId = null;

            if (faReportDocument.RelatedReportID != null && !CompareIDTypes(faReportDocument.RelatedReportID, documentId))
            {
                relatedReportId = faReportDocument.RelatedReportID;
            }

            if (Enum.TryParse(fishingActivity.TypeCode.Value, out FaTypes faType))
            {
                switch (faType)
                {
                    case FaTypes.DEPARTURE:
                    case FaTypes.AREA_ENTRY:
                    case FaTypes.AREA_EXIT:
                    case FaTypes.JOINT_FISHING_OPERATION:
                    case FaTypes.START_ACTIVITY:
                    case FaTypes.RELOCATION:
                        // not relevant to our db for mapping
                        break;
                    case FaTypes.FISHING_OPERATION:
                        relatedPages = fishingOperationService.MapDelete(fishingActivity, referencedReportId, relatedReportId);
                        break;
                    case FaTypes.GEAR_SHOT:
                        relatedPages = gearShotService.MapDelete(fishingActivity);
                        break;
                    case FaTypes.GEAR_RETRIEVAL:
                        relatedPages = gearRetrievalService.MapDelete(fishingActivity, relatedReportId);
                        break;
                    case FaTypes.TRANSHIPMENT:
                        {
                            string vesselRoleCode = faReportDocument.SpecifiedVesselTransportMeans.RoleCode.Value;

                            if (Enum.TryParse(vesselRoleCode, out FaVesselRoleCodes vesselRole))
                            {
                                relatedPages = transhipmentService.MapDelete(fishingActivity, vesselRole, documentOccurrenceDateTime);
                            }
                            else
                            {
                                LogWarning($"invalid role of vessel in a transhipment fa type report message: {vesselRoleCode}", nameof(MapDeclarationCancel));
                            }
                        }
                        break;
                    case FaTypes.DISCARD:
                        relatedPages = discardService.MapDelete(fishingActivity, relatedReportId);
                        break;
                    case FaTypes.ARRIVAL:
                        relatedPages = arrivalDeclarationService.MapDelete(referencedReportId);
                        break;
                    case FaTypes.LANDING:
                        relatedPages = landingService.MapDelete(fishingActivity, documentOccurrenceDateTime);
                        break;
                }
            }
            else
            {
                string fishingTrip = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
                LogWarning($"unknown FLUX_FA_TYPE ({fishingActivity.TypeCode.Value}), in trip: {fishingTrip}", nameof(MapDeclarationDelete));
            }

            return relatedPages;
        }

        public FvmsfishingActivityReport AddFVMSFishingActivityReport(FLUXFAReportMessageType faReportMessage, FishingActivityType fishingActivity)
        {
            FLUXReportDocumentType reportDocument = faReportMessage.FLUXReportDocument;
            FAReportDocumentType faReport = faReportMessage.FAReportDocument[0];
            CodeType faReportType = faReport.TypeCode;
            CodeType faPurpose = faReport.RelatedFLUXReportDocument.PurposeCode;
            CodeType faType = fishingActivity.TypeCode;

            DateTime date = fishingActivity.OccurrenceDateTime != null
                ? (DateTime)fishingActivity.OccurrenceDateTime
                : fishingActivity.SpecifiedDelimitedPeriod != null && fishingActivity.SpecifiedDelimitedPeriod.Any()
                    ? (DateTime)fishingActivity.SpecifiedDelimitedPeriod[0].EndDateTime
                    : (DateTime)reportDocument.CreationDateTime;

            Guid id = (Guid)reportDocument.ID[0];
            Guid? referencedId = reportDocument.ReferencedID != null ? (Guid)reportDocument.ReferencedID : default(Guid?);

            List<int> shipIds = GetShipIds(faReport.SpecifiedVesselTransportMeans, referencedId);

            int mdrFluxFaReportTypeId = (from fluxFaReportType in Db.MdrFluxFaReportTypes
                                         where fluxFaReportType.Code == faReportType.Value
                                               && fluxFaReportType.ValidFrom <= date
                                               && fluxFaReportType.ValidTo > date
                                         select fluxFaReportType.Id).First();

            int mdrFluxFaTypeId = (from fluxFaType in Db.MdrFluxFaTypes
                                   where fluxFaType.Code == faType.Value
                                         && fluxFaType.ValidFrom <= date
                                         && fluxFaType.ValidTo > date
                                   select fluxFaType.Id).First();

            int mdrFluxGpPurposeId = (from fluxGpPurpose in Db.MdrFluxGpPurposes
                                      where fluxGpPurpose.Code == faPurpose.Value
                                            && fluxGpPurpose.ValidFrom <= date
                                            && fluxGpPurpose.ValidTo > date
                                      select fluxGpPurpose.Id).First();

            int? mdrFluxFaSubType1Id = null;
            int? mdrFluxFaSubType2Id = null;

            if (fishingActivity.RelatedFishingActivity != null)
            {
                if (fishingActivity.RelatedFishingActivity.Length > 0 && fishingActivity.RelatedFishingActivity[0].TypeCode != null)
                {
                    mdrFluxFaSubType1Id = (from fluxFaType in Db.MdrFluxFaTypes
                                           where fluxFaType.Code == fishingActivity.RelatedFishingActivity[0].TypeCode.Value
                                                 && fluxFaType.ValidFrom <= date
                                                 && fluxFaType.ValidTo > date
                                           select fluxFaType.Id).First();
                }

                if (fishingActivity.RelatedFishingActivity.Length > 1 && fishingActivity.RelatedFishingActivity[1].TypeCode != null)
                {
                    mdrFluxFaSubType2Id = (from fluxFaType in Db.MdrFluxFaTypes
                                           where fluxFaType.Code == fishingActivity.RelatedFishingActivity[1].TypeCode.Value
                                                 && fluxFaType.ValidFrom <= date
                                                 && fluxFaType.ValidTo > date
                                           select fluxFaType.Id).First();
                }
            }

            FvmsfishingActivityReport fvmsFishingActivityReport = new()
            {
                ResponseUuid = id,
                ReferencedResponseUuid = referencedId,
                OccurenceDateTime = date,
                ResponseMessage = CommonUtils.JsonSerialize(faReportMessage),
                MdrFluxFaReportTypeId = mdrFluxFaReportTypeId,
                VesselId = shipIds.First(),
                MdrFluxFaTypeId = mdrFluxFaTypeId,
                MdrFluxFaSubType1Id = mdrFluxFaSubType1Id,
                MdrFluxFaSubType2Id = mdrFluxFaSubType2Id,
                MdrFluxGpPurposeId = mdrFluxGpPurposeId,
                TripIdentifier = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip),
                Status = nameof(FvmsFishingActivityReportStatus.Received)
            };

            Db.FvmsfishingActivityReports.Add(fvmsFishingActivityReport);
            Db.SaveChanges();

            return fvmsFishingActivityReport;
        }

        public FvmsfishingActivityReportLogBookPage AddFvmsFishingActivityReportLogBookPage(FvmsfishingActivityReport fvmsFishingActivityReport,
                                                                                            RelatedPageData relatedPage)
        {
            FvmsfishingActivityReportLogBookPage fvmsFishingActivityReportLogBookPage = new()
            {
                FishingActivityReport = fvmsFishingActivityReport,
                ShipLogBookPage = relatedPage.Page,
                IsMainPage = relatedPage.IsPrimaryRelated
            };

            Db.FvmsfishingActivityReportLogBookPages.Add(fvmsFishingActivityReportLogBookPage);

            return fvmsFishingActivityReportLogBookPage;
        }

        private static FLUXReportDocumentType CreateFluxReportDocument(ReportPurposeCodes purposeCode)
        {
            DateTime now = DateTime.Now;

            var fluxReportDocument = new FLUXReportDocumentType
            {
                ID = new IDType[] { IDType.GenerateGuid() },
                CreationDateTime = DateTimeType.BuildDateTime(now),
                PurposeCode = CodeType.CreatePurpose(purposeCode),
                OwnerFLUXParty = new FLUXPartyType
                {
                    ID = new IDType[] { IDType.CreateParty(nameof(CountryCodes.BGR)) },
                    Name = new TextType[] { TextType.CreateText("Bulgaria") }
                }
            };

            return fluxReportDocument;
        }
    }
}
