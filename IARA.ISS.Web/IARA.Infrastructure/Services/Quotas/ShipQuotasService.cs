using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IARA.Common.Constants;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.CatchQuotas;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.Excel.Tools.Interfaces;
using IARA.Excel.Tools.Models;
using IARA.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure.Services.Catches
{
    public class ShipQuotasService : Service, IShipQuotasService
    {
        private readonly IExcelExporterService excelExporterService;

        public ShipQuotasService(IARADbContext db, IExcelExporterService excelExporterService)
                 : base(db)
        {
            this.excelExporterService = excelExporterService;
        }

        public IQueryable<ShipQuotaDTO> GetAll(ShipQuotasFilters filters)
        {
            IQueryable<ShipQuotaDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                bool inactiveOnly = filters?.ShowInactiveRecords ?? false;
                result = GetAll(inactiveOnly);
            }
            else
            {
                result = filters.HasAnyFilters()
                    ? GetAllByFilter(filters)
                    : GetAllByFreeText(filters.FreeTextSearch, filters.ShowInactiveRecords);
            }

            return result;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            return GetSimpleEntityAuditValues(Db.ShipCatchQuotas, id);
        }

        public int Add(ShipQuotaEditDTO shipQuota)
        {
            ShipCatchQuota dbShipCatchQuota = (from shipCatchQuota in Db.ShipCatchQuotas
                                               where shipQuota.ShipId == shipCatchQuota.ShipId
                                                    && shipQuota.QuotaId == shipCatchQuota.CatchQuotaId
                                               select shipCatchQuota).SingleOrDefault();

            if (dbShipCatchQuota != null)
            {
                throw new ArgumentException("Cannot add existing ship catch quota");
            }

            ShipCatchQuota entry = new ShipCatchQuota()
            {
                CatchQuotaId = shipQuota.QuotaId.Value,
                ShipId = shipQuota.ShipId.Value,
                ShipQuotaSize = shipQuota.ShipQuotaSize.Value,
                IsActive = true
            };

            Db.ShipCatchQuotas.Add(entry);
            Db.SaveChanges();

            return entry.Id;
        }

        public List<QuotaHistDTO> GetHistoryForIds(IEnumerable<int> ids)
        {
            List<QuotaHistDTO> result = (from quotaHist in Db.ShipCatchQuotasHists
                                         where ids.Contains(quotaHist.ShipCatchQuotaId)
                                         orderby quotaHist.Id descending
                                         select new QuotaHistDTO()
                                         {
                                             Id = quotaHist.Id,
                                             QuotaId = quotaHist.ShipCatchQuotaId,
                                             AddRemoveQuota = quotaHist.ShipQuotaIncrement,
                                             NewQuotaValueKg = quotaHist.ShipQuotaSize,
                                             Basis = quotaHist.IncrementReason,
                                             Timestamp = quotaHist.ValidFrom
                                         }).ToList();

            foreach (QuotaHistDTO quota in result)
            {
                quota.UnloadedByCurrentDateKg = GetUnloadedByCurrentDate(quota.Id);
            }

            return result;
        }

        public ShipQuotaEditDTO Get(int id)
        {
            decimal unloadedByCurrentDateKg = GetUnloadedByCurrentDate(id);

            ShipQuotaEditDTO result = (from shipQuota in Db.ShipCatchQuotas
                                       join quota in Db.CatchQuotas on shipQuota.CatchQuotaId equals quota.Id
                                       join ship in Db.ShipsRegister on shipQuota.ShipId equals ship.Id
                                       join assoc in Db.NshipAssociations on ship.ShipAssociationId equals assoc.Id into assocMatchTable
                                       from assocMatch in assocMatchTable.DefaultIfEmpty()
                                       join fish in Db.Nfishes on quota.FishId equals fish.Id
                                       where shipQuota.Id == id
                                       select new ShipQuotaEditDTO
                                       {
                                           Id = shipQuota.Id,
                                           ShipId = shipQuota.ShipId,
                                           QuotaId = shipQuota.CatchQuotaId,
                                           ShipQuotaSize = shipQuota.ShipQuotaSize,
                                           UnloadedByCurrentDateKg = unloadedByCurrentDateKg,
                                           LeftoverQuotaSize = shipQuota.ShipQuotaSize - unloadedByCurrentDateKg
                                       }).First();

            return result;
        }

        public bool Edit(ShipQuotaEditDTO entry)
        {
            bool shoudUpdateShipsNomenclature = false;
            ShipCatchQuota dbQuota = Db.ShipCatchQuotas.First(x => x.Id == entry.Id);

            QuotaHistDTO newHistEntry = new QuotaHistDTO()
            {
                AddRemoveQuota = entry.ShipQuotaSize.Value - dbQuota.ShipQuotaSize,
                Basis = entry.ChangeBasis,
                NewQuotaValueKg = entry.ShipQuotaSize.Value,
                Timestamp = DateTime.Now,
                QuotaId = dbQuota.Id,
            };

            AddHist(newHistEntry, true);

            if (dbQuota.ShipId != entry.ShipId)
            {
                shoudUpdateShipsNomenclature = true;
            }

            dbQuota.CatchQuotaId = entry.QuotaId.Value;
            dbQuota.ShipId = entry.ShipId.Value;
            dbQuota.ShipQuotaSize = entry.ShipQuotaSize.Value;

            Db.SaveChanges();

            return shoudUpdateShipsNomenclature;
        }

        public void Delete(int id)
        {
            DeleteRecordWithId(Db.ShipCatchQuotas, id);
            Db.SaveChanges();
        }

        public void Restore(int id)
        {
            UndoDeleteRecordWithId(Db.ShipCatchQuotas, id);
            Db.SaveChanges();
        }

        public void Transfer(int newQuotaId, int oldQuotaId, int transferValue, string basis)
        {
            ShipCatchQuota newCatchQuota = (from quota in Db.ShipCatchQuotas
                                                .Include(x => x.CatchQuota)
                                            where quota.Id == newQuotaId
                                            select quota).First();

            ShipCatchQuota oldCatchQuota = (from quota in Db.ShipCatchQuotas
                                                .Include(x => x.CatchQuota)
                                            where quota.Id == oldQuotaId
                                            select quota).First();

            oldCatchQuota.ShipQuotaSize -= transferValue;
            newCatchQuota.ShipQuotaSize += transferValue;

            QuotaHistDTO newQuotaHistEntry = new QuotaHistDTO()
            {
                AddRemoveQuota = transferValue,
                Basis = basis,
                NewQuotaValueKg = newCatchQuota.ShipQuotaSize,
                Timestamp = DateTime.Now,
                QuotaId = newCatchQuota.Id,
                PeriodStart = newCatchQuota.CatchQuota.PeriodStart,
                PeriodEnd = newCatchQuota.CatchQuota.PeriodEnd
            };

            QuotaHistDTO oldQuotaHistEntry = new QuotaHistDTO()
            {
                AddRemoveQuota = transferValue,
                Basis = basis,
                NewQuotaValueKg = oldCatchQuota.ShipQuotaSize,
                Timestamp = DateTime.Now,
                QuotaId = oldCatchQuota.Id,
                PeriodStart = oldCatchQuota.CatchQuota.PeriodStart,
                PeriodEnd = oldCatchQuota.CatchQuota.PeriodEnd
            };

            AddHist(newQuotaHistEntry, true);
            AddHist(oldQuotaHistEntry, true);

            Db.SaveChanges();
        }

        public List<NomenclatureDTO> GetShipQuotasForList(int originalShipQuotaId)
        {
            ShipCatchQuota dbShipQuota = (from shipQuota in Db.ShipCatchQuotas
                                          .Include(x => x.CatchQuota)
                                          where shipQuota.Id == originalShipQuotaId
                                          select shipQuota).First();

            List<NomenclatureDTO> result = (from shipQuota in Db.ShipCatchQuotas
                                            join quota in Db.CatchQuotas on shipQuota.CatchQuotaId equals quota.Id
                                            join ship in Db.ShipsRegister on shipQuota.ShipId equals ship.Id
                                            join fish in Db.Nfishes on quota.FishId equals fish.Id
                                            where quota.PeriodEnd.Year == dbShipQuota.CatchQuota.PeriodEnd.Year
                                                   && fish.Id == dbShipQuota.CatchQuota.FishId
                                            orderby quota.PeriodEnd descending
                                            select new NomenclatureDTO
                                            {
                                                Value = shipQuota.Id,
                                                DisplayName = ship.Name + " - " + ship.Cfr,
                                                IsActive = shipQuota.IsActive,
                                            }).ToList();

            return result;
        }

        public Stream DownloadShipQuotaExcel(ExcelExporterRequestModel<ShipQuotasFilters> request)
        {
            ExcelExporterData<ShipQuotaDTO> data = new ExcelExporterData<ShipQuotaDTO>
            {
                PrimaryKey = nameof(ShipQuotaDTO.Id),
                Query = GetAll(request.Filters),
                HeaderNames = request.HeaderNames
            };

            return excelExporterService.BuildExcelFile(request, data);
        }

        private IQueryable<ShipQuotaDTO> GetAllByFilter(ShipQuotasFilters filters)
        {
            var shipQuotas = from shipQuota in Db.ShipCatchQuotas
                             join quota in Db.CatchQuotas on shipQuota.CatchQuotaId equals quota.Id
                             join ship in Db.ShipsRegister on shipQuota.ShipId equals ship.Id
                             join fish in Db.Nfishes on quota.FishId equals fish.Id
                             join assoc in Db.NshipAssociations on ship.ShipAssociationId equals assoc.Id into assocMatchTable
                             from assocMatch in assocMatchTable.DefaultIfEmpty()
                             where shipQuota.IsActive == !filters.ShowInactiveRecords
                             select new
                             {
                                 shipQuota.Id,
                                 shipQuota.CatchQuotaId,
                                 AssociationName = assocMatch.Name,
                                 ShipName = ship.Name,
                                 shipQuota.ShipId,
                                 ship.Cfr,
                                 ship.ExternalMark,
                                 quota.PeriodEnd.Year,
                                 FishId = fish.Id,
                                 FishName = fish.Name,
                                 fish.NameLatin,
                                 shipQuota.ShipQuotaSize,
                                 shipQuota.IsActive,
                             };

            if (filters.ShipId.HasValue)
            {
                shipQuotas = shipQuotas.Where(x => x.ShipId == filters.ShipId);
            }

            if (!string.IsNullOrEmpty(filters.CFR))
            {
                shipQuotas = shipQuotas.Where(x => x.Cfr.ToLower().Contains(filters.CFR.ToLower()));
            }

            if (!string.IsNullOrEmpty(filters.Association))
            {
                shipQuotas = shipQuotas.Where(x => x.AssociationName.ToLower().Contains(filters.Association.ToLower()));
            }

            if (filters.Year.HasValue)
            {
                shipQuotas = shipQuotas.Where(x => x.Year == filters.Year);
            }

            if (filters.FishId.HasValue)
            {
                shipQuotas = shipQuotas.Where(x => x.FishId == filters.FishId);
            }

            IQueryable<ShipQuotaDTO> result = from shipQuota in shipQuotas
                                              join quota in Db.CatchQuotas on shipQuota.CatchQuotaId equals quota.Id
                                              join ship in Db.ShipsRegister on shipQuota.ShipId equals ship.Id
                                              join fish in Db.Nfishes on quota.FishId equals fish.Id
                                              join assoc in Db.NshipAssociations on ship.ShipAssociationId equals assoc.Id into assocMatchTable
                                              from assocMatch in assocMatchTable.DefaultIfEmpty()
                                              orderby quota.PeriodEnd descending
                                              select new ShipQuotaDTO
                                              {
                                                  Id = shipQuota.Id,
                                                  AssociationName = assocMatch == null ? null : assocMatch.Name,
                                                  ShipName = ship.Name,
                                                  ShipCFR = ship.Cfr,
                                                  ShipExtMarking = ship.ExternalMark,
                                                  Year = quota.PeriodEnd.Year,
                                                  Fish = fish.Name + " (" + fish.NameLatin + ")",
                                                  QuotaSize = shipQuota.ShipQuotaSize,
                                                  UnloadedByCurrentDateKg = (from shipQuo in Db.ShipCatchQuotas
                                                                             join quo in Db.CatchQuotas on shipQuo.CatchQuotaId equals quo.Id
                                                                             join logBook in Db.LogBooks on shipQuo.ShipId equals logBook.ShipId
                                                                             join shipLogBookPage in Db.ShipLogBookPages on logBook.Id equals shipLogBookPage.LogBookId
                                                                             join originDeclaration in Db.OriginDeclarations on shipLogBookPage.Id equals originDeclaration.LogBookPageId
                                                                             join declarationFish in Db.OriginDeclarationFish on originDeclaration.Id equals declarationFish.OriginDeclarationId
                                                                             where shipQuo.Id == shipQuota.Id
                                                                                    && shipLogBookPage.PageFillDate <= quo.PeriodEnd
                                                                                    && shipLogBookPage.PageFillDate > quo.PeriodStart
                                                                                    && quo.FishId == declarationFish.FishId
                                                                             select declarationFish.Quantity).Sum(),
                                                  Leftover = shipQuota.ShipQuotaSize - (from shipQuo in Db.ShipCatchQuotas
                                                                                        join quo in Db.CatchQuotas on shipQuo.CatchQuotaId equals quo.Id
                                                                                        join logBook in Db.LogBooks on shipQuo.ShipId equals logBook.ShipId
                                                                                        join shipLogBookPage in Db.ShipLogBookPages on logBook.Id equals shipLogBookPage.LogBookId
                                                                                        join originDeclaration in Db.OriginDeclarations on shipLogBookPage.Id equals originDeclaration.LogBookPageId
                                                                                        join declarationFish in Db.OriginDeclarationFish on originDeclaration.Id equals declarationFish.OriginDeclarationId
                                                                                        where shipQuo.Id == shipQuota.Id
                                                                                               && shipLogBookPage.PageFillDate <= quo.PeriodEnd
                                                                                               && shipLogBookPage.PageFillDate > quo.PeriodStart
                                                                                               && quo.FishId == declarationFish.FishId
                                                                                        select declarationFish.Quantity).Sum(),
                                                  ConfiscatedQuantity = (from shipQuo in Db.ShipCatchQuotas
                                                                         join quo in Db.CatchQuotas on shipQuo.CatchQuotaId equals quo.Id
                                                                         join shipInspection in Db.ShipInspections on shipQuo.ShipId equals shipInspection.InspectiedShipId
                                                                         join inspection in Db.InspectionsRegister on shipInspection.InspectionId equals inspection.Id
                                                                         join catchMeasure in Db.InspectionCatchMeasures on inspection.Id equals catchMeasure.InspectionId
                                                                         where shipQuo.Id == shipQuota.Id
                                                                                 && inspection.InspectionStart >= quo.PeriodStart
                                                                                 && inspection.InspectionEnd <= quo.PeriodEnd
                                                                                 && quo.FishId == catchMeasure.FishId
                                                                                 && catchMeasure.IsTaken.Value == true
                                                                                 && quo.IsActive
                                                                         select catchMeasure.CatchQuantity.HasValue ? catchMeasure.CatchQuantity.Value : 0).Sum(),
                                                  IsActive = shipQuota.IsActive,
                                              };

            return result;
        }

        private IQueryable<ShipQuotaDTO> GetAllByFreeText(string text, bool showInactive)
        {
            text = text.ToLowerInvariant();

            IQueryable<ShipQuotaDTO> result = from shipQuota in Db.ShipCatchQuotas
                                              join quota in Db.CatchQuotas on shipQuota.CatchQuotaId equals quota.Id
                                              join ship in Db.ShipsRegister on shipQuota.ShipId equals ship.Id
                                              join fish in Db.Nfishes on quota.FishId equals fish.Id
                                              join assoc in Db.NshipAssociations on ship.ShipAssociationId equals assoc.Id into assocMatchTable
                                              from assocMatch in assocMatchTable.DefaultIfEmpty()
                                              where shipQuota.IsActive == !showInactive
                                                    && ((assocMatch != null && assocMatch.Name.ToLower().Contains(text))
                                                          || fish.Name.ToLower().Contains(text)
                                                          || fish.NameLatin.ToLower().Contains(text)
                                                          || ship.Name.ToLower().Contains(text)
                                                          || ship.ExternalMark.ToLower().Contains(text)
                                                          || ship.Cfr.ToLower().Contains(text)
                                                          || quota.PeriodEnd.Year.ToString().ToLower().Contains(text)
                                                          || shipQuota.ShipQuotaSize.ToString().ToLower().Contains(text)
                                                       )
                                              orderby quota.PeriodEnd descending
                                              select new ShipQuotaDTO
                                              {
                                                  Id = shipQuota.Id,
                                                  AssociationName = assocMatch == null ? null : assocMatch.Name,
                                                  ShipName = ship.Name,
                                                  ShipCFR = ship.Cfr,
                                                  ShipExtMarking = ship.ExternalMark,
                                                  Year = quota.PeriodEnd.Year,
                                                  Fish = fish.Name + " (" + fish.NameLatin + ")",
                                                  QuotaSize = shipQuota.ShipQuotaSize,
                                                  UnloadedByCurrentDateKg = (from shipQuo in Db.ShipCatchQuotas
                                                                             join quo in Db.CatchQuotas on shipQuo.CatchQuotaId equals quo.Id
                                                                             join logBook in Db.LogBooks on shipQuo.ShipId equals logBook.ShipId
                                                                             join shipLogBookPage in Db.ShipLogBookPages on logBook.Id equals shipLogBookPage.LogBookId
                                                                             join originDeclaration in Db.OriginDeclarations on shipLogBookPage.Id equals originDeclaration.LogBookPageId
                                                                             join declarationFish in Db.OriginDeclarationFish on originDeclaration.Id equals declarationFish.OriginDeclarationId
                                                                             where shipQuo.Id == shipQuota.Id
                                                                                    && shipLogBookPage.PageFillDate <= quo.PeriodEnd
                                                                                    && shipLogBookPage.PageFillDate > quo.PeriodStart
                                                                                    && quo.FishId == declarationFish.FishId
                                                                             select declarationFish.Quantity).Sum(),
                                                  Leftover = shipQuota.ShipQuotaSize - (from shipQuo in Db.ShipCatchQuotas
                                                                                        join quo in Db.CatchQuotas on shipQuo.CatchQuotaId equals quo.Id
                                                                                        join logBook in Db.LogBooks on shipQuo.ShipId equals logBook.ShipId
                                                                                        join shipLogBookPage in Db.ShipLogBookPages on logBook.Id equals shipLogBookPage.LogBookId
                                                                                        join originDeclaration in Db.OriginDeclarations on shipLogBookPage.Id equals originDeclaration.LogBookPageId
                                                                                        join declarationFish in Db.OriginDeclarationFish on originDeclaration.Id equals declarationFish.OriginDeclarationId
                                                                                        where shipQuo.Id == shipQuota.Id
                                                                                               && shipLogBookPage.PageFillDate <= quo.PeriodEnd
                                                                                               && shipLogBookPage.PageFillDate > quo.PeriodStart
                                                                                               && quo.FishId == declarationFish.FishId
                                                                                        select declarationFish.Quantity).Sum(),
                                                  ConfiscatedQuantity = (from shipQuo in Db.ShipCatchQuotas
                                                                         join quo in Db.CatchQuotas on shipQuo.CatchQuotaId equals quo.Id
                                                                         join shipInspection in Db.ShipInspections on shipQuo.ShipId equals shipInspection.InspectiedShipId
                                                                         join inspection in Db.InspectionsRegister on shipInspection.InspectionId equals inspection.Id
                                                                         join catchMeasure in Db.InspectionCatchMeasures on inspection.Id equals catchMeasure.InspectionId
                                                                         where shipQuo.Id == shipQuota.Id
                                                                                 && inspection.InspectionStart >= quo.PeriodStart
                                                                                 && inspection.InspectionEnd <= quo.PeriodEnd
                                                                                 && quo.FishId == catchMeasure.FishId
                                                                                 && catchMeasure.IsTaken.Value == true
                                                                                 && quo.IsActive
                                                                         select catchMeasure.CatchQuantity.HasValue ? catchMeasure.CatchQuantity.Value : 0).Sum(),
                                                  IsActive = shipQuota.IsActive,
                                              };

            return result;
        }

        private IQueryable<ShipQuotaDTO> GetAll(bool inactiveOnly = false)
        {
            IQueryable<ShipQuotaDTO> result = from shipQuota in Db.ShipCatchQuotas
                                              join quota in Db.CatchQuotas on shipQuota.CatchQuotaId equals quota.Id
                                              join ship in Db.ShipsRegister on shipQuota.ShipId equals ship.Id
                                              join fish in Db.Nfishes on quota.FishId equals fish.Id
                                              join assoc in Db.NshipAssociations on ship.ShipAssociationId equals assoc.Id into assocMatchTable
                                              from assocMatch in assocMatchTable.DefaultIfEmpty()
                                              where shipQuota.IsActive == !inactiveOnly
                                              orderby quota.PeriodEnd descending
                                              select new ShipQuotaDTO
                                              {
                                                  Id = shipQuota.Id,
                                                  AssociationName = assocMatch == null ? null : assocMatch.Name,
                                                  ShipName = ship.Name,
                                                  ShipCFR = ship.Cfr,
                                                  ShipExtMarking = ship.ExternalMark,
                                                  Year = quota.PeriodEnd.Year,
                                                  Fish = fish.Name + " (" + fish.NameLatin + ")",
                                                  QuotaSize = shipQuota.ShipQuotaSize,
                                                  UnloadedByCurrentDateKg = (from shipQuo in Db.ShipCatchQuotas
                                                                             join quo in Db.CatchQuotas on shipQuo.CatchQuotaId equals quo.Id
                                                                             join logBook in Db.LogBooks on shipQuo.ShipId equals logBook.ShipId
                                                                             join shipLogBookPage in Db.ShipLogBookPages on logBook.Id equals shipLogBookPage.LogBookId
                                                                             join originDeclaration in Db.OriginDeclarations on shipLogBookPage.Id equals originDeclaration.LogBookPageId
                                                                             join declarationFish in Db.OriginDeclarationFish on originDeclaration.Id equals declarationFish.OriginDeclarationId
                                                                             where shipQuo.Id == shipQuota.Id
                                                                                    && shipLogBookPage.PageFillDate <= quo.PeriodEnd
                                                                                    && shipLogBookPage.PageFillDate > quo.PeriodStart
                                                                                    && quo.FishId == declarationFish.FishId
                                                                             select declarationFish.Quantity).Sum(),
                                                  Leftover = shipQuota.ShipQuotaSize - (from shipQuo in Db.ShipCatchQuotas
                                                                                        join quo in Db.CatchQuotas on shipQuo.CatchQuotaId equals quo.Id
                                                                                        join logBook in Db.LogBooks on shipQuo.ShipId equals logBook.ShipId
                                                                                        join shipLogBookPage in Db.ShipLogBookPages on logBook.Id equals shipLogBookPage.LogBookId
                                                                                        join originDeclaration in Db.OriginDeclarations on shipLogBookPage.Id equals originDeclaration.LogBookPageId
                                                                                        join declarationFish in Db.OriginDeclarationFish on originDeclaration.Id equals declarationFish.OriginDeclarationId
                                                                                        where shipQuo.Id == shipQuota.Id
                                                                                               && shipLogBookPage.PageFillDate <= quo.PeriodEnd
                                                                                               && shipLogBookPage.PageFillDate > quo.PeriodStart
                                                                                               && quo.FishId == declarationFish.FishId
                                                                                        select declarationFish.Quantity).Sum(),
                                                  ConfiscatedQuantity = (from shipQuo in Db.ShipCatchQuotas
                                                                         join quo in Db.CatchQuotas on shipQuo.CatchQuotaId equals quo.Id
                                                                         join shipInspection in Db.ShipInspections on shipQuo.ShipId equals shipInspection.InspectiedShipId
                                                                         join inspection in Db.InspectionsRegister on shipInspection.InspectionId equals inspection.Id
                                                                         join catchMeasure in Db.InspectionCatchMeasures on inspection.Id equals catchMeasure.InspectionId
                                                                         where shipQuo.Id == shipQuota.Id
                                                                                 && inspection.InspectionStart >= quo.PeriodStart
                                                                                 && inspection.InspectionEnd <= quo.PeriodEnd
                                                                                 && quo.FishId == catchMeasure.FishId
                                                                                 && catchMeasure.IsTaken.Value == true
                                                                                 && quo.IsActive
                                                                         select catchMeasure.CatchQuantity.HasValue ? catchMeasure.CatchQuantity.Value : 0).Sum(),
                                                  IsActive = shipQuota.IsActive,
                                              };

            return result;
        }

        private void AddHist(QuotaHistDTO newHistEntryDto, bool deferSave = false)
        {
            ShipCatchQuotasHist lastHistEntry = Db.ShipCatchQuotasHists.Where(x => x.ShipCatchQuotaId == newHistEntryDto.QuotaId).OrderBy(x => x.ValidTo).LastOrDefault();
            if (lastHistEntry != null)
            {
                lastHistEntry.ValidTo = newHistEntryDto.Timestamp;
            }

            ShipCatchQuotasHist newHistEntry = new ShipCatchQuotasHist()
            {
                ShipCatchQuotaId = newHistEntryDto.QuotaId,
                ShipQuotaSize = (int)newHistEntryDto.NewQuotaValueKg,
                ShipQuotaIncrement = (int)newHistEntryDto.AddRemoveQuota,
                IncrementReason = newHistEntryDto.Basis,
                ValidFrom = newHistEntryDto.Timestamp,
                ValidTo = DefaultConstants.MAX_VALID_DATE
            };

            Db.ShipCatchQuotasHists.Add(newHistEntry);

            if (!deferSave)
            {
                Db.SaveChanges();
            }
        }

        private decimal GetUnloadedByCurrentDate(int shipQuotaId)
        {
            decimal unloadedByCurrentDate = 0;

            DateTime now = DateTime.Now;
            DateTime yearStart = new DateTime(now.Year, 1, 1);

            List<decimal> quantities = (from quota in Db.CatchQuotas
                                        join shipQuota in Db.ShipCatchQuotas on quota.Id equals shipQuota.CatchQuotaId
                                        join logBook in Db.LogBooks on shipQuota.ShipId equals logBook.ShipId
                                        join shipLogBookPage in Db.ShipLogBookPages on logBook.Id equals shipLogBookPage.LogBookId
                                        join originDeclaration in Db.OriginDeclarations on shipLogBookPage.Id equals originDeclaration.LogBookPageId
                                        join declarationFish in Db.OriginDeclarationFish on originDeclaration.Id equals declarationFish.OriginDeclarationId
                                        where shipQuota.Id == shipQuotaId
                                                 && shipLogBookPage.PageFillDate <= now
                                                 && shipLogBookPage.PageFillDate > yearStart
                                               && quota.FishId == declarationFish.FishId
                                        select declarationFish.Quantity).ToList();

            if (quantities != null)
            {
                foreach (decimal quantity in quantities)
                {
                    unloadedByCurrentDate += quantity;
                }
            }

            return unloadedByCurrentDate;
        }
    }
}
