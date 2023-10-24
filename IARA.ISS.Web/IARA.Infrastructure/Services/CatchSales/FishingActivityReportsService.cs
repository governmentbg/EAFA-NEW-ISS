using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.FishingActivityReports;
using IARA.DomainModels.Nomenclatures;
using IARA.DomainModels.RequestModels;
using IARA.FluxModels.Enums;
using IARA.Interfaces.CatchSales;

namespace IARA.Infrastructure.Services.CatchSales
{
    public class FishingActivityReportsService : Service, IFishingActivityReportsService
    {
        public FishingActivityReportsService(IARADbContext dbContext)
            : base(dbContext)
        { }

        public IQueryable<FishingActivityReportDTO> GetAllFishingActivityReports(FishingActivityReportsFilters filters)
        {
            IQueryable<FishingActivityReportDTO> result;

            if (filters == null || !filters.HasAnyFilters(false))
            {
                bool showInactive = filters?.ShowInactiveRecords ?? false;
                result = GetAllFishingActivityReports(showInactive);
            }
            else
            {
                result = filters.HasFreeTextSearch()
                    ? GetFreeTextFilteredFishingActivityReports(filters)
                    : GetParametersFilteredFishingActivityReports(filters);
            }

            return result;
        }

        public ILookup<string, FishingActivityReportItemDTO> GetAllFishingActivityReportItems(List<string> tripIdentifiers)
        {
            ILookup<string, FishingActivityReportItemDTO> items =
                (from report in Db.FvmsfishingActivityReports
                 join request in Db.Fluxfvmsrequests on report.ResponseUuid equals request.RequestUuid
                 join purpose in Db.MdrFluxGpPurposes on report.MdrFluxGpPurposeId equals purpose.Id
                 join reportType in Db.MdrFluxFaReportTypes on report.MdrFluxFaReportTypeId equals reportType.Id
                 join faType in Db.MdrFluxFaTypes on report.MdrFluxFaTypeId equals faType.Id
                 join faSubType1 in Db.MdrFluxFaTypes on report.MdrFluxFaSubType1Id equals faSubType1.Id into faSubTypeOne
                 from faSubType1 in faSubTypeOne.DefaultIfEmpty()
                 join faSubType2 in Db.MdrFluxFaTypes on report.MdrFluxFaSubType2Id equals faSubType2.Id into faSubTypeTwo
                 from faSubType2 in faSubTypeTwo.DefaultIfEmpty()
                 orderby report.OccurenceDateTime descending
                 select new
                 {
                     report.TripIdentifier,
                     Item = new FishingActivityReportItemDTO
                     {
                         Id = report.Id,
                         RequestId = request.Id,
                         Uuid = report.ResponseUuid.ToString(),
                         Purpose = purpose.Code + " - " + purpose.EnDescription,
                         ReportType = reportType.Code,
                         FaType = ConcatenateFaTypes(faType, faSubType1, faSubType2),
                         Date = report.OccurenceDateTime,
                         Status = report.Status,
                         ErrorMessage = report.ErrorMessage,
                         IsActive = report.IsActive
                     }
                 }).ToLookup(x => x.TripIdentifier, y => y.Item);

            foreach (IGrouping<string, FishingActivityReportItemDTO> trip in items)
            {
                foreach (FishingActivityReportItemDTO item in trip)
                {
                    switch (item.Status)
                    {
                        case nameof(FvmsFishingActivityReportStatus.Received):
                            item.Status = AppResources.receivedFaReport;
                            break;
                        case nameof(FvmsFishingActivityReportStatus.Processed):
                            item.Status = AppResources.processedFaReport;
                            break;
                        case nameof(FvmsFishingActivityReportStatus.Error):
                            item.Status = AppResources.errorFaReport;
                            break;
                    }
                }
            }

            return items;
        }

        public List<FishingActivityReportPageDTO> GetAllFishingActivityReportPages(List<int> reportIds)
        {
            List<FishingActivityReportPageDTO> pages =
                (from page in Db.FvmsfishingActivityReportLogBookPages
                 join shipPage in Db.ShipLogBookPages on page.ShipLogBookPageId equals shipPage.Id
                 join fgr in Db.FishingGearRegisters on shipPage.FishingGearRegisterId equals fgr.Id
                 join fg in Db.NfishingGears on fgr.FishingGearTypeId equals fg.Id
                 join fgt in Db.NfishingGearTypes on fg.GearTypeId equals fgt.Id
                 where reportIds.Contains(page.FishingActivityReportId)
                 group new { shipPage, page, fg, fgt } by shipPage.Id into grouped
                 orderby grouped.Min(x => x.shipPage.CreatedOn) descending
                 select new FishingActivityReportPageDTO
                 {
                     Id = grouped.Key,
                     ReportId = grouped.Min(x => x.page.FishingActivityReportId),
                     PageNumber = grouped.Min(x => x.shipPage.PageNum),
                     PageStatus = grouped.Min(x => x.shipPage.Status),
                     GearName = $"{grouped.Min(x => x.fg.Code)} - {grouped.Min(x => x.fg.Name)} ({grouped.Min(x => x.fgt.Name)})",
                     UnloadPort = (from od in Db.OriginDeclarations
                                   join odf in Db.OriginDeclarationFish on od.Id equals odf.OriginDeclarationId
                                   join up in Db.Nports on odf.UnloadPortId equals up.Id into uP
                                   from up in uP.DefaultIfEmpty()
                                   where od.LogBookPageId == grouped.Key
                                        && od.IsActive
                                        && odf.IsActive
                                   select up.Name).FirstOrDefault(),
                     UnloadedFish = string.Join(", ", from od in Db.OriginDeclarations
                                                      join odf in Db.OriginDeclarationFish on od.Id equals odf.OriginDeclarationId
                                                      join fish in Db.Nfishes on odf.FishId equals fish.Id
                                                      where od.LogBookPageId == grouped.Key
                                                            && od.IsActive
                                                            && odf.IsActive
                                                      select $"{odf.Quantity} kg {fish.Name}"),
                     IsActive = grouped.First().shipPage.IsActive
                 }).ToList();

            foreach (FishingActivityReportPageDTO page in pages)
            {
                switch (page.PageStatus)
                {
                    case nameof(LogBookPageStatusesEnum.Submitted):
                        page.PageStatus = AppResources.submittedPage;
                        break;
                    case nameof(LogBookPageStatusesEnum.InProgress):
                        page.PageStatus = AppResources.inProgressPage;
                        break;
                    case nameof(LogBookPageStatusesEnum.Canceled):
                        page.PageStatus = AppResources.canceledPage;
                        break;
                    case nameof(LogBookPageStatusesEnum.Missing):
                        page.PageStatus = AppResources.missingPage;
                        break;
                }
            }

            return pages;
        }

        public string GetFishingActivityReportJson(int id)
        {
            string json = (from report in Db.FvmsfishingActivityReports
                           where report.Id == id
                           select report.ResponseMessage).First();

            return json;
        }

        public override SimpleAuditDTO GetSimpleAudit(int id)
        {
            return GetSimpleEntityAuditValues(Db.FvmsfishingActivityReports, id);
        }

        private IQueryable<FishingActivityReportDTO> GetAllFishingActivityReports(bool showInactive)
        {
            IQueryable<FishingActivityReportDTO> query = from report in Db.FvmsfishingActivityReports
                                                         join ship in Db.ShipsRegister on report.VesselId equals ship.Id
                                                         group new { report, ship } by report.TripIdentifier into grouped
                                                         orderby grouped.Min(x => x.report.OccurenceDateTime) descending
                                                         select new FishingActivityReportDTO
                                                         {
                                                             TripIdentifier = grouped.Key,
                                                             ShipCfr = grouped.Min(x => x.ship.Cfr),
                                                             ShipName = grouped.Min(x => x.ship.Name),
                                                             StartTime = (from report2 in Db.FvmsfishingActivityReports
                                                                          join faType in Db.MdrFluxFaTypes on report2.MdrFluxFaTypeId equals faType.Id
                                                                          where report2.TripIdentifier == grouped.Key
                                                                              && faType.Code == nameof(FaTypes.DEPARTURE)
                                                                          orderby report2.OccurenceDateTime 
                                                                          select (DateTime?)report2.OccurenceDateTime).FirstOrDefault(),
                                                             EndTime = (from report2 in Db.FvmsfishingActivityReports
                                                                        join faType in Db.MdrFluxFaTypes on report2.MdrFluxFaTypeId equals faType.Id
                                                                        where report2.TripIdentifier == grouped.Key
                                                                            && faType.Code == nameof(FaTypes.ARRIVAL)
                                                                        orderby report2.OccurenceDateTime descending
                                                                        select (DateTime?)report2.OccurenceDateTime).FirstOrDefault(),
                                                             UnloadTime = (from report2 in Db.FvmsfishingActivityReports
                                                                           join faType in Db.MdrFluxFaTypes on report2.MdrFluxFaTypeId equals faType.Id
                                                                           where report2.TripIdentifier == grouped.Key
                                                                               && faType.Code == nameof(FaTypes.LANDING)
                                                                           orderby report2.OccurenceDateTime descending
                                                                           select (DateTime?)report2.OccurenceDateTime).FirstOrDefault()
                                                         };

            return query;
        }

        private IQueryable<FishingActivityReportDTO> GetFreeTextFilteredFishingActivityReports(FishingActivityReportsFilters filters)
        {
            string text = filters.FreeTextSearch.ToLower();

            IQueryable<FishingActivityReportDTO> query = from report in Db.FvmsfishingActivityReports
                                                         join ship in Db.ShipsRegister on report.VesselId equals ship.Id
                                                         group new { report, ship } by report.TripIdentifier into grouped
                                                         orderby grouped.Min(x => x.report.OccurenceDateTime) descending
                                                         select new FishingActivityReportDTO
                                                         {
                                                             TripIdentifier = grouped.Key,
                                                             ShipCfr = grouped.Min(x => x.ship.Cfr),
                                                             ShipName = grouped.Min(x => x.ship.Name),
                                                             StartTime = (from report2 in Db.FvmsfishingActivityReports
                                                                          join faType in Db.MdrFluxFaTypes on report2.MdrFluxFaTypeId equals faType.Id
                                                                          where report2.TripIdentifier == grouped.Key
                                                                              && faType.Code == nameof(FaTypes.DEPARTURE)
                                                                          orderby report2.OccurenceDateTime
                                                                          select (DateTime?)report2.OccurenceDateTime).FirstOrDefault(),
                                                             EndTime = (from report2 in Db.FvmsfishingActivityReports
                                                                        join faType in Db.MdrFluxFaTypes on report2.MdrFluxFaTypeId equals faType.Id
                                                                        where report2.TripIdentifier == grouped.Key
                                                                            && faType.Code == nameof(FaTypes.ARRIVAL)
                                                                        orderby report2.OccurenceDateTime descending
                                                                        select (DateTime?)report2.OccurenceDateTime).FirstOrDefault(),
                                                             UnloadTime = (from report2 in Db.FvmsfishingActivityReports
                                                                           join faType in Db.MdrFluxFaTypes on report2.MdrFluxFaTypeId equals faType.Id
                                                                           where report2.TripIdentifier == grouped.Key
                                                                               && faType.Code == nameof(FaTypes.LANDING)
                                                                           orderby report2.OccurenceDateTime descending
                                                                           select (DateTime?)report2.OccurenceDateTime).FirstOrDefault()
                                                         };

            query = from report in query
                    where report.TripIdentifier.ToLower().Contains(text)
                        || report.ShipCfr.ToLower().Contains(text)
                        || report.ShipName.ToLower().Contains(text)
                    select report;

            return query;
        }

        private IQueryable<FishingActivityReportDTO> GetParametersFilteredFishingActivityReports(FishingActivityReportsFilters filters)
        {
            IQueryable<FishingActivityReportDTO> query = from report in Db.FvmsfishingActivityReports
                                                         join ship in Db.ShipsRegister on report.VesselId equals ship.Id
                                                         group new { report, ship } by report.TripIdentifier into grouped
                                                         orderby grouped.Min(x => x.report.OccurenceDateTime) descending
                                                         select new FishingActivityReportDTO
                                                         {
                                                             TripIdentifier = grouped.Key,
                                                             ShipCfr = grouped.Min(x => x.ship.Cfr),
                                                             ShipName = grouped.Min(x => x.ship.Name),
                                                             StartTime = (from report2 in Db.FvmsfishingActivityReports
                                                                          join faType in Db.MdrFluxFaTypes on report2.MdrFluxFaTypeId equals faType.Id
                                                                          where report2.TripIdentifier == grouped.Key
                                                                              && faType.Code == nameof(FaTypes.DEPARTURE)
                                                                          orderby report2.OccurenceDateTime
                                                                          select (DateTime?)report2.OccurenceDateTime).FirstOrDefault(),
                                                             EndTime = (from report2 in Db.FvmsfishingActivityReports
                                                                        join faType in Db.MdrFluxFaTypes on report2.MdrFluxFaTypeId equals faType.Id
                                                                        where report2.TripIdentifier == grouped.Key
                                                                            && faType.Code == nameof(FaTypes.ARRIVAL)
                                                                        orderby report2.OccurenceDateTime descending
                                                                        select (DateTime?)report2.OccurenceDateTime).FirstOrDefault(),
                                                             UnloadTime = (from report2 in Db.FvmsfishingActivityReports
                                                                           join faType in Db.MdrFluxFaTypes on report2.MdrFluxFaTypeId equals faType.Id
                                                                           where report2.TripIdentifier == grouped.Key
                                                                               && faType.Code == nameof(FaTypes.LANDING)
                                                                           orderby report2.OccurenceDateTime descending
                                                                           select (DateTime?)report2.OccurenceDateTime).FirstOrDefault()
                                                         };

            if (!string.IsNullOrEmpty(filters.TripIdentifier))
            {
                query = query.Where(x => x.TripIdentifier.ToLower().Contains(filters.TripIdentifier.ToLower()));
            }

            if (filters.ShipId.HasValue)
            {
                string cfr = (from ship in Db.ShipsRegister
                              where ship.Id == filters.ShipId.Value
                              select ship.Cfr).First();

                query = query.Where(x => x.ShipCfr.ToLower().Contains(cfr.ToLower()));
            }

            if (filters.StartTime.HasValue)
            {
                query = query.Where(x => x.StartTime >= filters.StartTime.Value);
            }

            if (filters.EndTime.HasValue)
            {
                query = query.Where(x => x.StartTime <= filters.EndTime.Value);
            }

            return query;
        }

        private static string ConcatenateFaTypes(MdrFluxFaType faType, MdrFluxFaType faSubType1, MdrFluxFaType faSubType2)
        {
            string result = faType.Code;

            if (faSubType1 != null || faSubType2 != null)
            {
                result += " (";

                if (faSubType1 != null)
                {
                    result += faSubType1.Code;

                    if (faSubType2 != null)
                    {
                        result += ", " + faSubType2.Code;
                    }
                }
                else if (faSubType2 != null)
                {
                    result += faSubType2.Code;

                    if (faSubType1 != null)
                    {
                        result += ", " + faSubType1.Code;
                    }
                }

                result += ")";
            }

            return result;
        }
    }
}
