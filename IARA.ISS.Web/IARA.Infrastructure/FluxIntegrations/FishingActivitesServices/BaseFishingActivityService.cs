using System.Reflection.PortableExecutable;
using IARA.Flux.Models;
using IARA.FluxModels;
using IARA.FluxModels.Enums;
using IARA.Infrastructure.FluxIntegrations.FishingActivitesServices.Models;
using IARA.Infrastructure.FluxIntegrations.Helpers;
using TL.Logging.Abstractions.Interfaces;

namespace IARA.Infrastructure.FluxIntegrations.FishingActivitesServices
{
    internal abstract class BaseFishingActivityService
    {
        protected readonly IARADbContext Db;

        private readonly IExtendedLogger Logger;

        private const string LOGGER_MSG_TYPE = "FLUX FA DOMAIN";

        private readonly string _callerFileName;

        public BaseFishingActivityService(IARADbContext dbContext, IExtendedLogger logger, string callerFileName)
        {
            Db = dbContext;
            Logger = logger;
            _callerFileName = callerFileName;
        }

        protected void LogWarning(string message, string callerName)
        {
            Logger.LogWarning($"{LOGGER_MSG_TYPE}: {message}", _callerFileName, callerName);
        }

        protected string GetFishingTripIdentifier(FishingTripType fishingTrip)
        {
            IDType idType = fishingTrip.ID.Where(x => x.schemeID == nameof(FaTripIdTypes.EU_TRIP_ID)).Single();
            return idType.Value;
        }

        protected int GetPortId(FLUXLocationType[] relatedFluxLocations)
        {
            DateTime now = DateTime.Now;
            int portId = -1;

            FLUXLocationType portLocation = relatedFluxLocations.Where(x => x.TypeCode != null && x.TypeCode.Value == nameof(FluxLocationTypes.LOCATION))
                                                                .FirstOrDefault();

            if (portLocation != null)
            {
                if (portLocation.ID != null && portLocation.ID.schemeID == nameof(FluxLocationIdentifierTypes.LOCATION))
                {
                    string mdrLocationCode = portLocation.ID.Value;

                    portId = (from port in Db.Nports
                              where port.Code == portLocation.ID.Value && port.ValidFrom <= now && port.ValidTo > now
                              select (int?)port.Id).SingleOrDefault() ?? -1; // should aways be a match, if the data is only for BGR
                }
                else
                {
                    portId = -1;
                    LogWarning($"{LOGGER_MSG_TYPE} NO port provided in LOCATION type in fluxLocation object", "GetPortId");
                }
            }
            else
            {
                portId = -1;
                LogWarning($"{LOGGER_MSG_TYPE} NO port provided in LOCATION type in fluxLocation object", "GetPortId");
            }

            return portId;
        }

        protected FishingGearCharacteristics GetFluxFishingGearCharacteristics(GearCharacteristicType[] applicableGearCharacteristics)
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
                            {
                                characteristics.GearCount = (int)gearCharacteristic.ValueQuantity.Value;

                                if (characteristics.GearCount == 0)
                                {
                                    characteristics.GearCount = 1;
                                }
                            }
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
                            characteristics.NominalLength = gearCharacteristic.ValueMeasure.Value;
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
                        default:
                            LogWarning($"Unknown FaGearCharacteristicsCode: ${gearCharacteristicCode} for gear", "GetFluxFishingGearCharacteristics");
                            break;
                    }
                }
                else
                {
                    LogWarning($"Unknown FaGearCharacteristicCode: ${gearCharacteristic.TypeCode.Value}", "GetFluxFishingGearCharacteristics");
                }
            }

            return characteristics;
        }

        protected FishingGearData GetFishingGearData(FishingGearType fishingGear, FishingGearCharacteristics gearCharacteristics, FishingTripType specifiedFishingTrip)
        {
            DateTime now = DateTime.Now;
            string fishingGearCode = fishingGear.TypeCode.Value;

            List<FishingGearData> fishingGearsData = (from fg in Db.NfishingGears
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
                                                            && mdrFishingGear.Code == fishingGearCode
                                                            && permitLicense.RegistrationNum == gearCharacteristics.PermitLicenseNumber
                                                            && permitLicense.RecordType == nameof(RecordTypesEnum.Register)
                                                            && permitLicense.IsActive
                                                      orderby fishingGearRegister.Id descending
                                                      select new FishingGearData
                                                      {
                                                          FishingGearId = fg.Id,
                                                          FishingGearCode = fg.Code,
                                                          FishingGearRegisterId = fishingGearRegister.Id,
                                                          HasHooks = fishingGearType.HasHooks,
                                                          Height = fishingGearRegister.Height,
                                                          HooksCount = fishingGearRegister.HookCount,
                                                          LengthOrWidth = fishingGearRegister.Length,
                                                          LineCount = fishingGearRegister.LineCount,
                                                          NetEyeSize = fishingGearRegister.NetEyeSize,
                                                          NetNominalLength = fishingGearRegister.NetNominalLength,
                                                          NumberNetsInFleet = fishingGearRegister.NumberOfNetsInFleet,
                                                          TrawlModel = fishingGearRegister.TrawlModel,
                                                          CountOnBoard = fishingGearRegister.GearCount
                                                      }).ToList();

            if (fishingGearsData.Count > 1)
            {
                string fishingTrip = GetFishingTripIdentifier(specifiedFishingTrip);
                
                string msg = $"the fishing gear of code: {fishingGearCode} is more than one in permit license with number: {gearCharacteristics.PermitLicenseNumber}. FishingTripIdentifier: ${fishingTrip}";
                LogWarning(msg, "GetFishingGearData");

                // filter the gears by other characteristics and try again

                fishingGearsData = (from gear in fishingGearsData
                                    where (!FLUXValidationUtils.FishingGearCodesWithRequiredHeight.Contains(gear.FishingGearCode)
                                           || gear.Height == gearCharacteristics.Height
                                           || (!gear.Height.HasValue && gearCharacteristics.Height == 0)) // fallback case ...
                                           && (!FLUXValidationUtils.FishingGearCodesWithRequiredGearDimension.Contains(gear.FishingGearCode)
                                                 || gear.LengthOrWidth == gearCharacteristics.LengthOrWidth
                                                 || (!gear.LengthOrWidth.HasValue && gearCharacteristics.LengthOrWidth == 0)) // fallback case ...
                                           && (!FLUXValidationUtils.FishingGearCodesWithRequiredTrawlModel.Contains(gear.FishingGearCode)
                                                 || gear.TrawlModel == gearCharacteristics.TrawlModel
                                                 || (string.IsNullOrEmpty(gear.TrawlModel) && string.IsNullOrEmpty(gearCharacteristics.TrawlModel))) // fallback case ...
                                           && (!FLUXValidationUtils.FishingGearCodesWithRequiredNumberOfNets.Contains(gear.FishingGearCode)
                                                 || gear.NumberNetsInFleet == gearCharacteristics.NetsCount
                                                 || (!gear.NumberNetsInFleet.HasValue && gearCharacteristics.NetsCount == 0)) // fallback case ...
                                           && (!FLUXValidationUtils.FishingGearCodesWithRequiredNumberOfLines.Contains(gear.FishingGearCode)
                                                 || gear.LineCount == gearCharacteristics.LinesCount
                                                 || (!gear.LineCount.HasValue && gearCharacteristics.LinesCount == 0))  // fallback case ...
                                           && ((!FLUXValidationUtils.FishingGearCodesWithOptionalTrawlModel.Contains(gear.FishingGearCode)
                                                && !FLUXValidationUtils.FishingGearCodesWithOptionalGearDimension.Contains(gear.FishingGearCode))
                                                 || gear.TrawlModel == gearCharacteristics.TrawlModel
                                                 || gear.LengthOrWidth == gearCharacteristics.LengthOrWidth
                                                 || (string.IsNullOrEmpty(gear.TrawlModel) && string.IsNullOrEmpty(gearCharacteristics.TrawlModel) && !gear.LengthOrWidth.HasValue && gearCharacteristics.LengthOrWidth == 0)) // fallback case ...
                                           && (!FLUXValidationUtils.FishingGearCodesWithRequiredMeshSize.Contains(gear.FishingGearCode)
                                                 || gear.NetEyeSize == gearCharacteristics.MeshSize
                                                 || (!gear.NetEyeSize.HasValue && gearCharacteristics.MeshSize == 0)) // fallback case ...
                                           && (!FLUXValidationUtils.FishingGearCodesWithRequiredNetLength.Contains(gear.FishingGearCode)
                                                 || gear.NetNominalLength == gearCharacteristics.NominalLength
                                                 || (!gear.NetNominalLength.HasValue && gearCharacteristics.NominalLength == 0))
                                           && (!FLUXValidationUtils.FishingGearCodesWithRequiredNumberDimension.Contains(gear.FishingGearCode) // fallback case ...
                                                 || gear.HooksCount == gearCharacteristics.HooksCount // TODO maybe make HooksCount be just count ???
                                                 || (!gear.HooksCount.HasValue && gearCharacteristics.HooksCount == 0)) // fallback case ...
                                    select gear).ToList();

            }

            if (fishingGearsData.Count == 0)
            {
                string fishingTrip = GetFishingTripIdentifier(specifiedFishingTrip);
                
                string msg = $"there are NO fishing gears with code: {fishingGearCode} for permit license with number: {gearCharacteristics.PermitLicenseNumber}. FishingTripIdentifier: ${fishingTrip}";
                LogWarning(msg, "GetFishingGearData");

                throw new ArgumentException($"No Fishing gear with code {fishingGearCode} found for trip identifier: {fishingTrip}");
            }

            FishingGearData fishingGearData = fishingGearsData.First();

            return fishingGearData;
        }

        protected int GetPermitLicenseId(string permitLicenseNumber)
        {
            int permitLicenseId = (from permitLicense in Db.CommercialFishingPermitLicensesRegisters
                                   where permitLicense.RegistrationNum == permitLicenseNumber
                                         && permitLicense.IsActive
                                         && permitLicense.RecordType == nameof(RecordTypesEnum.Register)
                                   select permitLicense.Id).Single();

            return permitLicenseId;
        }

        protected HashSet<int> GetShipLogBookPageIdsForTrip(string fishingTripIdentifier)
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

        protected FvmsfishingActivityReport GetDocumentByUUID(Guid id)
        {
            FvmsfishingActivityReport fvmsFishingActivityReport = (from fvmsActivityReport in Db.FvmsfishingActivityReports
                                                                   where fvmsActivityReport.ResponseUuid == id
                                                                         && fvmsActivityReport.IsActive
                                                                   select fvmsActivityReport).First();

            return fvmsFishingActivityReport;
        }

        /// <summary>
        /// Gets FISHING_DEPTH characteric (if any) from array of FLUXCharacteristicType characteristics
        /// </summary>
        /// <param name="specifiedFLUXCharacteristic">array of specified Flux characteristics</param>
        /// <returns></returns>
        protected decimal? GetFishingGearDepth(FLUXCharacteristicType[] specifiedFLUXCharacteristic)
        {
            decimal? fishingGearDepth = null;

            if (specifiedFLUXCharacteristic != null)
            {
                var depthCharacteristic = specifiedFLUXCharacteristic.Where(x => x.TypeCode != null && x.TypeCode.Value == nameof(FaCharacteristicCodes.FISHING_DEPTH))
                                                                     .FirstOrDefault();

                if (depthCharacteristic != null)
                {
                    fishingGearDepth = depthCharacteristic.ValueMeasure.Value;
                }
                else
                {
                    LogWarning("No fishing depth provided for fishing operation", nameof(GetFishingGearDepth)); // TODO fishing trip ???
                }
            }

            return fishingGearDepth;
        }

        protected int? MapSpecifiedFishingGear(FishingGearType[] specifiedFishingGears, FishingActivityType fishingActivity)
        {
            int? specifiedFishingGearRegisterId = default;

            if (specifiedFishingGears != null && specifiedFishingGears[0] != null)
            {
                FishingGearCharacteristics fishingGearCharacteristics = GetFluxFishingGearCharacteristics(specifiedFishingGears[0].ApplicableGearCharacteristic);
                FishingGearData fishingGearData = GetFishingGearData(specifiedFishingGears[0], fishingGearCharacteristics, fishingActivity.SpecifiedFishingTrip);
                specifiedFishingGearRegisterId = fishingGearData.FishingGearRegisterId;
            }

            return specifiedFishingGearRegisterId;
        }

        protected List<ShipLogBookPage> GetReferenceIdRelatedLogBookPages(Guid referenceId)
        {
            FvmsfishingActivityReport referencedReport = GetDocumentByUUID(referenceId);
            HashSet<int> referencedShipLogBookPageIds = GetRelatedShipLogBookPagesByFVMSFAReportId(referencedReport.Id);

            List<ShipLogBookPage> relatedPages = (from logBookPage in Db.ShipLogBookPages
                                                  where referencedShipLogBookPageIds.Contains(logBookPage.Id)
                                                  select logBookPage).ToList();

            return relatedPages;
        }

        protected HashSet<int> GetRelatedShipLogBookPagesByFVMSFAReportId(int id)
        {
            HashSet<int> ids = (from fvmsReportShipPage in Db.FvmsfishingActivityReportLogBookPages
                                where fvmsReportShipPage.FishingActivityReportId == id
                                      && fvmsReportShipPage.IsActive
                                select fvmsReportShipPage.ShipLogBookPageId).ToHashSet();

            return ids;
        }

        protected List<ShipLogBookPage> GetTripRelatedLogBookPages(FishingActivityType fishingActivity)
        {
            string fishingTrip = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
            HashSet<int> relatedLogBookPageIds = GetShipLogBookPageIdsForTrip(fishingTrip);
            List<ShipLogBookPage> relatedPages = (from logBookPage in Db.ShipLogBookPages
                                                  where relatedLogBookPageIds.Contains(logBookPage.Id)
                                                  select logBookPage).ToList();

            return relatedPages;
        }
 
        protected ShipLogBookPage GetShipLogBookPageById(int id)
        {
            ShipLogBookPage shipLogBookPage = (from page in Db.ShipLogBookPages
                                               where page.Id == id
                                               select page).First();

            return shipLogBookPage;
        }

        protected int? GetCatchTypeId(string faCatchTypeCode)
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

        protected int GetCatchLocationZoneId(FLUXLocationType[] specifiedFLUXLocations)
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
                                  select (int?)zone.Id).SingleOrDefault();
                    }
                }
            }

            return zoneId ?? -1;
        }

        protected int GetAquaticOrganismId(string faSpeciesCode)
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

        protected int? GetCatchSizeId(SizeDistributionType sizeDistributionType)
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

        protected int? GetTurbotSizeGroup(SizeDistributionType specifiedSizeDistribution)
        {
            DateTime now = DateTime.Now;
            int? turbotSizeGroupId = default;

            if (specifiedSizeDistribution != null && specifiedSizeDistribution.CategoryCode != null) // Category code is actually for BFT, so no can do for turbot ...
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

        protected List<int> GetShipIds(VesselTransportMeansType specifiedVesselTransportMeans, Guid? referencedId = null)
        {
            if (specifiedVesselTransportMeans == null && referencedId.HasValue)
            {
                FvmsfishingActivityReport fvmsRelatedActivityReport = GetDocumentByUUID(referencedId.Value);
                FLUXFAReportMessageType faReportMessage = CommonUtils.Deserialize<FLUXFAReportMessageType>(fvmsRelatedActivityReport.ResponseMessage);
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
                LogWarning($"there are no ids with which to identify vessel.", "GetShipIds");
            }

            List<int> shipIds = GetShipIdsFiltered(cfr, uvi, ircs, extMark);

            return shipIds;
        }

        protected bool CompareIDTypes(IDType[] first, IDType[] second)
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
    }
}
