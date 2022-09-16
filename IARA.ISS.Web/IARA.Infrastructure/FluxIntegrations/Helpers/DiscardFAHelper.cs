using System;
using System.Linq;
using System.Collections.Generic;
using IARA.EntityModels.Entities;
using IARA.Flux.Models;
using IARA.Common.Enums;
using TL.SysToSysSecCom;
using IARA.FluxModels.Enums;

namespace IARA.Infrastructure.FluxIntegrations.Helpers
{
    internal partial class FishingActivityHelper
    {
        private List<ShipLogBookPage> MapFluxFAReportDeclarationDiscardOperation(IDType[] relatedFLUXReportDocumentIds, FishingActivityType fishingActivity)
        {
            List<ShipLogBookPage> shipLogBookPages = new List<ShipLogBookPage>();
            DateTime now = DateTime.Now;

            List<TripCatchRecordFishData> catchRecordFishesData = GetTripCatchRecordFishesData(fishingActivity, relatedFLUXReportDocumentIds);

            foreach (FACatchType specifiedCatch in fishingActivity.SpecifiedFACatch)
            {
                string catchCode = specifiedCatch.SpeciesCode.Value;
                decimal discardQuantity = specifiedCatch.WeightMeasure.Value;

                string fishSizeClass = null;
                if (specifiedCatch.SpecifiedSizeDistribution != null && specifiedCatch.SpecifiedSizeDistribution.ClassCode != null && specifiedCatch.SpecifiedSizeDistribution.ClassCode.Length > 0)
                {
                    fishSizeClass = specifiedCatch.SpecifiedSizeDistribution.ClassCode[0].Value;
                }

                string faCatchTypeCode = specifiedCatch.TypeCode.Value;

                int catchTypeId = (from catchType in Db.NcatchTypes
                                   join mdrFaCatchType in Db.MdrFaCatchTypes on catchType.MdrFaCatchTypeId equals mdrFaCatchType.Id
                                   where mdrFaCatchType.Code == faCatchTypeCode
                                         && mdrFaCatchType.ValidFrom <= now
                                         && mdrFaCatchType.ValidTo > now
                                         && catchType.ValidFrom <= now
                                         && catchType.ValidTo > now
                                   select catchType.Id).Single();

                var catchRecordFishes = (from catchRecordFish in catchRecordFishesData
                                         where catchRecordFish.CatchCode == catchCode
                                               && (string.IsNullOrEmpty(catchRecordFish.CatchSizeClass) || catchRecordFish.CatchSizeClass == fishSizeClass)
                                         select new
                                         {
                                             Id = catchRecordFish.Id,
                                             Quantity = catchRecordFish.Quantity,
                                             ShipLogBookPageId = catchRecordFish.ShipLogBookPageId
                                         }).ToList();

                var catchRecordFishExactQuantity = catchRecordFishes.Where(x => x.Quantity == discardQuantity).FirstOrDefault();
                if (catchRecordFishExactQuantity != null)
                {
                    CatchRecordFish discardedCatchRecordFish = (from crFish in Db.CatchRecordFish
                                                                where crFish.Id == catchRecordFishExactQuantity.Id
                                                                select crFish).Single();

                    discardedCatchRecordFish.CatchTypeId = catchTypeId;

                    ShipLogBookPage shipLogBookPage = GetShipLogBookPageById(catchRecordFishExactQuantity.ShipLogBookPageId);
                    shipLogBookPages.Add(shipLogBookPage);
                }
                else // the discarded fish is in more than one CatchRecordFish (or just a part of one with bigger quantity)
                {
                    var catchRecordFishExceedQuantity = catchRecordFishes.Where(x => x.Quantity > discardQuantity).FirstOrDefault();
                    if (catchRecordFishExceedQuantity != null) // the discarded fish is a part of one with bigger quantity
                    {
                        CatchRecordFish discardedCatchRecordFish = (from crFish in Db.CatchRecordFish
                                                                    where crFish.Id == catchRecordFishExceedQuantity.Id
                                                                    select crFish).Single();

                        HandleCatchRecordFishExeedsDiscardQuantity(discardedCatchRecordFish, discardQuantity, catchTypeId);

                        ShipLogBookPage shipLogBookPage = GetShipLogBookPageById(catchRecordFishExceedQuantity.ShipLogBookPageId);
                        shipLogBookPages.Add(shipLogBookPage);
                    }
                    else // the discarded fish is in more than one CatchRecordFish
                    {
                        var catchRecordFishesBelowQuantity = catchRecordFishes.Where(x => x.Quantity < discardQuantity).ToList();
                        foreach (var catchRecordFish in catchRecordFishesBelowQuantity)
                        {
                            CatchRecordFish discardedCatchRecordFish = (from crFish in Db.CatchRecordFish
                                                                        where crFish.Id == catchRecordFish.Id
                                                                        select crFish).Single();

                            if (discardedCatchRecordFish.Quantity == discardQuantity) // quantity has become equal
                            {
                                discardedCatchRecordFish.CatchTypeId = catchTypeId;
                                discardQuantity -= discardedCatchRecordFish.Quantity;
                            }
                            else if (discardedCatchRecordFish.Quantity > discardQuantity) // quantity is bigger
                            {
                                HandleCatchRecordFishExeedsDiscardQuantity(discardedCatchRecordFish, discardQuantity, catchTypeId);
                                discardQuantity = 0;
                            }
                            else // quantity is smaller
                            {
                                discardedCatchRecordFish.CatchTypeId = catchTypeId;
                                discardQuantity -= discardedCatchRecordFish.Quantity;
                            }

                            int shipLogBookPageId = (from catchRecord in Db.CatchRecords
                                                     where catchRecord.Id == discardedCatchRecordFish.CatchRecordId
                                                     select catchRecord.LogBookPageId).First();

                            ShipLogBookPage shipLogBookPage = GetShipLogBookPageById(shipLogBookPageId);
                            shipLogBookPages.Add(shipLogBookPage);

                            if (discardQuantity == 0)
                            {
                                break;
                            }
                        }
                    }
                }
            }

            return shipLogBookPages;
        }

        private List<ShipLogBookPage> UpdateFluxFAReportDeclarationDiscardOperation(FishingActivityType fishingActivity,
                                                                                    IDType[] relatedFLUXReportDocumentIds,
                                                                                    IDType referenceId)
        {
            // Make the already discarded fish on board

            FvmsfishingActivityReport referencedReport = GetDocumentByUUID((Guid)referenceId);
            FLUXFAReportMessageType referencedMessage = CommonUtils.JsonDeserialize<FLUXFAReportMessageType>(referencedReport.ResponseMessage);
            FishingActivityType prevFishingActivity = referencedMessage.FAReportDocument.SelectMany(x => x.SpecifiedFishingActivity)
                                                                                        .Where(x => x.TypeCode.Value == nameof(FaTypes.DISCARD))
                                                                                        .First();
            IDType[] prevRelatedReportIds = referencedMessage.FAReportDocument.Where(x => x.SpecifiedFishingActivity
                                                                                                   .Any(y => y.SpecifiedFishingTrip == prevFishingActivity.SpecifiedFishingTrip))
                                                                                      .SelectMany(x => x.RelatedReportID)
                                                                                      .ToArray();

            MakePreviouslyDiscardedFishOnBoardAgain(prevFishingActivity, prevRelatedReportIds);

            // Discard the new discarded caught fishes

            List<ShipLogBookPage> shipLogBookPages = MapFluxFAReportDeclarationDiscardOperation(relatedFLUXReportDocumentIds, fishingActivity);

            return shipLogBookPages;
        }

        private List<ShipLogBookPage> CancelFluxFAReportDeclarationDiscardOperation(FishingActivityType fishingActivity, IDType[] relatedReportIdentifiers)
        {
            List<ShipLogBookPage> relatedPages = new List<ShipLogBookPage>();

            HashSet<int> relatedCatchIds = MakePreviouslyDiscardedFishOnBoardAgain(fishingActivity, relatedReportIdentifiers); // Clears discarded fish, making it onboard again
            HashSet<int> relatedPageIds = (from page in Db.ShipLogBookPages
                                           join catchRecord in Db.CatchRecords on page.Id equals catchRecord.LogBookPageId
                                           where relatedCatchIds.Contains(catchRecord.Id)
                                           select page.Id).ToHashSet();

            relatedPages = (from page in Db.ShipLogBookPages
                            where relatedPageIds.Contains(page.Id)
                            select page).ToList();

            return relatedPages;
        }

        private List<ShipLogBookPage> DeleteFluxFAReportDeclarationDiscardOperation(FishingActivityType fishingActivity, IDType[] relatedReportIdentifiers)
        {
            List<ShipLogBookPage> relatedPages = new List<ShipLogBookPage>();

            HashSet<int> relatedCatchIds = MakePreviouslyDiscardedFishOnBoardAgain(fishingActivity, relatedReportIdentifiers); // Clears discarded fish, making it onboard again
            HashSet<int> relatedPageIds = (from page in Db.ShipLogBookPages
                                           join catchRecord in Db.CatchRecords on page.Id equals catchRecord.LogBookPageId
                                           where relatedCatchIds.Contains(catchRecord.Id)
                                           select page.Id).ToHashSet();

            relatedPages = (from page in Db.ShipLogBookPages
                            where relatedPageIds.Contains(page.Id)
                            select page).ToList();

            return relatedPages;
        }

        private HashSet<int> MakePreviouslyDiscardedFishOnBoardAgain(FishingActivityType fishingActivity, IDType[] relatedReportIdentifiers)
        {
            HashSet<int> relatedCatchIds = new HashSet<int>();
            DateTime now = DateTime.Now;
            List<string> catchTypes = new List<string>
            {
                nameof(CatchTypeCodesEnum.DISCARDED),
                nameof(CatchTypeCodesEnum.DEMINIMIS)
            };

            // Get previously discarded caught fishes

            var prevDiscardedFaCatchData = (from prevDiscardFaCatch in fishingActivity.SpecifiedFACatch
                                            select new
                                            {
                                                SpeciesCode = prevDiscardFaCatch.SpeciesCode.Value,
                                                CatchSizeClassCode = prevDiscardFaCatch.SpecifiedSizeDistribution.ClassCode[0].Value,
                                                Quantity = prevDiscardFaCatch.WeightMeasure.Value
                                            }).ToList();

            List<string> aquaticOrganismCodes = prevDiscardedFaCatchData.Select(x => x.SpeciesCode).ToList();

            List<TripCatchRecordFishData> prevDiscardedCatchRecordFishes = GetTripCatchRecordFishesData(fishingActivity,
                                                                                                        relatedReportIdentifiers,
                                                                                                        aquaticOrganismCodes,
                                                                                                        catchTypes);

            var prevDiscardActivityData = (from prevDiscardedFish in prevDiscardedCatchRecordFishes
                                           join prevDiscardActivity in prevDiscardedFaCatchData on prevDiscardedFish.CatchCode equals prevDiscardActivity.SpeciesCode
                                           where prevDiscardedFish.CatchSizeClass == prevDiscardActivity.CatchSizeClassCode
                                                 && prevDiscardedFish.Quantity == prevDiscardActivity.Quantity
                                           select prevDiscardActivity).ToLookup(x => x.SpeciesCode,
                                                                                y => new
                                                                                {
                                                                                    y.CatchSizeClassCode,
                                                                                    y.Quantity
                                                                                });

            List<int> prevCatchRecordFishIds = prevDiscardedCatchRecordFishes.Select(x => x.Id).ToList();

            var dbDiscardedCatchRecordFishesData = (from crFish in Db.CatchRecordFish
                                                    join fish in Db.Nfishes on crFish.FishId equals fish.Id
                                                    join catchSize in Db.NfishSizes on crFish.CatchSizeId equals catchSize.Id
                                                    where prevCatchRecordFishIds.Contains(crFish.Id)
                                                    select new
                                                    {
                                                        DbCatchrecordFish = crFish,
                                                        FishCode = fish.Code,
                                                        CatchSizeCode = catchSize.Code
                                                    }).ToList();

            // Set ONBOARD as catch type for each discarded previously in the corresponding message fishes

            int onBoardCatchTypeId = (from catchType in Db.NcatchTypes
                                      where catchType.Code == nameof(CatchTypeCodesEnum.ONBOARD)
                                            && catchType.ValidFrom <= now
                                            && catchType.ValidTo > now
                                      select catchType.Id).First();

            IDictionary<(string, string), decimal> madeOnBoardCatchFishQuantities = new Dictionary<(string, string), decimal>();

            foreach (var dbCatchRecordFishData in dbDiscardedCatchRecordFishesData)
            {
                var key = (dbCatchRecordFishData.FishCode, dbCatchRecordFishData.CatchSizeCode);
                var dbCatchRecordFish = dbCatchRecordFishData.DbCatchrecordFish;
                decimal totalDiscardedQuantity = prevDiscardActivityData[dbCatchRecordFishData.FishCode]
                                                                        .Where(x => x.CatchSizeClassCode == dbCatchRecordFishData.CatchSizeCode)
                                                                        .Select(x => x.Quantity).Sum();

                decimal madeOnBoardCatchFishQuantity;
                bool hasMadeOnBoard = madeOnBoardCatchFishQuantities.TryGetValue(key, out madeOnBoardCatchFishQuantity);
                if (!hasMadeOnBoard)
                {
                    dbCatchRecordFish.CatchTypeId = onBoardCatchTypeId;
                    madeOnBoardCatchFishQuantities.Add(key, dbCatchRecordFish.Quantity);

                    relatedCatchIds.Add(dbCatchRecordFish.CatchRecordId);
                }
                else if (madeOnBoardCatchFishQuantity < totalDiscardedQuantity)
                {
                    dbCatchRecordFish.CatchTypeId = onBoardCatchTypeId;
                    madeOnBoardCatchFishQuantities[key] = madeOnBoardCatchFishQuantity + dbCatchRecordFish.Quantity;

                    relatedCatchIds.Add(dbCatchRecordFish.CatchRecordId);
                }
            }

            return relatedCatchIds;
        }

        private List<TripCatchRecordFishData> GetTripCatchRecordFishesData(FishingActivityType fishingActivity,
                                                                           IDType[] relatedFLUXReportDocumentIds,
                                                                           List<string> aquaticOrganismCodes = null,
                                                                           List<string> catchTypeCodes = null)
        {
            DateTime now = DateTime.Now;

            string specifiedFishingTrip = null;
            HashSet<int> shipLogBookPageIds = new HashSet<int>();

            if (relatedFLUXReportDocumentIds != null)
            {
                Guid referencedId = (Guid)relatedFLUXReportDocumentIds.Where(x => x.schemeID == "UUID").Single();
                FvmsfishingActivityReport fvmsRelatedActivityReport = GetDocumentByUUID(referencedId);
                FLUXFAReportMessageType faReportMessage = CommonUtils.JsonDeserialize<FLUXFAReportMessageType>(fvmsRelatedActivityReport.ResponseMessage);

                if (faReportMessage.FAReportDocument.Length > 0 && faReportMessage.FAReportDocument[0].SpecifiedFishingActivity.Length > 0)
                {
                    specifiedFishingTrip = GetFishingTripIdentifier(faReportMessage.FAReportDocument[0].SpecifiedFishingActivity[0].SpecifiedFishingTrip);
                }
            }
            else
            {
                specifiedFishingTrip = GetFishingTripIdentifier(fishingActivity.SpecifiedFishingTrip);
            }

            if (!string.IsNullOrEmpty(specifiedFishingTrip))
            {
                shipLogBookPageIds = GetShipLogBookPageIdsForTrip(specifiedFishingTrip);
            }
            else
            {
                Logger.LogWarning($"{LOGGER_MSG_TYPE} no related document for discard operation + no fishing trip identifier in the fishing activity in the present message as well");
            }

            var catchRecordFishesData = (from shipLogBookPage in Db.ShipLogBookPages
                                         join catchRecord in Db.CatchRecords on shipLogBookPage.Id equals catchRecord.LogBookPageId
                                         join crFish in Db.CatchRecordFish on catchRecord.Id equals crFish.CatchRecordId
                                         join fish in Db.Nfishes on crFish.FishId equals fish.Id
                                         join mdrFaoSpecies in Db.MdrFaoSpecies on fish.MdrFaoSpeciesId equals mdrFaoSpecies.Id
                                         join catchSize in Db.NfishSizes on crFish.CatchSizeId equals catchSize.Id into cSize
                                         from catchSize in cSize.DefaultIfEmpty()
                                         join mdrFishSizeClass in Db.MdrFishSizeClasses on catchSize.MdrFishSizeClassId equals mdrFishSizeClass.Id into mdrCSize
                                         from mdrFishSizeClass in mdrCSize.DefaultIfEmpty()
                                         join catchType in Db.NcatchTypes on crFish.CatchTypeId equals catchType.Id
                                         where shipLogBookPageIds.Contains(shipLogBookPage.Id)
                                               && (catchTypeCodes == null || catchTypeCodes.Contains(catchType.Code))
                                               && (aquaticOrganismCodes == null || aquaticOrganismCodes.Contains(fish.Code))
                                               && mdrFaoSpecies.ValidFrom <= now
                                               && mdrFaoSpecies.ValidTo > now
                                               && fish.ValidFrom <= now
                                               && fish.ValidTo > now
                                         select new TripCatchRecordFishData
                                         {
                                             Id = crFish.Id,
                                             Quantity = crFish.Quantity,
                                             ShipLogBookPageId = shipLogBookPage.Id,
                                             CatchCode = mdrFaoSpecies.Code,
                                             CatchSizeClass = mdrFishSizeClass != null ? mdrFishSizeClass.Code : default
                                         }).ToList();

            return catchRecordFishesData;
        }

        private void HandleCatchRecordFishExeedsDiscardQuantity(CatchRecordFish discardedCatchRecordFish, decimal discardQuantity, int catchTypeId)
        {
            decimal newQuantity = discardedCatchRecordFish.Quantity - discardQuantity;
            discardedCatchRecordFish.Quantity = newQuantity;

            CatchRecordFish newDiscardedCatchRecordFish = new CatchRecordFish
            {
                CatchRecordId = discardedCatchRecordFish.CatchRecordId,
                CatchSizeId = discardedCatchRecordFish.CatchSizeId,
                CatchTypeId = catchTypeId,
                CatchZoneId = discardedCatchRecordFish.CatchZoneId,
                FishId = discardedCatchRecordFish.FishId,
                IsContinentalCatch = discardedCatchRecordFish.IsContinentalCatch,
                Quantity = newQuantity,
                SturgeonGender = discardedCatchRecordFish.SturgeonGender,
                SturgeonSize = discardedCatchRecordFish.SturgeonSize,
                SturgeonWeightKg = discardedCatchRecordFish.SturgeonWeightKg,
                ThirdCountryCatchZone = discardedCatchRecordFish.ThirdCountryCatchZone,
                TurbotCount = discardedCatchRecordFish.TurbotCount,
                TurbotSizeGroupId = discardedCatchRecordFish.TurbotSizeGroupId,
                IsActive = discardedCatchRecordFish.IsActive
            };

            Db.CatchRecordFish.Add(newDiscardedCatchRecordFish);
        }
    }

    internal class TripCatchRecordFishData
    {
        public int Id { get; set; }
        public decimal Quantity { get; set; }
        public int ShipLogBookPageId { get; set; }
        public string CatchCode { get; set; }
        public string CatchSizeClass { get; set; }
    }
}
