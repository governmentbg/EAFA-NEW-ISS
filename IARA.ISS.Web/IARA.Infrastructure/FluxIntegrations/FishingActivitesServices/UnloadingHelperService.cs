using IARA.Flux.Models;
using IARA.FluxModels.Enums;
using IARA.Infrastructure.FluxIntegrations.FishingActivitesServices.Models;
using IARA.Infrastructure.FluxIntegrations.Helpers;
using TL.Logging.Abstractions.Interfaces;

namespace IARA.Infrastructure.FluxIntegrations.FishingActivitesServices
{
    internal class UnloadingHelperService : BaseFishingActivityService
    {
        public UnloadingHelperService(IARADbContext dbContext, IExtendedLogger logger)
            : base(dbContext, logger, "UnloadingHelperService.cs")
        { }

        public OriginDeclarationsData MapFaCatchToOriginDeclaration(FishingActivityType fishingActivity,
                                                            List<FACatchType> faCatches,
                                                            UnloadingTypesEnum unloadingType,
                                                            DateTime documentOccurrenceDateTime)
        {
            OriginDeclarationsData originDeclartionsData = GetOriginDeclarationsData(fishingActivity, faCatches, unloadingType, documentOccurrenceDateTime);

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

        public OriginDeclarationsData GetOriginDeclarationsData(FishingActivityType fishingActivity,
                                                                List<FACatchType> faCatches,
                                                                UnloadingTypesEnum unloadingType,
                                                                DateTime documentOccurrenceDateTime)
        {
            DateTime now = DateTime.Now;

            HashSet<string> unloadedCatchesFishCodes = faCatches.Select(x => x.SpeciesCode.Value).ToHashSet();
            string fishingTrip = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
            HashSet<int> shipLogBookPageIds = GetShipLogBookPageIdsForTrip(fishingTrip);

            if (shipLogBookPageIds.Count == 0)
            {
                throw new ArgumentException($"No related ship log book pages found for fishing trip identifier: {fishingTrip}");
            }

            List<CatchRecordFishData> catchRecordFishesData = (from catchRecordFish in Db.CatchRecordFish
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
                                                                     && (catchRecordFish.UnloadedQuantity < catchRecordFish.Quantity
                                                                          || (catchRecordFish.UnloadedQuantity == 0 && catchRecordFish.Quantity == 0))
                                                                     && catchRecordFish.CatchZoneId.HasValue
                                                               select new CatchRecordFishData
                                                               {
                                                                   LogBookPageId = catchRecord.LogBookPageId,
                                                                   CatchRecordId = catchRecord.Id,
                                                                   CatchRecordFishId = catchRecordFish.Id,
                                                                   CatchZoneId = catchRecordFish.CatchZoneId.Value,
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

            if (catchRecordFishesData.Count == 0)
            {
                throw new ArgumentException($"No catch record fish data found for fishing trip identifier: {fishingTrip}. Unloaded Species codes are: {string.Join(';', unloadedCatchesFishCodes)}");
            }

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

            DateTime operationDateTime;
            if (fishingActivity.SpecifiedDelimitedPeriod != null)
            {
                operationDateTime = (DateTime)fishingActivity.SpecifiedDelimitedPeriod[0].EndDateTime;
            }
            else if (fishingActivity.OccurrenceDateTime != null)
            {
                operationDateTime = fishingActivity.OccurrenceDateTime.Item;
            }
            else
            {
                operationDateTime = documentOccurrenceDateTime;
            }

            List<int> transhipmenShipIds = new List<int>();
            if (unloadingType == UnloadingTypesEnum.TRN) // В случай на трансбордиране
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
                FishingGearData fishingGearData = GetFishingGearData(faCatch.UsedFishingGear[0], fishingGearCharacteristics, fishingActivity.SpecifiedFishingTrip);

                string faCatchSizeTypeCode = null;
                if (faCatch.SpecifiedSizeDistribution != null && faCatch.SpecifiedSizeDistribution.ClassCode != null && faCatch.SpecifiedSizeDistribution.ClassCode.Length > 0)
                {
                    faCatchSizeTypeCode = faCatch.SpecifiedSizeDistribution.ClassCode[0].Value;
                }

                List<CatchRecordFishData> catchRecordFishesForUnloading = (from catchRecordFishData in catchRecordFishesData
                                                                           where catchRecordFishData.SpeciesCode == faCatch.SpeciesCode.Value
                                                                                 && (!zoneId.HasValue
                                                                                      || zoneId == -1
                                                                                      || catchRecordFishData.CatchZoneId == -1
                                                                                      || catchRecordFishData.CatchZoneId == zoneId)
                                                                                 && catchRecordFishData.FaCatchSizeTypeCode == faCatchSizeTypeCode
                                                                                 && (catchRecordFishData.UnloadedQuantity < catchRecordFishData.Quantity
                                                                                      || (catchRecordFishData.UnloadedQuantity == 0 && catchRecordFishData.Quantity == 0))
                                                                                 && catchRecordFishData.FishingGearRegisterId == fishingGearData.FishingGearRegisterId
                                                                           orderby catchRecordFishData.Quantity descending,
                                                                                   catchRecordFishData.UnloadedQuantity ascending
                                                                           select catchRecordFishData).ToList();

                if (catchRecordFishesForUnloading.Count == 0)
                {
                    string msg = $"No catch record fish data found for unloaded species code: {faCatch.SpeciesCode.Value} and catch size type code: {faCatchSizeTypeCode} for fishing trip identifier: {fishingTrip}";
                    LogWarning(msg, "GetOriginDeclarationsData");
                }

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
                                                                  new OriginDeclarationFishData
                                                                  {
                                                                      FishFreshnessId = fishFreshnessId,
                                                                      FishPresentationId = fishPresentationId,
                                                                      IsProcessedOnBoard = isProcessedOnBoard,
                                                                      PortId = portId,
                                                                      QuantityForProcessing = quantityForProcessing,
                                                                      OperationDateTime = operationDateTime,
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
                                                                       new OriginDeclarationFishData
                                                                       {
                                                                           FishFreshnessId = fishFreshnessId,
                                                                           FishPresentationId = fishPresentationId,
                                                                           IsProcessedOnBoard = isProcessedOnBoard,
                                                                           PortId = portId,
                                                                           QuantityForProcessing = quantityForProcessing,
                                                                           OperationDateTime = operationDateTime,
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
                                                              new OriginDeclarationFishData
                                                              {
                                                                  FishFreshnessId = fishFreshnessId,
                                                                  FishPresentationId = fishPresentationId,
                                                                  IsProcessedOnBoard = isProcessedOnBoard,
                                                                  PortId = portId,
                                                                  QuantityForProcessing = quantityForProcessing,
                                                                  OperationDateTime = operationDateTime,
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
                                                                   new OriginDeclarationFishData
                                                                   {
                                                                       FishFreshnessId = fishFreshnessId,
                                                                       FishPresentationId = fishPresentationId,
                                                                       IsProcessedOnBoard = isProcessedOnBoard,
                                                                       PortId = portId,
                                                                       QuantityForProcessing = quantityForProcessing,
                                                                       OperationDateTime = operationDateTime,
                                                                       TranshipmenShipIds = transhipmenShipIds,
                                                                       UnloadingType = unloadingType,
                                                                       UnloadTypeId = unloadTypeId
                                                                   });
                    }
                }

                // Update CatchRecordFish unloaded quantities

                List<int> catchRecordFishIds = catchRecordFishesForUnloading.Where(x => x.CatchRecordFishId.HasValue)
                                                                            .Select(x => x.CatchRecordFishId.Value)
                                                                            .ToList();

                List<CatchRecordFish> usedCatchRecordFishes = (from catchRecordFish in Db.CatchRecordFish
                                                               where catchRecordFishIds.Contains(catchRecordFish.Id)
                                                               select catchRecordFish).ToList();

                foreach (CatchRecordFish usedCatchRecordFish in usedCatchRecordFishes)
                {
                    usedCatchRecordFish.UnloadedQuantity = catchRecordFishesForUnloading.Where(x => x.CatchRecordFishId == usedCatchRecordFish.Id)
                                                                                        .Single().UnloadedQuantity;
                }
            }

            OriginDeclarationsData originDeclarationData = new OriginDeclarationsData
            {
                OriginDeclarations = originDeclarations,
                LogBookPageOriginDeclarationFishes = logBookPageOriginDeclarationFishes
            };

            return originDeclarationData;
        }

        private void AddLogBookPageOriginDeclarationFishes(IDictionary<int, List<OriginDeclarationFish>> logBookPageOriginDeclarationFishes,
                                                           List<CatchRecordFishData> catchRecordFishesForUnloading,
                                                           OriginDeclarationFishData originDeclarationFishData)
        {
            CatchRecordFishData catchRecordFishData = catchRecordFishesForUnloading.Where(x => x.Quantity == originDeclarationFishData.QuantityForProcessing)
                                                                                   .FirstOrDefault();

            if (catchRecordFishData != null) // Има точното количество риба уловено, което е и разтоварено
            {
                AddSingleLogBookPageOriginDeclarationFish(catchRecordFishData, originDeclarationFishData, logBookPageOriginDeclarationFishes);
            }
            else // Няма точно количество и трябва да попълваме с няколко улова
            {
                int catchesForUnloading = catchRecordFishesForUnloading.Count;
                for (int i = 0; i < catchesForUnloading; i++)
                {
                    catchRecordFishData = catchRecordFishesForUnloading[i];
                    AddSingleLogBookPageOriginDeclarationFish(catchRecordFishData, originDeclarationFishData, logBookPageOriginDeclarationFishes);

                    if (originDeclarationFishData.QuantityForProcessing <= 0) // Разтоварено е цялото количество и може да се приключи с цикъла
                    {
                        break;
                    }

                    else if (i == catchesForUnloading - 1 && originDeclarationFishData.QuantityForProcessing > 0) // На последната уловена риба сме, а не е разтоварено всичкото количество, което е отбелязано за разтоварване
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
        }

        private void AddOriginDeclarationFishWithoutCatchRecord(string speciesCode,
                                                               HashSet<int> logBookPageIds,
                                                               FishingGearData fishingGearData,
                                                               IDictionary<int, List<OriginDeclarationFish>> logBookPageOriginDeclarationFishes,
                                                               decimal quantityForTranshipment,
                                                               int? zoneId,
                                                               OriginDeclarationFishData originDeclarationFishHelper)
        {
            int fishId = GetAquaticOrganismId(speciesCode);
            int logBookPageId = (from shipLogBookPage in Db.ShipLogBookPages
                                 where logBookPageIds.Contains(shipLogBookPage.Id)
                                       && shipLogBookPage.FishingGearRegisterId == fishingGearData.FishingGearRegisterId
                                 select shipLogBookPage.Id).SingleOrDefault();

            if (logBookPageId == default)
            {
                throw new ArgumentException($"Cannot find log book page between ids: {string.Join(';', logBookPageIds)} for fishing gear register id: {fishingGearData.FishingGearRegisterId}");
            }

            AddLogBookPageOriginDeclarationFishes(logBookPageOriginDeclarationFishes,
                                                  new List<CatchRecordFishData>
                                                  {
                                                        new CatchRecordFishData
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

        private void AddSingleLogBookPageOriginDeclarationFish(CatchRecordFishData catchRecordFish,
                                                               OriginDeclarationFishData originDeclarationFishData,
                                                               IDictionary<int, List<OriginDeclarationFish>> logBookPageOriginDeclarationFishes)
        {
            decimal freeQuantityForUnloading = catchRecordFish.Quantity - catchRecordFish.UnloadedQuantity;

            OriginDeclarationFish originDeclarationFish = new OriginDeclarationFish
            {
                CatchRecordFishId = catchRecordFish.CatchRecordFishId,
                FishId = catchRecordFish.FishId,
                UnloadTypeId = originDeclarationFishData.UnloadTypeId,
                CatchFishPresentationId = originDeclarationFishData.FishPresentationId ?? -1,
                CatchFishFreshnessId = originDeclarationFishData.FishFreshnessId ?? -1,
                IsProcessedOnBoard = originDeclarationFishData.IsProcessedOnBoard
            };

            if (originDeclarationFishData.UnloadingType == UnloadingTypesEnum.TRN) // В случай на трансбордиране
            {
                originDeclarationFish.TransboardDateTime = originDeclarationFishData.OperationDateTime;
                originDeclarationFish.TransboardShipId = originDeclarationFishData.TranshipmenShipIds.FirstOrDefault();
                originDeclarationFish.TransboardTargetPortId = originDeclarationFishData.PortId;
            }
            else
            {
                originDeclarationFish.UnloadPortId = originDeclarationFishData.PortId;
                originDeclarationFish.UnloadDateTime = originDeclarationFishData.OperationDateTime;
            }

            if (freeQuantityForUnloading >= originDeclarationFishData.QuantityForProcessing) // Ако свободното количество за текущия улов е равно или повече от това, което трябва да се разтовари
            {
                originDeclarationFish.Quantity = originDeclarationFishData.QuantityForProcessing;
                catchRecordFish.UnloadedQuantity += originDeclarationFishData.QuantityForProcessing;
                originDeclarationFishData.QuantityForProcessing -= freeQuantityForUnloading;
            }
            else // Свободното количество за текущия улов в по-малко от това, което трябва да се разтовари
            {
                originDeclarationFish.Quantity = catchRecordFish.Quantity;
                catchRecordFish.UnloadedQuantity = catchRecordFish.Quantity;
                originDeclarationFishData.QuantityForProcessing -= catchRecordFish.Quantity;
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
    }
}
