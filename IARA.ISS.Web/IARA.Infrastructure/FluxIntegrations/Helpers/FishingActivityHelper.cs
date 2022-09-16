using System;
using System.Collections.Generic;
using System.Linq;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.DataAccess;
using IARA.EntityModels.Entities;
using IARA.Flux.Models;
using IARA.FluxModels;
using IARA.FluxModels.Enums;
using IARA.Logging.Abstractions.Interfaces;
using Microsoft.EntityFrameworkCore;
using TL.SysToSysSecCom;

namespace IARA.Infrastructure.FluxIntegrations.Helpers
{
    internal partial class FishingActivityHelper
    {
        private IARADbContext Db;

        private IExtendedLogger Logger { get; set; }

        private const string LOGGER_MSG_TYPE = "FLUX FA DOMAIN:";

        public FishingActivityHelper(IARADbContext dbContext, IExtendedLogger logger)
        {
            Db = dbContext;
            Logger = logger;
        }

        public List<ShipLogBookPage> MapFluxFAReportNotificationOriginal(FAReportDocumentType faReportDocument)
        {
            List<ShipLogBookPage> relatedPages = new List<ShipLogBookPage>();

            foreach (FishingActivityType fishingActivity in faReportDocument.SpecifiedFishingActivity)
            {
                FaTypes faType;
                bool isSuccessfulFaTypeCast = Enum.TryParse<FaTypes>(fishingActivity.TypeCode.Value, out faType);
                if (isSuccessfulFaTypeCast)
                {
                    switch (faType)
                    {
                        case FaTypes.ARRIVAL:
                            relatedPages = MapFluxFAReportNotificationArrival(fishingActivity);
                            break;
                    }
                }
                else
                {
                    string fishingTrip = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
                    Logger.LogWarning($"{LOGGER_MSG_TYPE} Unknown FLUX_FA_TYPE ({fishingActivity.TypeCode.Value}), in trip: {fishingTrip}");
                }
            }

            return relatedPages;
        }

        public List<ShipLogBookPage> MapFluxFAReportNotificationReplace(FAReportDocumentType faReportDocument)
        {
            List<ShipLogBookPage> relatedPages = MapFluxFAReportNotificationOriginal(faReportDocument);
            return relatedPages;
        }

        public List<ShipLogBookPage> MapFluxFAReportNotificationCancellation(FAReportDocumentType faReportDocument)
        {
            List<ShipLogBookPage> relatedPages = new List<ShipLogBookPage>();

            foreach (FishingActivityType fishingActivity in faReportDocument.SpecifiedFishingActivity)
            {
                FaTypes faType;
                bool isSuccessfulFaTypeCast = Enum.TryParse<FaTypes>(fishingActivity.TypeCode.Value, out faType);
                if (isSuccessfulFaTypeCast)
                {
                    switch (faType)
                    {
                        case FaTypes.ARRIVAL:
                            relatedPages = CancelFluxFAReportNotificationArrival(faReportDocument.RelatedFLUXReportDocument.ReferencedID);
                            break;
                    }
                }
                else
                {
                    string fishingTrip = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
                    Logger.LogWarning($"{LOGGER_MSG_TYPE} Unknown FLUX_FA_TYPE ({fishingActivity.TypeCode.Value}), in trip: {fishingTrip}");
                }
            }

            return relatedPages;
        }

        public List<ShipLogBookPage> MapFluxFAReportNotificationDelete(IDType referenceId)
        {
            List<ShipLogBookPage> relatedPages = new List<ShipLogBookPage>();

            FvmsfishingActivityReport referencedReport = GetDocumentByUUID((Guid)referenceId);
            FLUXFAReportMessageType referencedMessage = CommonUtils.JsonDeserialize<FLUXFAReportMessageType>(referencedReport.ResponseMessage);

            foreach (FAReportDocumentType fAReportDocument in referencedMessage.FAReportDocument)
            {
                foreach (FishingActivityType fishingActivity in fAReportDocument.SpecifiedFishingActivity)
                {
                    FaTypes faType;
                    bool isSuccessfulFaTypeCast = Enum.TryParse<FaTypes>(fishingActivity.TypeCode.Value, out faType);
                    if (isSuccessfulFaTypeCast)
                    {
                        switch (faType)
                        {
                            case FaTypes.ARRIVAL:
                                relatedPages = DeleteFluxFAReportNotificationArrival(referenceId);
                                break;
                        }
                    }
                    else
                    {
                        string fishingTrip = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
                        Logger.LogWarning($"{LOGGER_MSG_TYPE} Unknown FLUX_FA_TYPE ({fishingActivity.TypeCode.Value}), in trip: {fishingTrip}");
                    }
                }
            }

            return relatedPages;
        }

        public List<ShipLogBookPage> MapFluxFAReportDeclarationOriginal(IDType[] documentId, FAReportDocumentType faReportDocument, DateTime documentOccurrenceDateTime)
        {
            List<ShipLogBookPage> relatedPages = new List<ShipLogBookPage>();

            List<int> shipIds = GetShipIds(faReportDocument.SpecifiedVesselTransportMeans);

            foreach (FishingActivityType fishingActivity in faReportDocument.SpecifiedFishingActivity)
            {
                IDType[] relatedReportId = null;

                if (faReportDocument.RelatedReportID != null && !CompareIDTypes(faReportDocument.RelatedReportID, documentId))
                {
                    relatedReportId = faReportDocument.RelatedReportID;
                }

                FaTypes faType;
                bool isSuccessfulFaTypeCast = Enum.TryParse<FaTypes>(fishingActivity.TypeCode.Value, out faType);
                if (isSuccessfulFaTypeCast)
                {
                    switch (faType)
                    {
                        case FaTypes.DEPARTURE:
                            relatedPages = MapFluxFAReportDeclarationDeparture(fishingActivity, shipIds);
                            break;
                        case FaTypes.AREA_ENTRY:
                        case FaTypes.AREA_EXIT:
                        case FaTypes.JOINT_FISHING_OPERATION:
                        case FaTypes.START_ACTIVITY:
                        case FaTypes.RELOCATION:
                            // not relevant to our db for mapping
                            break;
                        case FaTypes.FISHING_OPERATION:
                            relatedPages = MapFluxFaReportDeclartionFishingOperation(fishingActivity, relatedReportId);
                            break;
                        case FaTypes.GEAR_SHOT:
                            relatedPages = MapFluxFAReportDeclarationGearShot(fishingActivity);
                            break;
                        case FaTypes.GEAR_RETRIEVAL:
                            relatedPages = MapFluxFAReportDeclarationGearRetrieval(relatedReportId, fishingActivity);
                            break;
                        case FaTypes.TRANSHIPMENT:
                            {
                                string vesselRoleCode = faReportDocument.SpecifiedVesselTransportMeans.RoleCode.Value;
                                FaVesselRoleCodes vesselRole;
                                bool successfulCastRoleCode = Enum.TryParse<FaVesselRoleCodes>(vesselRoleCode, out vesselRole);

                                if (successfulCastRoleCode)
                                {
                                    relatedPages = MapFluxFAReportDeclarationTranshipment(fishingActivity, vesselRole, documentOccurrenceDateTime);
                                }
                                else
                                {
                                    Logger.LogWarning($"{LOGGER_MSG_TYPE} Invalid role of vessel in a transhipment fa type report message: {vesselRoleCode}");
                                }
                            }
                            break;
                        case FaTypes.DISCARD:
                            relatedPages = MapFluxFAReportDeclarationDiscardOperation(relatedReportId, fishingActivity);
                            break;
                        case FaTypes.ARRIVAL:
                            relatedPages = MapFluxFAReportDeclarationArrival(fishingActivity);
                            break;
                        case FaTypes.LANDING:
                            relatedPages = MapFluxFAReportDeclarationLanding(fishingActivity, documentOccurrenceDateTime);
                            break;
                    }
                }
                else
                {
                    string fishingTrip = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
                    Logger.LogWarning($"{LOGGER_MSG_TYPE} Unknown FLUX_FA_TYPE ({fishingActivity.TypeCode.Value}), in trip: {fishingTrip}");
                }
            }

            return relatedPages;
        }

        public List<ShipLogBookPage> MapFluxFAReportDeclarationReplace(IDType[] documentId, FAReportDocumentType faReportDocument, DateTime documentOccurrenceDateTime)
        {
            List<ShipLogBookPage> relatedPages = new List<ShipLogBookPage>();

            List<int> shipIds = GetShipIds(faReportDocument.SpecifiedVesselTransportMeans);

            foreach (FishingActivityType fishingActivity in faReportDocument.SpecifiedFishingActivity)
            {
                IDType[] relatedReportId = null;

                if (faReportDocument.RelatedReportID != null && !CompareIDTypes(faReportDocument.RelatedReportID, documentId))
                {
                    relatedReportId = faReportDocument.RelatedReportID;
                }

                FaTypes faType;
                bool isSuccessfulFaTypeCast = Enum.TryParse<FaTypes>(fishingActivity.TypeCode.Value, out faType);
                if (isSuccessfulFaTypeCast)
                {
                    switch (faType)
                    {
                        case FaTypes.DEPARTURE:
                            relatedPages = UpdateFluxFAReportDeclarationDeparture(fishingActivity, shipIds);
                            break;
                        case FaTypes.AREA_ENTRY:
                        case FaTypes.AREA_EXIT:
                        case FaTypes.JOINT_FISHING_OPERATION:
                        case FaTypes.START_ACTIVITY:
                        case FaTypes.RELOCATION:
                            // not relevant to our db for mapping
                            break;
                        case FaTypes.FISHING_OPERATION:
                            relatedPages = UpdateFluxFaReportDeclartionFishingOperation(fishingActivity, faReportDocument.RelatedFLUXReportDocument.ReferencedID, relatedReportId);
                            break;
                        case FaTypes.GEAR_SHOT:
                            relatedPages = UpdateFluxFAReportDeclarationGearShot(fishingActivity);
                            break;
                        case FaTypes.GEAR_RETRIEVAL:
                            relatedPages = UpdateFluxFAReportDeclarationGearRetrieval(fishingActivity, faReportDocument.RelatedFLUXReportDocument.ReferencedID, relatedReportId);
                            break;
                        case FaTypes.TRANSHIPMENT:
                            // TODO
                            break;
                        case FaTypes.DISCARD:
                            relatedPages = UpdateFluxFAReportDeclarationDiscardOperation(fishingActivity, relatedReportId, faReportDocument.RelatedFLUXReportDocument.ReferencedID);
                            break;
                        case FaTypes.ARRIVAL:
                            relatedPages = UpdateFluxFAReportDeclarationArrival(fishingActivity);
                            break;
                        case FaTypes.LANDING:
                            relatedPages = UpdateFluxFAReportDeclarationLanding(fishingActivity, documentOccurrenceDateTime, faReportDocument.RelatedFLUXReportDocument.ReferencedID);
                            break;
                    }
                }
                else
                {
                    string fishingTrip = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
                    Logger.LogWarning($"{LOGGER_MSG_TYPE} Unknown FLUX_FA_TYPE ({fishingActivity.TypeCode.Value}), in trip: {fishingTrip}");
                }
            }

            return relatedPages;
        }

        public List<ShipLogBookPage> MapFluxFAReportDeclarationCancellation(IDType[] documentId, FAReportDocumentType faReportDocument, DateTime documentOccurrenceDateTime)
        {
            List<ShipLogBookPage> relatedPages = new List<ShipLogBookPage>();
            IDType referencedIdType = faReportDocument.RelatedFLUXReportDocument.ReferencedID;

            FvmsfishingActivityReport referencedReport = GetDocumentByUUID((Guid)referencedIdType);
            FLUXFAReportMessageType referencedMessage = CommonUtils.JsonDeserialize<FLUXFAReportMessageType>(referencedReport.ResponseMessage);

            foreach (FAReportDocumentType fAReportDocument in referencedMessage.FAReportDocument)
            {
                foreach (FishingActivityType fishingActivity in fAReportDocument.SpecifiedFishingActivity)
                {
                    IDType[] relatedReportId = null;

                    if (faReportDocument.RelatedReportID != null && !CompareIDTypes(faReportDocument.RelatedReportID, documentId))
                    {
                        relatedReportId = faReportDocument.RelatedReportID;
                    }

                    FaTypes faType;
                    bool isSuccessfulFaTypeCast = Enum.TryParse<FaTypes>(fishingActivity.TypeCode.Value, out faType);
                    if (isSuccessfulFaTypeCast)
                    {
                        switch (faType)
                        {
                            case FaTypes.DEPARTURE:
                                relatedPages = CancelFluxFAReportDeclarationDeparture(referencedIdType);
                                break;
                            case FaTypes.AREA_ENTRY:
                            case FaTypes.AREA_EXIT:
                            case FaTypes.JOINT_FISHING_OPERATION:
                            case FaTypes.START_ACTIVITY:
                            case FaTypes.RELOCATION:
                                // not relevant to our db for mapping
                                break;
                            case FaTypes.FISHING_OPERATION:
                                relatedPages = CancelFluxFaReportDeclartionFishingOperation(fishingActivity, relatedReportId);
                                break;
                            case FaTypes.GEAR_SHOT:
                                relatedPages = CancelFluxFAReportDeclarationGearShot(fishingActivity);
                                break;
                            case FaTypes.GEAR_RETRIEVAL:
                                relatedPages = CancelFluxFAReportDeclarationGearRetrieval(fishingActivity, relatedReportId);
                                break;
                            case FaTypes.TRANSHIPMENT:
                                // TODO
                                break;
                            case FaTypes.DISCARD:
                                relatedPages = CancelFluxFAReportDeclarationDiscardOperation(fishingActivity, relatedReportId);
                                break;
                            case FaTypes.ARRIVAL:
                                relatedPages = CancelFluxFAReportDeclarationArrival(referencedIdType);
                                break;
                            case FaTypes.LANDING:
                                relatedPages = CancelFluxFAReportDeclarationLanding(fishingActivity, documentOccurrenceDateTime);
                                break;
                        }
                    }
                    else
                    {
                        string fishingTrip = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
                        Logger.LogWarning($"{LOGGER_MSG_TYPE} Unknown FLUX_FA_TYPE ({fishingActivity.TypeCode.Value}), in trip: {fishingTrip}");
                    }
                }
            }

            return relatedPages;
        }

        public List<ShipLogBookPage> MapFluxFAReportDeclarationDelete(IDType[] documentId, IDType referenceId, DateTime documentOccurrenceDateTime)
        {
            List<ShipLogBookPage> relatedPages = new List<ShipLogBookPage>();

            FvmsfishingActivityReport referencedReport = GetDocumentByUUID((Guid)referenceId);
            FLUXFAReportMessageType referencedMessage = CommonUtils.JsonDeserialize<FLUXFAReportMessageType>(referencedReport.ResponseMessage);

            foreach (FAReportDocumentType faReportDocument in referencedMessage.FAReportDocument)
            {
                foreach (FishingActivityType fishingActivity in faReportDocument.SpecifiedFishingActivity)
                {
                    IDType[] relatedReportId = null;

                    if (faReportDocument.RelatedReportID != null && !CompareIDTypes(faReportDocument.RelatedReportID, documentId))
                    {
                        relatedReportId = faReportDocument.RelatedReportID;
                    }

                    FaTypes faType;
                    bool isSuccessfulFaTypeCast = Enum.TryParse<FaTypes>(fishingActivity.TypeCode.Value, out faType);
                    if (isSuccessfulFaTypeCast)
                    {
                        switch (faType)
                        {
                            case FaTypes.DEPARTURE:
                                relatedPages = DeleteFluxFAReportDeclarationDeparture(referenceId);
                                break;
                            case FaTypes.AREA_ENTRY:
                            case FaTypes.AREA_EXIT:
                            case FaTypes.JOINT_FISHING_OPERATION:
                            case FaTypes.START_ACTIVITY:
                            case FaTypes.RELOCATION:
                                // not relevant to our db for mapping
                                break;
                            case FaTypes.FISHING_OPERATION:
                                relatedPages = DeleteFluxFaReportDeclartionFishingOperation(fishingActivity, relatedReportId);
                                break;
                            case FaTypes.GEAR_SHOT:
                                relatedPages = DeleteFluxFAReportDeclarationGearShot(fishingActivity);
                                break;
                            case FaTypes.GEAR_RETRIEVAL:
                                relatedPages = DeleteFluxFAReportDeclarationGearRetrieval(fishingActivity, relatedReportId);
                                break;
                            case FaTypes.TRANSHIPMENT:
                                // TODO
                                break;
                            case FaTypes.DISCARD:
                                relatedPages = DeleteFluxFAReportDeclarationDiscardOperation(fishingActivity, relatedReportId);
                                break;
                            case FaTypes.ARRIVAL:
                                relatedPages = DeleteFluxFAReportDeclarationArrival(referenceId);
                                break;
                            case FaTypes.LANDING:
                                relatedPages = DeleteFluxFAReportDeclarationLanding(fishingActivity, documentOccurrenceDateTime);
                                break;
                        }
                    }
                    else
                    {
                        string fishingTrip = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
                        Logger.LogWarning($"{LOGGER_MSG_TYPE} Unknown FLUX_FA_TYPE ({fishingActivity.TypeCode.Value}), in trip: {fishingTrip}");
                    }
                }
            }

            return relatedPages;
        }

        public FvmsfishingActivityReport AddFVMSFishingActivityReport(FLUXFAReportMessageType faReportMessage, CodeType faReportType, CodeType faType)
        {
            DateTime now = DateTime.Now;
            FLUXReportDocumentType FLUXReportDocument = faReportMessage.FLUXReportDocument;

            Guid id = (Guid)FLUXReportDocument.ID[0];
            Guid? referencedId = FLUXReportDocument.ReferencedID != null ? (Guid)FLUXReportDocument.ReferencedID : default(Guid?);

            List<int> shipIds = GetShipIds(faReportMessage.FAReportDocument[0].SpecifiedVesselTransportMeans, referencedId);

            int mdrFluxFaReportTypeId = (from fluxFaReportType in Db.MdrFluxFaReportTypes
                                         where fluxFaReportType.Code == faReportType.Value
                                               && fluxFaReportType.ValidFrom <= now
                                               && fluxFaReportType.ValidTo > now
                                         select fluxFaReportType.Id).First();

            int mdrFluxFaTypeId = (from fluxFaType in Db.MdrFluxFaTypes
                                   where fluxFaType.Code == faType.Value
                                         && fluxFaType.ValidFrom <= now
                                         && fluxFaType.ValidTo > now
                                   select fluxFaType.Id).First();

            int mdrFluxGpPurposeId = (from fluxGpPurpose in Db.MdrFluxGpPurposes
                                      where fluxGpPurpose.Code == faReportMessage.FLUXReportDocument.PurposeCode.Value
                                            && fluxGpPurpose.ValidFrom <= now
                                            && fluxGpPurpose.ValidTo > now
                                      select fluxGpPurpose.Id).First();

            FvmsfishingActivityReport fvmsFishingActivityReport = new FvmsfishingActivityReport
            {
                ResponseUuid = id,
                ReferencedResponseUuid = referencedId,
                ResponseMessage = CommonUtils.JsonSerialize(faReportMessage),
                MdrFluxFaReportTypeId = mdrFluxFaReportTypeId, // TODO make this string or create FK to MdrFluxFaReportTypes
                VesselId = shipIds.FirstOrDefault(),
                MdrFluxFaTypeId = mdrFluxFaTypeId, // TODO make this string or create FK to MdrFluxFaTypes
                MdrFluxGpPurposeId = mdrFluxGpPurposeId // TODO make this string or create FK to MdrFluxGpPurposes
            };

            Db.FvmsfishingActivityReports.Add(fvmsFishingActivityReport);

            return fvmsFishingActivityReport;
        }

        public FvmsfishingActivityReportLogBookPage AddFvmsFishingActivityReportLogBookPage(FvmsfishingActivityReport fvmsFishingActivityReport,
                                                                                            ShipLogBookPage shipLogBookPage,
                                                                                            string tripIdentifier)
        {
            FvmsfishingActivityReportLogBookPage fvmsFishingActivityReportLogBookPage = new FvmsfishingActivityReportLogBookPage
            {
                FishingActivityReport = fvmsFishingActivityReport,
                ShipLogBookPage = shipLogBookPage,
                TripIdentifier = tripIdentifier
            };

            Db.FvmsfishingActivityReportLogBookPages.Add(fvmsFishingActivityReportLogBookPage);

            return fvmsFishingActivityReportLogBookPage;
        }

        // common helper methods

        public string GetFishingTripIdentifier(FishingTripType fishingTrip)
        {
            IDType idType = fishingTrip.ID.Where(x => x.schemeID == nameof(FaTripIdTypes.EU_TRIP_ID)).Single();
            return idType.Value;
        }

        public FvmsfishingActivityReport GetDocumentByUUID(Guid id)
        {
            FvmsfishingActivityReport fvmsFishingActivityReport = (from fvmsActivityReport in Db.FvmsfishingActivityReports
                                                                   where fvmsActivityReport.ResponseUuid == id
                                                                         && fvmsActivityReport.IsActive
                                                                   select fvmsActivityReport).First();

            return fvmsFishingActivityReport;
        }

        private int? GetPortId(FLUXLocationType[] relatedFluxLocations)
        {
            DateTime now = DateTime.Now;
            int? portId = default;

            foreach (FLUXLocationType fluxLocation in relatedFluxLocations)
            {
                if (fluxLocation.TypeCode != null)
                {
                    if (fluxLocation.TypeCode.Value == nameof(FluxLocationTypes.LOCATION))
                    {
                        if (fluxLocation.ID != null && fluxLocation.ID.schemeID == nameof(FluxLocationIdentifierTypes.LOCATION))
                        {
                            string mdrLocationCode = fluxLocation.ID.Value;
                            portId = (from port in Db.Nports
                                      where port.Code == fluxLocation.ID.Value && port.ValidFrom <= now && port.ValidTo > now
                                      select port.Id).SingleOrDefault(); // should aways be a match, if the data is only for BGR
                        }
                        else
                        {
                            portId = -1;
                            Logger.LogWarning($"{LOGGER_MSG_TYPE} NO port provided in LOCATION type in fluxLocation object for Arrival FA");
                        }
                    }
                }
                else
                {
                    portId = -1;
                    Logger.LogWarning($"{LOGGER_MSG_TYPE} NO port provided in LOCATION type in fluxLocation object for Arrival FA");
                }
            }

            return portId;
        }

        /// <summary>
        /// Gets FISHING_DEPTH characteric (if any) from array of FLUXCharacteristicType characteristics
        /// </summary>
        /// <param name="specifiedFLUXCharacteristic">array of specified Flux characteristics</param>
        /// <returns></returns>
        private decimal? GetFishingGearDepth(FLUXCharacteristicType[] specifiedFLUXCharacteristic)
        {
            decimal? fishingGearDepth = null;
            if (specifiedFLUXCharacteristic != null)
            {
                foreach (var characteristic in specifiedFLUXCharacteristic)
                {
                    if (characteristic.TypeCode.Value == nameof(FaCharacteristicCodes.FISHING_DEPTH))
                    {
                        fishingGearDepth = characteristic.ValueMeasure.Value;
                        break;
                    }
                }
            }

            return fishingGearDepth;
        }

        private OriginDeclarationData MapFaCatchToOriginDeclaration(FishingActivityType fishingActivity,
                                                                    List<FACatchType> faCatches,
                                                                    UnloadingTypesEnum unloadingType,
                                                                    DateTime documentOccurrenceDateTime)
        {
            OriginDeclarationData originDeclartionsData = GetOriginDeclarationsData(fishingActivity, faCatches, unloadingType, documentOccurrenceDateTime);

            foreach (OriginDeclaration originDeclaration in originDeclartionsData.OriginDeclarations)
            {
                List<OriginDeclarationFish> originDeclarationFishes = originDeclartionsData.LogBookPageOriginDeclarationFishes[originDeclaration.LogBookPageId];
                foreach (OriginDeclarationFish originDeclarationFish in originDeclarationFishes)
                {
                    originDeclaration.OriginDeclarationFishes.Add(originDeclarationFish);
                }
            }

            List<OriginDeclaration> originDeclarationsToAdd = originDeclartionsData.OriginDeclarations.Where(x => x.OriginDeclarationFishes.Any())
                                                                                                      .ToList();
            Db.OriginDeclarations.AddRange(originDeclarationsToAdd);

            return originDeclartionsData;
        }

        private OriginDeclarationData GetOriginDeclarationsData(FishingActivityType fishingActivity,
                                                                List<FACatchType> faCatches,
                                                                UnloadingTypesEnum unloadingType,
                                                                DateTime documentOccurrenceDateTime)
        {
            DateTime now = DateTime.Now;

            HashSet<string> unloadedCatchesFishCodes = faCatches.Select(x => x.SpeciesCode.Value).ToHashSet();
            string fishingTrip = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
            HashSet<int> shipLogBookPageIds = GetShipLogBookPageIdsForTrip(fishingTrip);

            List<CatchRecordFishHelper> catchRecordFishesData = (from catchRecordFish in Db.CatchRecordFish
                                                                 join aquaticOrganism in Db.Nfishes on catchRecordFish.FishId equals aquaticOrganism.Id
                                                                 join mdrFaoSpecies in Db.MdrFaoSpecies on aquaticOrganism.MdrFaoSpeciesId equals mdrFaoSpecies.Id
                                                                 join catchSize in Db.NfishSizes on catchRecordFish.CatchSizeId equals catchSize.Id into cSize
                                                                 from catchSize in cSize.DefaultIfEmpty()
                                                                 join mdrFishSizeClass in Db.MdrFishSizeClasses on catchSize.MdrFishSizeClassId equals mdrFishSizeClass.Id into mdrCSize
                                                                 from mdrFishSizeClass in mdrCSize.DefaultIfEmpty()
                                                                 join catchRecord in Db.CatchRecords on catchRecordFish.CatchRecordId equals catchRecord.Id
                                                                 join catchZone in Db.NcatchZones on catchRecordFish.CatchZoneId equals catchZone.Id
                                                                 join shipLogBookPage in Db.ShipLogBookPages on catchRecord.LogBookPageId equals shipLogBookPage.Id
                                                                 join originDeclaration in Db.OriginDeclarations on shipLogBookPage.Id equals originDeclaration.LogBookPageId into od
                                                                 from originDeclaration in od.DefaultIfEmpty()
                                                                 where shipLogBookPageIds.Contains(shipLogBookPage.Id)
                                                                       && unloadedCatchesFishCodes.Contains(mdrFaoSpecies.Code)
                                                                       && mdrFaoSpecies.ValidFrom <= now
                                                                       && mdrFaoSpecies.ValidTo > now
                                                                       && aquaticOrganism.ValidFrom <= now
                                                                       && aquaticOrganism.ValidTo > now
                                                                       && (mdrFishSizeClass == null || (mdrFishSizeClass.ValidFrom <= now && mdrFishSizeClass.ValidTo > now))
                                                                       && (catchRecordFish.UnloadedQuantity < catchRecordFish.Quantity
                                                                            || (catchRecordFish.UnloadedQuantity == 0 && catchRecordFish.Quantity == 0))
                                                                 select new CatchRecordFishHelper
                                                                 {
                                                                     LogBookPageId = catchRecord.LogBookPageId,
                                                                     CatchRecordId = catchRecord.Id,
                                                                     CatchRecordFishId = catchRecordFish.Id,
                                                                     CatchZoneId = catchRecordFish.CatchZoneId,
                                                                     faCatchZoneCode = catchZone.Gfcmquadrant,
                                                                     OriginDeclarationId = originDeclaration != null ? originDeclaration.Id : default(int?),
                                                                     FishId = catchRecordFish.FishId,
                                                                     SpeciesCode = mdrFaoSpecies.Code,
                                                                     Quantity = catchRecordFish.Quantity,
                                                                     CatchSizeId = catchRecordFish.CatchSizeId.Value,
                                                                     FaCatchSizeTypeCode = mdrFishSizeClass != null ? mdrFishSizeClass.Code : null,
                                                                     UnloadedQuantity = catchRecordFish.UnloadedQuantity,
                                                                     FishingGearRegisterId = shipLogBookPage.FishingGearRegisterId.Value
                                                                 }).ToList();

            List<OriginDeclaration> originDeclarations = new List<OriginDeclaration>();

            HashSet<int> logBookPageIds = catchRecordFishesData.Select(x => x.LogBookPageId).ToHashSet();
            foreach (int logBookPageId in logBookPageIds)
            {
                originDeclarations.Add(new OriginDeclaration { LogBookPageId = logBookPageId });
            }

            int unloadTypeId = (from unloadType in Db.NcatchFishUnloadTypes
                                where unloadType.Code == unloadingType.ToString()
                                      && unloadType.ValidFrom <= now
                                      && unloadType.ValidTo > now
                                select unloadType.Id).Single();

            IDictionary<int, List<OriginDeclarationFish>> logBookPageOriginDeclarationFishes = new Dictionary<int, List<OriginDeclarationFish>>();

            DateTime startDateTime;
            if (fishingActivity.SpecifiedDelimitedPeriod != null)
            {
                startDateTime = (DateTime)fishingActivity.SpecifiedDelimitedPeriod[0].StartDateTime;
            }
            else if (fishingActivity.OccurrenceDateTime != null)
            {
                startDateTime = fishingActivity.OccurrenceDateTime.Item;
            }
            else
            {
                startDateTime = documentOccurrenceDateTime;
            }

            List<int> transhipmenShipIds = new List<int>();
            if (unloadingType == UnloadingTypesEnum.TRN)
            {
                transhipmenShipIds = GetShipIds(fishingActivity.RelatedVesselTransportMeans[0]);
            }

            int? portId = GetPortId(fishingActivity.RelatedFLUXLocation);

            foreach (FACatchType faCatch in faCatches.OrderByDescending(x => x.WeightMeasure.Value))
            {
                AAPProcessType[] appliedProcesses = faCatch.AppliedAAPProcess;

                int? zoneId = default;
                if (faCatch.SpecifiedFLUXLocation != null)
                {
                    zoneId = GetCatchLocationZoneId(faCatch.SpecifiedFLUXLocation);
                }

                FishingGearCharacteristics fishingGearCharacteristics = GetFluxFishingGearCharacteristics(faCatch.UsedFishingGear[0].ApplicableGearCharacteristic);
                FishingGearDataHelper fishingGearData = GetFishingGearData(faCatch.UsedFishingGear[0], fishingGearCharacteristics);

                string faCatchSizeTypeCode = null;
                if (faCatch.SpecifiedSizeDistribution != null && faCatch.SpecifiedSizeDistribution.ClassCode != null && faCatch.SpecifiedSizeDistribution.ClassCode.Length > 0)
                {
                    faCatchSizeTypeCode = faCatch.SpecifiedSizeDistribution.ClassCode[0].Value;
                }

                List<CatchRecordFishHelper> catchRecordFishesForUnloading = (from catchRecordFishData in catchRecordFishesData
                                                                             where catchRecordFishData.SpeciesCode == faCatch.SpeciesCode.Value
                                                                                   && (!zoneId.HasValue || catchRecordFishData.CatchZoneId == zoneId)
                                                                                   && catchRecordFishData.FaCatchSizeTypeCode == faCatchSizeTypeCode
                                                                                   && (catchRecordFishData.UnloadedQuantity < catchRecordFishData.Quantity
                                                                                        || (catchRecordFishData.UnloadedQuantity == 0 && catchRecordFishData.Quantity == 0))
                                                                                   && catchRecordFishData.FishingGearRegisterId == fishingGearData.FishingGearRegisterId
                                                                             orderby catchRecordFishData.Quantity descending,
                                                                                     catchRecordFishData.UnloadedQuantity ascending
                                                                             select catchRecordFishData).ToList();
                int? fishPresentationId = default, fishFreshnessId = default;
                bool isProcessedOnBoard;
                decimal quantityForProcessing;

                if (appliedProcesses != null && appliedProcesses.Length > 0) // the fish is processed on board
                {
                    isProcessedOnBoard = true;

                    foreach (AAPProcessType appliedProcess in appliedProcesses)
                    {
                        quantityForProcessing = appliedProcess.ResultAAPProduct.Sum(x => x.WeightMeasure.Value);

                        foreach (CodeType codeType in appliedProcess.TypeCode)
                        {
                            FluxProcessTypes processType;
                            bool processTypesuccessfulCast = Enum.TryParse<FluxProcessTypes>(codeType.listID, out processType);

                            if (processTypesuccessfulCast)
                            {
                                switch (processType)
                                {
                                    case FluxProcessTypes.FISH_FRESHNESS:
                                        fishFreshnessId = GetFishFreshnessId(codeType.Value);
                                        break;
                                    case FluxProcessTypes.FISH_PRESENTATION:
                                        fishPresentationId = GetFishPresentationId(codeType.Value);
                                        break;
                                    case FluxProcessTypes.FISH_PRESERVATION:
                                        // nothing to do for now (no table for preservation in our DB)
                                        break;
                                }
                            }
                        }

                        if (catchRecordFishesForUnloading.Count > 0)
                        {
                            AddLogBookPageOriginDeclarationFishes(logBookPageOriginDeclarationFishes,
                                                                  catchRecordFishesForUnloading,
                                                                  new OriginDeclarationFishHelper
                                                                  {
                                                                      FishFreshnessId = fishFreshnessId,
                                                                      FishPresentationId = fishPresentationId,
                                                                      IsProcessedOnBoard = isProcessedOnBoard,
                                                                      PortId = portId,
                                                                      QuantityForProcessing = quantityForProcessing,
                                                                      StartDateTime = startDateTime,
                                                                      TranshipmenShipIds = transhipmenShipIds,
                                                                      UnloadingType = unloadingType,
                                                                      UnloadTypeId = unloadTypeId
                                                                  });
                        }
                        else // The fish was not a part of a fishing operation with catch
                        {
                            AddOriginDeclarationFishWithoutCatchRecord(faCatch.SpeciesCode.Value,
                                                                       logBookPageIds,
                                                                       fishingGearData,
                                                                       logBookPageOriginDeclarationFishes,
                                                                       quantityForProcessing,
                                                                       zoneId,
                                                                       new OriginDeclarationFishHelper
                                                                       {
                                                                           FishFreshnessId = fishFreshnessId,
                                                                           FishPresentationId = fishPresentationId,
                                                                           IsProcessedOnBoard = isProcessedOnBoard,
                                                                           PortId = portId,
                                                                           QuantityForProcessing = quantityForProcessing,
                                                                           StartDateTime = startDateTime,
                                                                           TranshipmenShipIds = transhipmenShipIds,
                                                                           UnloadingType = unloadingType,
                                                                           UnloadTypeId = unloadTypeId
                                                                       });
                        }
                    }
                }
                else
                {
                    quantityForProcessing = faCatch.WeightMeasure.Value;
                    isProcessedOnBoard = false;

                    fishPresentationId = (from presentation in Db.NfishPresentations
                                          join mdrFishPresentation in Db.MdrFishPresentations on presentation.MdrFishPresentationId equals mdrFishPresentation.Id
                                          where mdrFishPresentation.Code == nameof(FishPresentationCodesEnum.WHL)
                                                && presentation.ValidFrom <= now
                                                && presentation.ValidTo > now
                                                && mdrFishPresentation.ValidFrom <= now
                                                && mdrFishPresentation.ValidTo > now
                                          select presentation.Id).Single();

                    if (catchRecordFishesForUnloading.Count > 0)
                    {
                        AddLogBookPageOriginDeclarationFishes(logBookPageOriginDeclarationFishes,
                                                              catchRecordFishesForUnloading,
                                                              new OriginDeclarationFishHelper
                                                              {
                                                                  FishFreshnessId = fishFreshnessId,
                                                                  FishPresentationId = fishPresentationId,
                                                                  IsProcessedOnBoard = isProcessedOnBoard,
                                                                  PortId = portId,
                                                                  QuantityForProcessing = quantityForProcessing,
                                                                  StartDateTime = startDateTime,
                                                                  TranshipmenShipIds = transhipmenShipIds,
                                                                  UnloadingType = unloadingType,
                                                                  UnloadTypeId = unloadTypeId
                                                              });
                    }
                    else // The fish was not a part of a fishing operation with catch
                    {
                        AddOriginDeclarationFishWithoutCatchRecord(faCatch.SpeciesCode.Value,
                                                                   logBookPageIds,
                                                                   fishingGearData,
                                                                   logBookPageOriginDeclarationFishes,
                                                                   quantityForProcessing,
                                                                   zoneId,
                                                                   new OriginDeclarationFishHelper
                                                                   {
                                                                       FishFreshnessId = fishFreshnessId,
                                                                       FishPresentationId = fishPresentationId,
                                                                       IsProcessedOnBoard = isProcessedOnBoard,
                                                                       PortId = portId,
                                                                       QuantityForProcessing = quantityForProcessing,
                                                                       StartDateTime = startDateTime,
                                                                       TranshipmenShipIds = transhipmenShipIds,
                                                                       UnloadingType = unloadingType,
                                                                       UnloadTypeId = unloadTypeId
                                                                   });
                    }
                }
            }

            OriginDeclarationData originDeclarationData = new OriginDeclarationData
            {
                OriginDeclarations = originDeclarations,
                LogBookPageOriginDeclarationFishes = logBookPageOriginDeclarationFishes
            };

            return originDeclarationData;
        }

        private int GetAquaticOrganismId(string faSpeciesCode)
        {
            DateTime now = DateTime.Now;
            int aquaticOrganismId = (from fish in Db.Nfishes
                                     join mdrFaoSpecies in Db.MdrFaoSpecies on fish.MdrFaoSpeciesId equals mdrFaoSpecies.Id
                                     where mdrFaoSpecies.Code == faSpeciesCode
                                           && mdrFaoSpecies.ValidFrom <= now
                                           && mdrFaoSpecies.ValidTo > now
                                           && fish.ValidFrom <= now
                                           && fish.ValidTo > now
                                     select fish.Id).Single();

            return aquaticOrganismId;
        }

        private int? GetCatchSizeId(SizeDistributionType sizeDistributionType)
        {
            int? catchSizeId = default;
            string faSizeDistributionCode = null;

            if (sizeDistributionType != null && sizeDistributionType.ClassCode != null && sizeDistributionType.ClassCode.Length > 0)
            {
                faSizeDistributionCode = sizeDistributionType.ClassCode[0].Value;
            }

            if (!string.IsNullOrEmpty(faSizeDistributionCode))
            {
                DateTime now = DateTime.Now;
                catchSizeId = (from catchSize in Db.NfishSizes
                               join mdrFishSizeClass in Db.MdrFishSizeClasses on catchSize.MdrFishSizeClassId equals mdrFishSizeClass.Id
                               where mdrFishSizeClass.Code == faSizeDistributionCode
                                     && mdrFishSizeClass.ValidFrom <= now
                                     && mdrFishSizeClass.ValidTo > now
                                     && catchSize.ValidFrom <= now
                                     && catchSize.ValidTo > now
                               select catchSize.Id).SingleOrDefault();
            }

            return catchSizeId;
        }

        private int? GetCatchTypeId(string faCatchTypeCode)
        {
            DateTime now = DateTime.Now;
            int? catchTypeId = (from catchType in Db.NcatchTypes
                                join mdrFaCatchType in Db.MdrFaCatchTypes on catchType.MdrFaCatchTypeId equals mdrFaCatchType.Id
                                where mdrFaCatchType.Code == faCatchTypeCode
                                      && mdrFaCatchType.ValidFrom <= now
                                      && mdrFaCatchType.ValidTo > now
                                      && catchType.ValidFrom <= now
                                      && catchType.ValidTo > now
                                select catchType.Id).SingleOrDefault();

            return catchTypeId;
        }

        private int GetCatchLocationZoneId(FLUXLocationType[] specifiedFLUXLocations)
        {
            DateTime now = DateTime.Now;
            int? zoneId = default;

            foreach (FLUXLocationType fluxLocation in specifiedFLUXLocations)
            {
                if (fluxLocation.TypeCode.Value == nameof(FluxLocationTypes.AREA))
                {
                    if (fluxLocation.ID.schemeID == nameof(FluxLocationIdentifierTypes.STAT_RECTANGLE))
                    {
                        string mdrLocationCode = fluxLocation.ID.Value;
                        zoneId = (from zone in Db.NcatchZones
                                  where zone.Gfcmquadrant == mdrLocationCode && zone.ValidFrom <= now && zone.ValidTo > now
                                  select zone.Id).SingleOrDefault();
                    }
                }
            }

            return zoneId ?? -1;
        }

        private List<int> GetShipIds(VesselTransportMeansType specifiedVesselTransportMeans, Guid? referencedId = null)
        {
            if (specifiedVesselTransportMeans == null && referencedId.HasValue)
            {
                FvmsfishingActivityReport fvmsRelatedActivityReport = GetDocumentByUUID(referencedId.Value);
                FLUXFAReportMessageType faReportMessage = CommonUtils.JsonDeserialize<FLUXFAReportMessageType>(fvmsRelatedActivityReport.ResponseMessage);
                specifiedVesselTransportMeans = faReportMessage.FAReportDocument[0].SpecifiedVesselTransportMeans;
            }

            IDType cfrIdType = specifiedVesselTransportMeans.ID.Where(x => x.schemeID == IDTypes.CFR).SingleOrDefault();
            IDType uviIdType = specifiedVesselTransportMeans.ID.Where(x => x.schemeID == IDTypes.UVI).SingleOrDefault();
            IDType ircsIdType = specifiedVesselTransportMeans.ID.Where(x => x.schemeID == IDTypes.IRCS).SingleOrDefault();
            IDType extMarkIdType = specifiedVesselTransportMeans.ID.Where(x => x.schemeID == IDTypes.EXT_MARK).SingleOrDefault();

            string cfr = cfrIdType != null ? cfrIdType.Value : null;
            string uvi = uviIdType != null ? uviIdType.Value : null;
            string ircs = ircsIdType != null ? ircsIdType.Value : null;
            string extMark = extMarkIdType != null ? extMarkIdType.Value : null;

            if (string.IsNullOrWhiteSpace(cfr) && string.IsNullOrWhiteSpace(uvi) && string.IsNullOrWhiteSpace(ircs) && string.IsNullOrWhiteSpace(extMark))
            { // if every idType is null -> throw error maybe ???
                Logger.LogWarning($"{LOGGER_MSG_TYPE} there are no ids with which to identify vessel.");
            }

            List<int> shipIds = GetShipIdsFiltered(cfr, uvi, ircs, extMark);

            return shipIds;
        }

        private HashSet<int> GetShipLogBookPageIdsForTrip(string fishingTripIdentifier)
        {
            HashSet<int> shipLogBookPageIds = (from fishingActivityReportLogBookPage in Db.FvmsfishingActivityReportLogBookPages
                                               join shipLogBookPage in Db.ShipLogBookPages on fishingActivityReportLogBookPage.ShipLogBookPageId equals shipLogBookPage.Id
                                               join logBook in Db.LogBooks on shipLogBookPage.LogBookId equals logBook.Id
                                               where fishingActivityReportLogBookPage.TripIdentifier == fishingTripIdentifier
                                                     && fishingActivityReportLogBookPage.IsActive
                                                     && shipLogBookPage.IsActive
                                                     && logBook.IsActive
                                               select fishingActivityReportLogBookPage.ShipLogBookPageId).ToHashSet();

            return shipLogBookPageIds;
        }

        private FishingGearDataHelper GetFishingGearData(FishingGearType fishingGear, FishingGearCharacteristics gearCharacteristics)
        {
            DateTime now = DateTime.Now;

            List<FishingGearDataHelper> fishingGearsData = (from fg in Db.NfishingGears
                                                            join mdrFishingGear in Db.MdrGearTypes on fg.MdrGearTypeId equals mdrFishingGear.Id
                                                            join fishingGearType in Db.NfishingGearTypes on fg.GearTypeId equals fishingGearType.Id
                                                            join fishingGearRegister in Db.FishingGearRegisters on fg.Id equals fishingGearRegister.FishingGearTypeId
                                                            join permitLicense in Db.CommercialFishingPermitLicensesRegisters on fishingGearRegister.PermitLicenseId equals permitLicense.Id
                                                            where fg.ValidFrom <= now
                                                                  && fg.ValidTo > now
                                                                  && mdrFishingGear.ValidFrom <= now
                                                                  && mdrFishingGear.ValidTo > now
                                                                  && fishingGearType.ValidFrom <= now
                                                                  && fishingGearType.ValidTo > now
                                                                  && mdrFishingGear.Code == fishingGear.TypeCode.Value
                                                                  && permitLicense.RegistrationNum == gearCharacteristics.PermitLicenseNumber
                                                                  && permitLicense.RecordType == nameof(RecordTypesEnum.Register)
                                                                  && fishingGearRegister.NetEyeSize == gearCharacteristics.MeshSize
                                                                  && (gearCharacteristics.LengthOrWidth <= 0 || fishingGearRegister.Length == gearCharacteristics.LengthOrWidth)
                                                                  && fishingGearRegister.HookCount == gearCharacteristics.HooksCount
                                                                  && (gearCharacteristics.Height <= 0 || fishingGearRegister.Height == gearCharacteristics.Height)
                                                                  && (gearCharacteristics.LinesCount <= 0 || fishingGearRegister.LineCount == gearCharacteristics.LinesCount)
                                                                  && permitLicense.IsActive
                                                            orderby fishingGearRegister.Id descending
                                                            select new FishingGearDataHelper
                                                            {
                                                                FishingGearId = fg.Id,
                                                                FishingGearRegisterId = fishingGearRegister.Id,
                                                                HasHooks = fishingGearType.HasHooks
                                                            }).ToList();

            if (fishingGearsData.Count > 1)
            {
                Logger.LogWarning($"{LOGGER_MSG_TYPE} the fishing gear with NetEyeSize {gearCharacteristics.MeshSize} is more than one in permit license with number: {gearCharacteristics.PermitLicenseNumber}.");
            }

            FishingGearDataHelper fishingGearData = fishingGearsData.First();

            return fishingGearData;
        }

        private HashSet<int> GetRelatedShipLogBookPagesByFVMSFAReportId(int id)
        {
            HashSet<int> ids = (from fvmsReportShipPage in Db.FvmsfishingActivityReportLogBookPages
                                where fvmsReportShipPage.FishingActivityReportId == id
                                      && fvmsReportShipPage.IsActive
                                select fvmsReportShipPage.ShipLogBookPageId).ToHashSet();

            return ids;
        }

        private FishingGearCharacteristics GetFluxFishingGearCharacteristics(GearCharacteristicType[] applicableGearCharacteristics)
        {
            FishingGearCharacteristics characteristics = new FishingGearCharacteristics();

            foreach (GearCharacteristicType gearCharacteristic in applicableGearCharacteristics)
            {
                FaGearCharacteristicCodes gearCharacteristicCode;
                bool successfulCastGearCode = Enum.TryParse<FaGearCharacteristicCodes>(gearCharacteristic.TypeCode.Value, out gearCharacteristicCode);

                if (successfulCastGearCode)
                {
                    switch (gearCharacteristicCode)
                    {
                        case FaGearCharacteristicCodes.QG:
                            characteristics.GearCount = (int)gearCharacteristic.ValueQuantity.Value;
                            break;
                        case FaGearCharacteristicCodes.GN:
                            characteristics.HooksCount = (int)gearCharacteristic.ValueQuantity.Value;
                            break;
                        case FaGearCharacteristicCodes.ME:
                            characteristics.MeshSize = gearCharacteristic.ValueMeasure.Value;
                            break;
                        case FaGearCharacteristicCodes.GD:
                            characteristics.PermitLicenseNumber = gearCharacteristic.Value.Value;
                            break;
                        case FaGearCharacteristicCodes.GM:
                            characteristics.LengthOrWidth = gearCharacteristic.ValueMeasure.Value;
                            break;
                        case FaGearCharacteristicCodes.HE:
                            characteristics.Height = gearCharacteristic.ValueMeasure.Value;
                            break;
                        case FaGearCharacteristicCodes.NL:
                            characteristics.NominalHeight = gearCharacteristic.ValueMeasure.Value;
                            break;
                        case FaGearCharacteristicCodes.NI:
                            characteristics.LinesCount = (int)gearCharacteristic.ValueQuantity.Value;
                            break;
                        case FaGearCharacteristicCodes.NN:
                            characteristics.NetsCount = (int)gearCharacteristic.ValueQuantity.Value;
                            break;
                        case FaGearCharacteristicCodes.MT:
                            characteristics.TrawlModel = gearCharacteristic.Value.Value;
                            break;
                    }
                }
            }

            return characteristics;
        }

        private ShipLogBookPage GetShipLogBookPageById(int id)
        {
            ShipLogBookPage shipLogBookPage = (from page in Db.ShipLogBookPages
                                               where page.Id == id
                                               select page).First();

            return shipLogBookPage;
        }

        private void AddOriginDeclarationFishWithoutCatchRecord(string speciesCode,
                                                               HashSet<int> logBookPageIds,
                                                               FishingGearDataHelper fishingGearData,
                                                               IDictionary<int, List<OriginDeclarationFish>> logBookPageOriginDeclarationFishes,
                                                               decimal quantityForTranshipment,
                                                               int? zoneId,
                                                               OriginDeclarationFishHelper originDeclarationFishHelper)
        {
            int fishId = GetAquaticOrganismId(speciesCode);
            int logBookPageId = (from shipLogBookPage in Db.ShipLogBookPages
                                 where logBookPageIds.Contains(shipLogBookPage.Id)
                                       && shipLogBookPage.FishingGearRegisterId == fishingGearData.FishingGearRegisterId
                                 select shipLogBookPage.Id).Single();

            AddLogBookPageOriginDeclarationFishes(logBookPageOriginDeclarationFishes,
                                                  new List<CatchRecordFishHelper>
                                                  {
                                                        new CatchRecordFishHelper
                                                        {
                                                            UnloadedQuantity = 0,
                                                            Quantity = quantityForTranshipment,
                                                            CatchZoneId = zoneId ?? -1,
                                                            FishId = fishId,
                                                            SpeciesCode = speciesCode,
                                                            LogBookPageId = logBookPageId,
                                                            FishingGearRegisterId = fishingGearData.FishingGearRegisterId
                                                        }
                                                  },
                                                  originDeclarationFishHelper);
        }

        private void AddLogBookPageOriginDeclarationFishes(IDictionary<int, List<OriginDeclarationFish>> logBookPageOriginDeclarationFishes,
                                                           List<CatchRecordFishHelper> catchRecordFishesForUnloading,
                                                           OriginDeclarationFishHelper originDeclarationFishData)
        {
            int catchesForUnloading = catchRecordFishesForUnloading.Count;
            for (int i = 0; i < catchesForUnloading; i++)
            {
                CatchRecordFishHelper catchRecordFishData = catchRecordFishesForUnloading[i];
                AddSingleLogBookPageOriginDeclarationFish(catchRecordFishData, originDeclarationFishData, logBookPageOriginDeclarationFishes);

                if (originDeclarationFishData.QuantityForProcessing == 0) // Разтоварено е цялото количество и може да се приключи с цикъла
                {
                    break;
                }
                // На последната уловена риба сме, а не е разтоварено всичкото количество, което е отбелязано за разтоварване
                else if (i == catchesForUnloading - 1 && originDeclarationFishData.QuantityForProcessing > 0)
                {
                    List<OriginDeclarationFish> addedOriginDeclarationFishes;
                    logBookPageOriginDeclarationFishes.TryGetValue(catchRecordFishData.LogBookPageId, out addedOriginDeclarationFishes);

                    if (addedOriginDeclarationFishes != null && addedOriginDeclarationFishes.Count > 0)
                    {
                        // Добавяне неразтовареното към последното разтоварване, за да се впише някъде, въпреки че не е уловено количеството
                        OriginDeclarationFish lastOriginDeclarationFish = addedOriginDeclarationFishes.Where(x => x.FishId == catchRecordFishData.FishId)
                                                                                                      .OrderBy(x => x.Quantity)
                                                                                                      .First();
                        lastOriginDeclarationFish.Quantity += originDeclarationFishData.QuantityForProcessing;
                    }
                }
            }
        }

        private void AddSingleLogBookPageOriginDeclarationFish(CatchRecordFishHelper catchRecordFish,
                                                               OriginDeclarationFishHelper originDeclarationFishData,
                                                               IDictionary<int, List<OriginDeclarationFish>> logBookPageOriginDeclarationFishes)
        {
            decimal quantityForUnloading = catchRecordFish.Quantity - catchRecordFish.UnloadedQuantity;

            OriginDeclarationFish originDeclarationFish = new OriginDeclarationFish
            {
                CatchRecordFishId = catchRecordFish.CatchRecordFishId,
                FishId = catchRecordFish.FishId,
                UnloadTypeId = originDeclarationFishData.UnloadTypeId,
                CatchFishPresentationId = originDeclarationFishData.FishPresentationId ?? -1,
                CatchFishFreshnessId = originDeclarationFishData.FishFreshnessId ?? -1,
                IsProcessedOnBoard = originDeclarationFishData.IsProcessedOnBoard
            };

            if (originDeclarationFishData.UnloadingType == UnloadingTypesEnum.TRN)
            {
                originDeclarationFish.TransboardDateTime = originDeclarationFishData.StartDateTime;
                originDeclarationFish.TransboardShipId = originDeclarationFishData.TranshipmenShipIds.FirstOrDefault();
                originDeclarationFish.TransboardTargetPortId = originDeclarationFishData.PortId;
            }
            else
            {
                originDeclarationFish.UnloadPortId = originDeclarationFishData.PortId;
                originDeclarationFish.UnloadDateTime = originDeclarationFishData.StartDateTime;
            }

            if (quantityForUnloading >= originDeclarationFishData.QuantityForProcessing)
            {
                originDeclarationFish.Quantity = originDeclarationFishData.QuantityForProcessing;
                catchRecordFish.UnloadedQuantity += originDeclarationFishData.QuantityForProcessing;
                originDeclarationFishData.QuantityForProcessing -= quantityForUnloading;
            }
            else
            {
                originDeclarationFish.Quantity = catchRecordFish.Quantity;
                originDeclarationFishData.QuantityForProcessing -= catchRecordFish.Quantity;
                catchRecordFish.UnloadedQuantity = catchRecordFish.Quantity;
            }

            if (logBookPageOriginDeclarationFishes.ContainsKey(catchRecordFish.LogBookPageId))
            {
                logBookPageOriginDeclarationFishes[catchRecordFish.LogBookPageId].Add(originDeclarationFish);
            }
            else
            {
                logBookPageOriginDeclarationFishes.Add(catchRecordFish.LogBookPageId, new List<OriginDeclarationFish> { originDeclarationFish });
            }
        }

        private int GetFishPresentationId(string faFishPresentationCode)
        {
            DateTime now = DateTime.Now;
            int fishPresentationId = (from presentation in Db.NfishPresentations
                                      join mdrFishPresentation in Db.MdrFishPresentations on presentation.MdrFishPresentationId equals mdrFishPresentation.Id
                                      where mdrFishPresentation.Code == faFishPresentationCode
                                            && presentation.ValidFrom <= now
                                            && presentation.ValidTo > now
                                            && mdrFishPresentation.ValidFrom <= now
                                            && mdrFishPresentation.ValidTo > now
                                      select presentation.Id).Single();

            return fishPresentationId;
        }

        private int GetFishFreshnessId(string faFishFreshnessCode)
        {
            DateTime now = DateTime.Now;
            int fishFreshnessId = (from freshness in Db.NfishFreshnesses
                                   join mdrFishFreshness in Db.MdrFishFreshnesses on freshness.MdrFishFreshnessId equals mdrFishFreshness.Id
                                   where mdrFishFreshness.Code == faFishFreshnessCode
                                         && freshness.ValidFrom <= now
                                         && freshness.ValidTo > now
                                         && mdrFishFreshness.ValidFrom <= now
                                         && mdrFishFreshness.ValidFrom > now
                                   select freshness.Id).Single();

            return fishFreshnessId;
        }

        private int? MapSpecifiedFishingGear(FishingGearType[] specifiedFishingGears)
        {
            int? specifiedFishingGearRegisterId = default;

            if (specifiedFishingGears != null && specifiedFishingGears[0] != null)
            {
                FishingGearCharacteristics fishingGearCharacteristics = GetFluxFishingGearCharacteristics(specifiedFishingGears[0].ApplicableGearCharacteristic);
                FishingGearDataHelper fishingGearData = GetFishingGearData(specifiedFishingGears[0], fishingGearCharacteristics);
                specifiedFishingGearRegisterId = fishingGearData.FishingGearRegisterId;
            }

            return specifiedFishingGearRegisterId;
        }

        private List<int> GetShipIdsFiltered(string cfr, string uvi, string ircs, string extMark)
        {
            List<int> shipIds;

            var shipData = from ship in Db.ShipsRegister
                           select new
                           {
                               Id = ship.Id,
                               Cfr = ship.Cfr,
                               Uvi = ship.Uvi,
                               Ircs = ship.IrcscallSign,
                               ExternalMark = ship.ExternalMark
                           };

            if (!string.IsNullOrEmpty(cfr))
            {
                shipData = from ship in shipData
                           where ship.Cfr == cfr
                           select ship;
            }

            // Засега ще филтрираме само по CFR - би трябвало да е винаги наличен

            //if (!string.IsNullOrEmpty(uvi))
            //{
            //    shipData = from ship in shipData
            //               where ship.Uvi == uvi
            //               select ship;
            //}

            //if (!string.IsNullOrEmpty(ircs))
            //{
            //    shipData = from ship in shipData
            //               where ship.Ircs == ircs
            //               select ship;
            //}

            //if (!string.IsNullOrEmpty(extMark))
            //{
            //    shipData = from ship in shipData
            //               where ship.ExternalMark == extMark
            //               select ship;
            //}

            shipIds = (from ship in shipData
                       orderby ship.Id descending
                       select ship.Id).ToList();

            return shipIds;
        }

        private bool CompareIDTypes(IDType[] first, IDType[] second)
        {
            if (first.Length != second.Length)
            {
                return false;
            }

            for (int i = 0; i < first.Length; i++)
            {
                if (!first[i].Equals(second[i]))
                {
                    return false;
                }
            }

            return true;
        }
    }

    internal class FishingGearCharacteristics
    {
        public int? GearCount { get; set; }

        public decimal? MeshSize { get; set; }

        public int? HooksCount { get; set; }

        public string PermitLicenseNumber { get; set; }

        public decimal LengthOrWidth { get; set; }

        public decimal Height { get; set; }

        public decimal NominalHeight { get; set; }

        public int LinesCount { get; set; }

        public int NetsCount { get; set; }

        public string TrawlModel { get; set; }
    }

    internal class FishingGearDataHelper
    {
        public int FishingGearId { get; set; }

        public int FishingGearRegisterId { get; set; }

        public bool HasHooks { get; set; }
    }

    internal class CatchRecordFishHelper
    {
        public int LogBookPageId { get; set; }
        public int? CatchRecordId { get; set; }
        public int? CatchRecordFishId { get; set; }
        public int FishId { get; set; }
        public string SpeciesCode { get; set; }
        public decimal Quantity { get; set; }
        public decimal UnloadedQuantity { get; set; }
        public int CatchZoneId { get; set; }
        public string faCatchZoneCode { get; set; }
        public int? OriginDeclarationId { get; set; }
        public int CatchSizeId { get; set; }
        public string FaCatchSizeTypeCode { get; set; }
        public int FishingGearRegisterId { get; set; }
    }

    internal class OriginDeclarationFishHelper
    {
        public int UnloadTypeId { get; set; }
        public int? FishPresentationId { get; set; }
        public int? FishFreshnessId { get; set; }
        public bool IsProcessedOnBoard { get; set; }
        public UnloadingTypesEnum UnloadingType { get; set; }
        public DateTime StartDateTime { get; set; }
        public List<int> TranshipmenShipIds { get; set; }
        public int? PortId { get; set; }
        public decimal QuantityForProcessing { get; set; }
    }

    internal class FishingGearCatchRecord
    {
        public int? FishingGearRegisterId { get; set; }

        public CatchRecord CatchRecord { get; set; }

        public string FishingTripIdentifier { get; set; }
    }

    internal class OriginDeclarationData
    {
        public List<OriginDeclaration> OriginDeclarations { get; set; }

        public IDictionary<int, List<OriginDeclarationFish>> LogBookPageOriginDeclarationFishes { get; set; }
    }
}
