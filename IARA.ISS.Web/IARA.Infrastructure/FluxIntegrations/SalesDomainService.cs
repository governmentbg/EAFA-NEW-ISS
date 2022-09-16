using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IARA.Common.Constants;
using IARA.DataAccess;
using IARA.EntityModels.Entities;
using IARA.Flux.Models;
using IARA.FluxModels;
using IARA.FluxModels.Enums;
using IARA.Infrastructure.FluxIntegrations.Interfaces;
using IARA.Infrastructure.FluxIntegrations.Mappings.SalesDomain;
using IARA.Interfaces.Flux;

namespace IARA.Infrastructure.FluxIntegrations
{
    public class SalesDomainService : BaseService, ISalesDomainService
    {
        private readonly IFluxSalesDomainReceiverService salesDomainReceiverService;
        private readonly ISalesReportMapper salesReportMapper;

        public SalesDomainService(IARADbContext db,
                                  IFluxSalesDomainReceiverService salesDomainReceiverService,
                                  ISalesReportMapper salesReportMapper)
            : base(db)
        {
            this.salesDomainReceiverService = salesDomainReceiverService;
            this.salesReportMapper = salesReportMapper;
        }

        public Task<bool> ReportSalesDocument(FLUXSalesReportMessageType message)
        {
            return salesDomainReceiverService.ReportSalesDocument(message);
        }

        public FLUXSalesReportMessageType FindSalesDocuments(FLUXSalesQueryMessageType queryMessage)
        {
            SalesQueryType query = queryMessage.SalesQuery;
            SalesQueryParams paremeters = ParseSalesQueryParams(query);
            List<(int id, Guid referenceId)> pages = GetPageIdsByQueryParameters(paremeters);

            FLUXSalesReportMessageType result = paremeters.QueryType switch
            {
                FluxSalesTypes.SN => salesReportMapper.MapFirstSalePageToSalesReport(pages, ReportPurposeCodes.Original),
                FluxSalesTypes.TOD => salesReportMapper.MapAdmissionPageToSalesReport(pages, ReportPurposeCodes.Original),
                FluxSalesTypes.TRD => salesReportMapper.MapTransportPageToSalesReport(pages, ReportPurposeCodes.Original),
                _ => throw new NotImplementedException()
            };

            // референтният номер на заявката се вписва като референтен за доклада
            result.FLUXReportDocument.ReferencedID = IDType.CreateID(query.ID.schemeID, query.ID.Value);

            return result;
        }

        public bool MustSendSalesReport(int? shipId = null, decimal? length = null)
        {
            if (length.HasValue)
            {
                return length >= DefaultConstants.FLUX_VESSEL_MIN_LENGTH_M;
            }

            if (shipId.HasValue)
            {
                decimal shipLength = (from ship in Db.ShipsRegister
                                      where ship.Id == shipId.Value
                                      select ship.TotalLength).First();

                return shipLength >= DefaultConstants.FLUX_VESSEL_MIN_LENGTH_M;
            }

            return false;
        }

        private static SalesQueryParams ParseSalesQueryParams(SalesQueryType query)
        {
            SalesQueryParams result = new()
            {
                QueryType = query.TypeCode.Value
            };

            if (query.SpecifiedDelimitedPeriod != null)
            {
                result.StartDate = (DateTime)query.SpecifiedDelimitedPeriod.StartDateTime;
                result.EndDate = (DateTime)query.SpecifiedDelimitedPeriod.EndDateTime;
            }

            foreach (SalesQueryParameterType param in query.SimpleSalesQueryParameter)
            {
                switch (param.TypeCode.Value)
                {
                    case SalesQueryParamTypes.ROLE:
                        // не е приложимо
                        break;
                    case SalesQueryParamTypes.FLAG:
                        // не е приложимо
                        break;
                    case SalesQueryParamTypes.PLACE:
                        // не е приложимо - местоположенията ни са в свободен текст
                        break;
                    case SalesQueryParamTypes.VESSEL:
                        result.ShipCfr = param.ValueID.Value;
                        break;
                    case SalesQueryParamTypes.SALES_ID:
                        result.SalesId = param.ValueID.Value;
                        break;
                    case SalesQueryParamTypes.TRIP_ID:
                        result.TripId = param.ValueID.Value;
                        break;
                }
            }

            return result;
        }

        private List<(int id, Guid referenceId)> GetPageIdsByQueryParameters(SalesQueryParams parameters)
        {
            List<(int id, Guid referenceId)> result = parameters.QueryType switch
            {
                FluxSalesTypes.SN => GetFirstSalePageIdsByQueryParameters(parameters),
                FluxSalesTypes.TOD => GetAdmissionPageIdsByQueryParameters(parameters),
                FluxSalesTypes.TRD => GetTransportPageIdsByQueryParameters(parameters),
                _ => throw new NotImplementedException()
            };

            return result;
        }

        private List<(int id, Guid referenceId)> GetFirstSalePageIdsByQueryParameters(SalesQueryParams parameters)
        {
            IQueryable<FirstSaleLogBookPage> query = from page in Db.FirstSaleLogBookPages
                                                     select page;

            if (parameters.StartDate.HasValue)
            {
                query = query.Where(x => x.SaleDate >= parameters.StartDate.Value);
            }

            if (parameters.EndDate.HasValue)
            {
                query = query.Where(x => x.SaleDate <= parameters.EndDate.Value);
            }

            if (!string.IsNullOrEmpty(parameters.ShipCfr))
            {
                query = from page in query
                        join originDeclaration in Db.OriginDeclarations on page.OriginDeclarationId equals originDeclaration.Id
                        join shipPage in Db.ShipLogBookPages on originDeclaration.LogBookPageId equals shipPage.Id
                        join shipLogbook in Db.LogBooks on shipPage.LogBookId equals shipLogbook.Id
                        join ship in Db.ShipsRegister on shipLogbook.ShipId equals ship.Id
                        where ship.Cfr.Trim().ToLower() == parameters.ShipCfr.Trim().ToLower()
                        select page;
            }

            if (!string.IsNullOrEmpty(parameters.SalesId))
            {
                query = from page in query
                        where parameters.SalesId == $"{SalesReportMapper.BULGARIA_CODE}-{FluxSalesTypes.SN}-{page.SaleDate.Value.Year}-{page.PageNum}"
                        select page;
            }

            if (!string.IsNullOrEmpty(parameters.TripId))
            {
                List<int> pageIds = (from fa in Db.FvmsfishingActivityReportLogBookPages
                                     join shipPage in Db.ShipLogBookPages on fa.ShipLogBookPageId equals shipPage.Id
                                     join originDeclaration in Db.OriginDeclarations on shipPage.Id equals originDeclaration.LogBookPageId
                                     join firstSalePage in Db.FirstSaleLogBookPages on originDeclaration.Id equals firstSalePage.OriginDeclarationId
                                     where fa.TripIdentifier.Trim().ToLower() == parameters.TripId.Trim().ToLower()
                                     select firstSalePage.Id).ToList();

                query = from page in query
                        where pageIds.Contains(page.Id)
                        select page;
            }

            var data = query.Select(x => new { x.Id, x.FluxIdentifier }).ToList();

            List<(int id, Guid referenceId)> result = data.Select(x => (x.Id, x.FluxIdentifier)).ToList();
            return result;
        }

        private List<(int id, Guid referenceId)> GetAdmissionPageIdsByQueryParameters(SalesQueryParams parameters)
        {
            IQueryable<AdmissionLogBookPage> query = from page in Db.AdmissionLogBookPages
                                                     select page;

            if (parameters.StartDate.HasValue)
            {
                query = query.Where(x => x.HandoverDate >= parameters.StartDate.Value);
            }

            if (parameters.EndDate.HasValue)
            {
                query = query.Where(x => x.HandoverDate <= parameters.EndDate.Value);
            }

            if (!string.IsNullOrEmpty(parameters.ShipCfr))
            {
                query = from page in query
                        join originDeclaration in Db.OriginDeclarations on page.OriginDeclarationId equals originDeclaration.Id
                        join shipPage in Db.ShipLogBookPages on originDeclaration.LogBookPageId equals shipPage.Id
                        join shipLogbook in Db.LogBooks on shipPage.LogBookId equals shipLogbook.Id
                        join ship in Db.ShipsRegister on shipLogbook.ShipId equals ship.Id
                        where ship.Cfr.Trim().ToLower() == parameters.ShipCfr.Trim().ToLower()
                        select page;
            }

            if (!string.IsNullOrEmpty(parameters.SalesId))
            {
                query = from page in query
                        where parameters.SalesId == $"{SalesReportMapper.BULGARIA_CODE}-{FluxSalesTypes.SN}-{page.HandoverDate.Value.Year}-{page.PageNum}"
                        select page;
            }

            if (!string.IsNullOrEmpty(parameters.TripId))
            {
                List<int> pageIds = (from fa in Db.FvmsfishingActivityReportLogBookPages
                                     join shipPage in Db.ShipLogBookPages on fa.ShipLogBookPageId equals shipPage.Id
                                     join originDeclaration in Db.OriginDeclarations on shipPage.Id equals originDeclaration.LogBookPageId
                                     join admissionPage in Db.AdmissionLogBookPages on originDeclaration.Id equals admissionPage.OriginDeclarationId
                                     where fa.TripIdentifier.Trim().ToLower() == parameters.TripId.Trim().ToLower()
                                     select admissionPage.Id).ToList();

                query = from page in query
                        where pageIds.Contains(page.Id)
                        select page;
            }

            var data = query.Select(x => new { x.Id, x.FluxIdentifier }).ToList();

            List<(int id, Guid referenceId)> result = data.Select(x => (x.Id, x.FluxIdentifier)).ToList();
            return result;
        }

        private List<(int id, Guid referenceId)> GetTransportPageIdsByQueryParameters(SalesQueryParams parameters)
        {
            IQueryable<TransportationLogBookPage> query = from page in Db.TransportationLogBookPages
                                                          select page;

            if (parameters.StartDate.HasValue)
            {
                query = query.Where(x => x.LoadingDate >= parameters.StartDate.Value);
            }

            if (parameters.EndDate.HasValue)
            {
                query = query.Where(x => x.LoadingDate <= parameters.EndDate.Value);
            }

            if (!string.IsNullOrEmpty(parameters.ShipCfr))
            {
                query = from page in query
                        join originDeclaration in Db.OriginDeclarations on page.OriginDeclarationId equals originDeclaration.Id
                        join shipPage in Db.ShipLogBookPages on originDeclaration.LogBookPageId equals shipPage.Id
                        join shipLogbook in Db.LogBooks on shipPage.LogBookId equals shipLogbook.Id
                        join ship in Db.ShipsRegister on shipLogbook.ShipId equals ship.Id
                        where ship.Cfr.Trim().ToLower() == parameters.ShipCfr.Trim().ToLower()
                        select page;
            }

            if (!string.IsNullOrEmpty(parameters.SalesId))
            {
                query = from page in query
                        where parameters.SalesId == $"{SalesReportMapper.BULGARIA_CODE}-{FluxSalesTypes.SN}-{page.LoadingDate.Value.Year}-{page.PageNum}"
                        select page;
            }

            if (!string.IsNullOrEmpty(parameters.TripId))
            {
                List<int> pageIds = (from fa in Db.FvmsfishingActivityReportLogBookPages
                                     join shipPage in Db.ShipLogBookPages on fa.ShipLogBookPageId equals shipPage.Id
                                     join originDeclaration in Db.OriginDeclarations on shipPage.Id equals originDeclaration.LogBookPageId
                                     join transportPage in Db.TransportationLogBookPages on originDeclaration.Id equals transportPage.OriginDeclarationId
                                     where fa.TripIdentifier.Trim().ToLower() == parameters.TripId.Trim().ToLower()
                                     select transportPage.Id).ToList();

                query = from page in query
                        where pageIds.Contains(page.Id)
                        select page;
            }

            var data = query.Select(x => new { x.Id, x.FluxIdentifier }).ToList();

            List<(int id, Guid referenceId)> result = data.Select(x => (x.Id, x.FluxIdentifier)).ToList();
            return result;
        }
    }

    internal class SalesQueryParams
    {
        public string QueryType { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string ShipCfr { get; set; }

        public string SalesId { get; set; }

        public string TripId { get; set; }
    }
}
