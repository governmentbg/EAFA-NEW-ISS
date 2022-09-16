using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IARA.Common.Constants;
using IARA.DataAccess;
using IARA.DomainModels.DTOModels.CatchQuotas;
using IARA.DomainModels.DTOModels.Files;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.EntityModels.Entities;
using IARA.Excel.Tools.Interfaces;
using IARA.Excel.Tools.Models;
using IARA.Infrastructure.Services.Internal;
using IARA.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IARA.Infrastructure.Services
{
    public class YearlyQuotasService : Service, IYearlyQuotasService
    {
        private readonly IExcelExporterService excelExporterService;

        public YearlyQuotasService(IARADbContext db, IExcelExporterService excelExporterService)
                 : base(db)
        {
            this.excelExporterService = excelExporterService;
        }

        public IQueryable<YearlyQuotaDTO> GetAll(YearlyQuotasFilters filters)
        {
            IQueryable<YearlyQuotaDTO> result;

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
            SimpleAuditDTO audit = GetSimpleEntityAuditValues(Db.CatchQuotas, id);
            return audit;
        }


        public int Add(YearlyQuotaEditDTO quota)
        {
            CatchQuota dbCatchQuota = (from catchQuota in Db.CatchQuotas
                                       where quota.FishId == catchQuota.FishId
                                            && quota.Year == catchQuota.PeriodStart.Year
                                       select catchQuota).SingleOrDefault();

            if (dbCatchQuota != null)
            {
                throw new ArgumentException("Cannot add existing catch quota");
            }

            CatchQuota entry = new CatchQuota()
            {
                FishId = quota.FishId.Value,
                PeriodStart = new DateTime(quota.Year.Value, 1, 1),
                PeriodEnd = new DateTime(quota.Year.Value, 12, 31),
                QuotaSize = quota.QuotaValueKg.Value,
                IsActive = true
            };

            List<int> portIds = quota.UnloadPorts != null ? quota.UnloadPorts.Select(x => x.Value).ToList() : null;
            AddOrEditCatchQuotaUnloadPorts(entry, portIds);
            Db.CatchQuotas.Add(entry);

            if (quota.Files != null)
            {
                foreach (FileInfoDTO file in quota.Files)
                {
                    Db.AddOrEditFile(entry, entry.CatchQuotaFiles, file);
                }
            }

            Db.SaveChanges();
            return entry.Id;
        }

        public IQueryable<QuotaHistDTO> GetHistoryForIds(IEnumerable<int> ids)
        {
            IQueryable<QuotaHistDTO> result = from quotaHist in Db.CatchQuotasHists
                                              where //quotaHist.ValidFrom < DateTime.Now
                                                    //&& quotaHist.ValidTo > DateTime.Now
                                                    quotaHist.CatchQuotaId.HasValue
                                                    && ids.Contains(quotaHist.CatchQuotaId.Value)
                                              orderby quotaHist.ValidFrom descending
                                              select new QuotaHistDTO()
                                              {
                                                  Id = quotaHist.Id,
                                                  QuotaId = quotaHist.CatchQuotaId.Value,
                                                  AddRemoveQuota = quotaHist.QuotaIncrement.GetValueOrDefault(),
                                                  NewQuotaValueKg = quotaHist.QuotaSize,
                                                  Basis = quotaHist.UpdateReason,
                                                  Timestamp = quotaHist.ValidFrom
                                              };

            return result;
        }


        public YearlyQuotaEditDTO Get(int id)
        {
            YearlyQuotaEditDTO result = (from quota in Db.CatchQuotas
                                         join fish in Db.Nfishes on quota.FishId equals fish.Id
                                         where quota.Id == id
                                         select new YearlyQuotaEditDTO
                                         {
                                             Id = quota.Id,
                                             FishId = quota.FishId,
                                             Year = quota.PeriodEnd.Year,
                                             QuotaValueKg = quota.QuotaSize
                                         }).First();

            result.LeftoverValueKg = this.GetQuotaLeftover(id, result.QuotaValueKg.Value);
            result.UnloadPorts = this.GetCatchQuotaUnloadPorts(id);
            result.Files = Db.GetFiles(Db.CatchQuotaFiles, id);
            return result;
        }

        public YearlyQuotaEditDTO GetLastYearsQuota(int newQuotaId)
        {
            YearlyQuotaEditDTO newQuota = (from quota in Db.CatchQuotas
                                           join fish in Db.Nfishes on quota.FishId equals fish.Id
                                           where quota.Id == newQuotaId
                                           select new YearlyQuotaEditDTO
                                           {
                                               Id = quota.Id,
                                               FishId = quota.FishId,
                                               Year = quota.PeriodEnd.Year,
                                               QuotaValueKg = quota.QuotaSize
                                           }).First();

            newQuota.LeftoverValueKg = this.GetQuotaLeftover(newQuotaId, newQuota.QuotaValueKg.Value);

            YearlyQuotaEditDTO oldQuota = (from quota in Db.CatchQuotas
                                           join fish in Db.Nfishes on quota.FishId equals fish.Id
                                           where quota.FishId == newQuota.FishId
                                                   && quota.PeriodEnd.Year == newQuota.Year - 1
                                           select new YearlyQuotaEditDTO
                                           {
                                               Id = quota.Id,
                                               FishId = quota.FishId,
                                               Year = quota.PeriodEnd.Year,
                                               QuotaValueKg = quota.QuotaSize
                                           }).SingleOrDefault();
            if (oldQuota != null)
            {
                oldQuota.LeftoverValueKg = this.GetQuotaLeftover(oldQuota.Id, oldQuota.QuotaValueKg.Value);
            }

            return oldQuota;
        }

        public void Edit(YearlyQuotaEditDTO entry)
        {
            CatchQuota dbQuota = Db.CatchQuotas.AsSplitQuery().Include(x => x.CatchQuotaFiles).First(x => x.Id == entry.Id);

            dbQuota.PeriodStart = new DateTime(entry.Year.Value, 1, 1);
            dbQuota.PeriodEnd = new DateTime(entry.Year.Value, 12, 31);

            QuotaHistDTO newHistEntry = new QuotaHistDTO()
            {
                AddRemoveQuota = entry.QuotaValueKg.Value - dbQuota.QuotaSize,
                Basis = entry.ChangeBasis,
                NewQuotaValueKg = entry.QuotaValueKg.Value,
                Timestamp = DateTime.Now,
                QuotaId = dbQuota.Id,
                PeriodStart = dbQuota.PeriodStart,
                PeriodEnd = dbQuota.PeriodEnd
            };

            AddHist(newHistEntry, true);

            dbQuota.FishId = entry.FishId.Value;
            dbQuota.QuotaSize = entry.QuotaValueKg.Value;

            List<int> portIds = entry.UnloadPorts != null ? entry.UnloadPorts.Select(x => x.Value).ToList() : null;
            AddOrEditCatchQuotaUnloadPorts(dbQuota, portIds);

            if (entry.Files != null)
            {
                foreach (FileInfoDTO file in entry.Files)
                {
                    Db.AddOrEditFile(dbQuota, dbQuota.CatchQuotaFiles, file);
                }
            }

            Db.SaveChanges();
        }

        public void Transfer(int newQuotaId, int oldQuotaId, int transferValue, string basis)
        {
            CatchQuota newCatchQuota = (from quota in Db.CatchQuotas
                                        where quota.Id == newQuotaId
                                        select quota).First();

            CatchQuota oldCatchQuota = (from quota in Db.CatchQuotas
                                        where quota.Id == oldQuotaId
                                        select quota).First();

            oldCatchQuota.QuotaSize -= transferValue;
            newCatchQuota.QuotaSize += transferValue;

            QuotaHistDTO newQuotaHistEntry = new QuotaHistDTO()
            {
                AddRemoveQuota = transferValue,
                Basis = basis,
                NewQuotaValueKg = newCatchQuota.QuotaSize,
                Timestamp = DateTime.Now,
                QuotaId = newCatchQuota.Id,
                PeriodStart = newCatchQuota.PeriodStart,
                PeriodEnd = newCatchQuota.PeriodEnd
            };

            QuotaHistDTO oldQuotaHistEntry = new QuotaHistDTO()
            {
                AddRemoveQuota = transferValue,
                Basis = basis,
                NewQuotaValueKg = oldCatchQuota.QuotaSize,
                Timestamp = DateTime.Now,
                QuotaId = oldCatchQuota.Id,
                PeriodStart = oldCatchQuota.PeriodStart,
                PeriodEnd = oldCatchQuota.PeriodEnd
            };

            AddHist(newQuotaHistEntry, true);
            AddHist(oldQuotaHistEntry, true);

            Db.SaveChanges();
        }

        public List<NomenclatureDTO> GetYearlyQuotasForList()
        {
            List<NomenclatureDTO> result = (from quota in Db.CatchQuotas
                                            join fish in Db.Nfishes on quota.FishId equals fish.Id
                                            orderby quota.PeriodEnd descending
                                            select new NomenclatureDTO
                                            {
                                                Value = quota.Id,
                                                DisplayName = fish.Name + " (" + fish.NameLatin + ", " + fish.Code + ")" + " - " + quota.PeriodStart.Year,
                                                IsActive = quota.IsActive
                                            }).ToList();

            return result;
        }

        public void Delete(int id)
        {
            DeleteRecordWithId(Db.CatchQuotas, id);
            Db.SaveChanges();
        }

        public void Restore(int id)
        {
            UndoDeleteRecordWithId(Db.CatchQuotas, id);
            Db.SaveChanges();
        }

        public Stream DownloadYearlyQuotaExcel(ExcelExporterRequestModel<YearlyQuotasFilters> request)
        {
            ExcelExporterData<YearlyQuotaDTO> data = new ExcelExporterData<YearlyQuotaDTO>
            {
                PrimaryKey = nameof(YearlyQuotaDTO.Id),
                Query = GetAll(request.Filters),
                HeaderNames = request.HeaderNames
            };

            return excelExporterService.BuildExcelFile(request, data);
        }

        private IQueryable<YearlyQuotaDTO> GetAllByFilter(YearlyQuotasFilters filters)
        {
            var quotas = from quota in Db.CatchQuotas
                         join fish in Db.Nfishes on quota.FishId equals fish.Id
                         where quota.IsActive == !filters.ShowInactiveRecords
                         select new
                         {
                             quota.Id,
                             fish.Name,
                             FishId = fish.Id,
                             quota.QuotaSize,
                             quota.PeriodEnd.Year,
                             IsActive = quota.IsActive
                         };

            if (filters.Year.HasValue)
            {
                quotas = quotas.Where(x => x.Year == filters.Year);
            }

            if (filters.FishId.HasValue)
            {
                quotas = quotas.Where(x => x.FishId == filters.FishId);
            }

            IQueryable<YearlyQuotaDTO> result = from quota in quotas
                                                join fish in Db.Nfishes on quota.FishId equals fish.Id
                                                select new YearlyQuotaDTO
                                                {
                                                    Id = quota.Id,
                                                    Fish = fish.Name + " (" + fish.NameLatin + ", " + fish.Code + ")",
                                                    QuotaValueKg = quota.QuotaSize,
                                                    UnloadedQuantity = (from quo in Db.CatchQuotas
                                                                        join shipQuota in Db.ShipCatchQuotas on quo.Id equals shipQuota.CatchQuotaId
                                                                        join logBook in Db.LogBooks on shipQuota.ShipId equals logBook.ShipId
                                                                        join shipLogBookPage in Db.ShipLogBookPages on logBook.Id equals shipLogBookPage.LogBookId
                                                                        join originDeclaration in Db.OriginDeclarations on shipLogBookPage.Id equals originDeclaration.LogBookPageId
                                                                        join declarationFish in Db.OriginDeclarationFish on originDeclaration.Id equals declarationFish.OriginDeclarationId
                                                                        where quo.Id == quota.Id
                                                                               && shipLogBookPage.PageFillDate <= quo.PeriodEnd
                                                                               && shipLogBookPage.PageFillDate > quo.PeriodStart
                                                                               && quo.FishId == declarationFish.FishId
                                                                               && quo.IsActive
                                                                        select declarationFish).Sum(x => x.Quantity),
                                                    Leftover = quota.QuotaSize - (from quo in Db.CatchQuotas
                                                                                  join shipQuota in Db.ShipCatchQuotas on quo.Id equals shipQuota.CatchQuotaId
                                                                                  join logBook in Db.LogBooks on shipQuota.ShipId equals logBook.ShipId
                                                                                  join shipLogBookPage in Db.ShipLogBookPages on logBook.Id equals shipLogBookPage.LogBookId
                                                                                  join originDeclaration in Db.OriginDeclarations on shipLogBookPage.Id equals originDeclaration.LogBookPageId
                                                                                  join declarationFish in Db.OriginDeclarationFish on originDeclaration.Id equals declarationFish.OriginDeclarationId
                                                                                  where quo.Id == quota.Id
                                                                                         && shipLogBookPage.PageFillDate <= quo.PeriodEnd
                                                                                         && shipLogBookPage.PageFillDate > quo.PeriodStart
                                                                                         && quo.FishId == declarationFish.FishId
                                                                                         && quo.IsActive
                                                                                  select declarationFish).Sum(x => x.Quantity),
                                                    ConfiscatedQuantity = (from quo in Db.CatchQuotas
                                                                           join catchMeasure in Db.InspectionCatchMeasures on quo.FishId equals catchMeasure.FishId
                                                                           join inspection in Db.InspectionsRegister on catchMeasure.InspectionId equals inspection.Id
                                                                           where quo.Id == quota.Id
                                                                                   && inspection.InspectionStart >= quo.PeriodStart
                                                                                   && inspection.InspectionEnd <= quo.PeriodEnd
                                                                                   && catchMeasure.IsTaken.Value == true
                                                                                   && quo.IsActive
                                                                           select catchMeasure.CatchQuantity.HasValue ? catchMeasure.CatchQuantity.Value : 0).Sum(),
                                                    Year = quota.Year,
                                                    IsActive = quota.IsActive
                                                };

            return result;
        }

        private IQueryable<YearlyQuotaDTO> GetAllByFreeText(string text, bool showInactive)
        {
            text = text.ToLowerInvariant();

            IQueryable<YearlyQuotaDTO> quotas = from quota in Db.CatchQuotas
                                                join fish in Db.Nfishes on quota.FishId equals fish.Id
                                                where quota.IsActive == !showInactive
                                                      && (((!string.IsNullOrWhiteSpace(fish.Name)) && fish.Name.ToLower().Contains(text))
                                                          || ((!string.IsNullOrWhiteSpace(fish.NameLatin)) && fish.NameLatin.ToLower().Contains(text))
                                                          || ((!string.IsNullOrWhiteSpace(fish.Code)) && fish.Code.ToLower().Contains(text))
                                                          || quota.QuotaSize.ToString().Contains(text)
                                                          || quota.PeriodEnd.Year.ToString().Contains(text))
                                                orderby quota.PeriodEnd descending
                                                select new YearlyQuotaDTO
                                                {
                                                    Id = quota.Id,
                                                    Fish = fish.Name + " (" + fish.NameLatin + ", " + fish.Code + ")",
                                                    QuotaValueKg = quota.QuotaSize,
                                                    UnloadedQuantity = (from quo in Db.CatchQuotas
                                                                        join shipQuota in Db.ShipCatchQuotas on quo.Id equals shipQuota.CatchQuotaId
                                                                        join logBook in Db.LogBooks on shipQuota.ShipId equals logBook.ShipId
                                                                        join shipLogBookPage in Db.ShipLogBookPages on logBook.Id equals shipLogBookPage.LogBookId
                                                                        join originDeclaration in Db.OriginDeclarations on shipLogBookPage.Id equals originDeclaration.LogBookPageId
                                                                        join declarationFish in Db.OriginDeclarationFish on originDeclaration.Id equals declarationFish.OriginDeclarationId
                                                                        where quo.Id == quota.Id
                                                                               && shipLogBookPage.PageFillDate <= quo.PeriodEnd
                                                                               && shipLogBookPage.PageFillDate > quo.PeriodStart
                                                                               && quo.FishId == declarationFish.FishId
                                                                               && quo.IsActive
                                                                        select declarationFish).Sum(x => x.Quantity),
                                                    Leftover = quota.QuotaSize - (from quo in Db.CatchQuotas
                                                                                  join shipQuota in Db.ShipCatchQuotas on quo.Id equals shipQuota.CatchQuotaId
                                                                                  join logBook in Db.LogBooks on shipQuota.ShipId equals logBook.ShipId
                                                                                  join shipLogBookPage in Db.ShipLogBookPages on logBook.Id equals shipLogBookPage.LogBookId
                                                                                  join originDeclaration in Db.OriginDeclarations on shipLogBookPage.Id equals originDeclaration.LogBookPageId
                                                                                  join declarationFish in Db.OriginDeclarationFish on originDeclaration.Id equals declarationFish.OriginDeclarationId
                                                                                  where quo.Id == quota.Id
                                                                                         && shipLogBookPage.PageFillDate <= quo.PeriodEnd
                                                                                         && shipLogBookPage.PageFillDate > quo.PeriodStart
                                                                                         && quo.FishId == declarationFish.FishId
                                                                                         && quo.IsActive
                                                                                  select declarationFish).Sum(x => x.Quantity),
                                                    ConfiscatedQuantity = (from quo in Db.CatchQuotas
                                                                           join catchMeasure in Db.InspectionCatchMeasures on quo.FishId equals catchMeasure.FishId
                                                                           join inspection in Db.InspectionsRegister on catchMeasure.InspectionId equals inspection.Id
                                                                           where quo.Id == quota.Id
                                                                                   && inspection.InspectionStart >= quo.PeriodStart
                                                                                   && inspection.InspectionEnd <= quo.PeriodEnd
                                                                                   && catchMeasure.IsTaken.Value == true
                                                                                   && quo.IsActive
                                                                           select catchMeasure.CatchQuantity.HasValue ? catchMeasure.CatchQuantity.Value : 0).Sum(),
                                                    Year = quota.PeriodEnd.Year,
                                                    IsActive = quota.IsActive
                                                };

            return quotas;
        }

        private IQueryable<YearlyQuotaDTO> GetAll(bool inactiveOnly)
        {
            IQueryable<YearlyQuotaDTO> quotas = from quota in Db.CatchQuotas
                                                join fish in Db.Nfishes on quota.FishId equals fish.Id
                                                where quota.IsActive == !inactiveOnly
                                                orderby quota.PeriodEnd descending
                                                select new YearlyQuotaDTO
                                                {
                                                    Id = quota.Id,
                                                    Fish = fish.Name + " (" + fish.NameLatin + ", " + fish.Code + ")",
                                                    Year = quota.PeriodEnd.Year,
                                                    QuotaValueKg = quota.QuotaSize,
                                                    UnloadedQuantity = (from quo in Db.CatchQuotas
                                                                        join shipQuota in Db.ShipCatchQuotas on quo.Id equals shipQuota.CatchQuotaId
                                                                        join logBook in Db.LogBooks on shipQuota.ShipId equals logBook.ShipId
                                                                        join shipLogBookPage in Db.ShipLogBookPages on logBook.Id equals shipLogBookPage.LogBookId
                                                                        join originDeclaration in Db.OriginDeclarations on shipLogBookPage.Id equals originDeclaration.LogBookPageId
                                                                        join declarationFish in Db.OriginDeclarationFish on originDeclaration.Id equals declarationFish.OriginDeclarationId
                                                                        where quo.Id == quota.Id
                                                                               && shipLogBookPage.PageFillDate <= quo.PeriodEnd
                                                                               && shipLogBookPage.PageFillDate > quo.PeriodStart
                                                                               && quo.FishId == declarationFish.FishId
                                                                               && quo.IsActive
                                                                        select declarationFish).Sum(x => x.Quantity),
                                                    Leftover = quota.QuotaSize - (from quo in Db.CatchQuotas
                                                                                  join shipQuota in Db.ShipCatchQuotas on quo.Id equals shipQuota.CatchQuotaId
                                                                                  join logBook in Db.LogBooks on shipQuota.ShipId equals logBook.ShipId
                                                                                  join shipLogBookPage in Db.ShipLogBookPages on logBook.Id equals shipLogBookPage.LogBookId
                                                                                  join originDeclaration in Db.OriginDeclarations on shipLogBookPage.Id equals originDeclaration.LogBookPageId
                                                                                  join declarationFish in Db.OriginDeclarationFish on originDeclaration.Id equals declarationFish.OriginDeclarationId
                                                                                  where quo.Id == quota.Id
                                                                                         && shipLogBookPage.PageFillDate <= quo.PeriodEnd
                                                                                         && shipLogBookPage.PageFillDate > quo.PeriodStart
                                                                                         && quo.FishId == declarationFish.FishId
                                                                                         && quo.IsActive
                                                                                  select declarationFish).Sum(x => x.Quantity),
                                                    ConfiscatedQuantity = (from quo in Db.CatchQuotas
                                                                           join catchMeasure in Db.InspectionCatchMeasures on quo.FishId equals catchMeasure.FishId
                                                                           join inspection in Db.InspectionsRegister on catchMeasure.InspectionId equals inspection.Id
                                                                           where quo.Id == quota.Id
                                                                                   && inspection.InspectionStart >= quo.PeriodStart
                                                                                   && inspection.InspectionEnd <= quo.PeriodEnd
                                                                                   && catchMeasure.IsTaken.Value == true
                                                                                   && quo.IsActive
                                                                           select catchMeasure.CatchQuantity.HasValue ? catchMeasure.CatchQuantity.Value : 0).Sum(),
                                                    IsActive = quota.IsActive
                                                };

            return quotas;
        }

        private void AddHist(QuotaHistDTO newHistEntryDto, bool deferSave = false)
        {
            CatchQuotasHist lastHistEntry = Db.CatchQuotasHists.Where(x => x.CatchQuotaId == newHistEntryDto.QuotaId).OrderBy(x => x.ValidTo).LastOrDefault();
            if (lastHistEntry != null)
            {
                lastHistEntry.ValidTo = newHistEntryDto.Timestamp;
            }

            CatchQuotasHist newHistEntry = new CatchQuotasHist()
            {
                CatchQuotaId = newHistEntryDto.QuotaId,
                PeriodStart = newHistEntryDto.PeriodStart,
                PeriodEnd = newHistEntryDto.PeriodEnd,
                QuotaIncrement = (int)newHistEntryDto.AddRemoveQuota,
                QuotaSize = (int)newHistEntryDto.NewQuotaValueKg,
                UpdateReason = newHistEntryDto.Basis,
                ValidFrom = newHistEntryDto.Timestamp,
                ValidTo = DefaultConstants.MAX_VALID_DATE
            };

            Db.CatchQuotasHists.Add(newHistEntry);

            if (!deferSave)
            {
                Db.SaveChanges();
            }
        }

        private void AddOrEditCatchQuotaUnloadPorts(CatchQuota quota, List<int> portIds)
        {

            List<int> dbUnloadPortsIds = (from port in Db.CatchQuotaUnloadPorts
                                          where port.CatchQuotaId == quota.Id
                                          && port.IsActive == true
                                          select port.PortId).ToList();

            IEnumerable<int> portIdsToDelete = portIds != null ? dbUnloadPortsIds.Except(portIds) : dbUnloadPortsIds;
            IEnumerable<int> portIdsToAdd = portIds != null ? portIds.Except(dbUnloadPortsIds) : null;

            if (portIdsToAdd != null)
            {
                foreach (int portId in portIdsToAdd)
                {
                    CatchQuotaUnloadPort dbUnloadPort = (from port in Db.CatchQuotaUnloadPorts
                                                         where port.CatchQuotaId == quota.Id
                                                         && port.PortId == portId
                                                         select port).SingleOrDefault();
                    if (dbUnloadPort != null)
                    {
                        dbUnloadPort.IsActive = true;
                    }
                    else
                    {
                        CatchQuotaUnloadPort entry = new CatchQuotaUnloadPort
                        {
                            CatchQuota = quota,
                            PortId = portId,
                            IsActive = true
                        };

                        Db.CatchQuotaUnloadPorts.Add(entry);
                    }
                }
            }
            if (portIdsToDelete != null)
            {
                foreach (int portId in portIdsToDelete)
                {
                    CatchQuotaUnloadPort dbUnloadPort = (from port in Db.CatchQuotaUnloadPorts
                                                         where port.CatchQuotaId == quota.Id
                                                         && port.PortId == portId
                                                         select port).SingleOrDefault();
                    dbUnloadPort.IsActive = false;
                }
            }
        }

        private List<NomenclatureDTO> GetCatchQuotaUnloadPorts(int quotaId)
        {
            List<NomenclatureDTO> ports = (from unloadPort in Db.CatchQuotaUnloadPorts
                                           join port in Db.Nports on unloadPort.PortId equals port.Id
                                           where unloadPort.CatchQuotaId == quotaId
                                                 && unloadPort.IsActive
                                           select new NomenclatureDTO
                                           {
                                               DisplayName = port.Name,
                                               Value = port.Id
                                           }).ToList();

            return ports;
        }

        private decimal GetQuotaLeftover(int quotaId, int quotaSize)
        {
            decimal leftover = 0;

            List<decimal> quantities = (from quota in Db.CatchQuotas
                                        join shipQuota in Db.ShipCatchQuotas on quota.Id equals shipQuota.CatchQuotaId
                                        join logBook in Db.LogBooks on shipQuota.ShipId equals logBook.ShipId
                                        join shipLogBookPage in Db.ShipLogBookPages on logBook.Id equals shipLogBookPage.LogBookId
                                        join originDeclaration in Db.OriginDeclarations on shipLogBookPage.Id equals originDeclaration.LogBookPageId
                                        join declarationFish in Db.OriginDeclarationFish on originDeclaration.Id equals declarationFish.OriginDeclarationId
                                        where quota.Id == quotaId
                                               && shipLogBookPage.PageFillDate <= quota.PeriodEnd
                                               && shipLogBookPage.PageFillDate > quota.PeriodStart
                                               && quota.FishId == declarationFish.FishId
                                               && quota.IsActive
                                        select declarationFish.Quantity).ToList();

            if (quantities != null)
            {
                decimal quantitiesSum = quantities.Sum();
                leftover = quotaSize - quantitiesSum;
            }

            return leftover;
        }
    }
}
