using System;
using System.Linq;
using System.Collections.Generic;
using IARA.DataAccess;
using IARA.Flux.Models;
using IARA.FluxModels;
using IARA.FluxModels.Enums;
using IARA.Infrastructure.FluxIntegrations.Mappings.SalesDomain.Base;
using IARA.Interfaces.Flux;

namespace IARA.Infrastructure.FluxIntegrations.Mappings.SalesDomain
{
    public class SalesReportMapper : BaseService, ISalesReportMapper
    {
        public const string BULGARIA_CODE = "BGR";
        public const string BULGARIA_NAME_EN = "Bulgaria";
        public const string BULGARIA_CURRENCY_CODE = "BGN";

        public SalesReportMapper(IARADbContext db)
            : base(db)
        { }

        public FLUXSalesReportMessageType MapFirstSalePageToSalesReport(List<(int id, Guid referenceId)> pages, ReportPurposeCodes purpose)
        {
            FLUXSalesReportMessageType report = CreateSalesReportMessage(pages, purpose, FluxSalesTypes.SN);
            return report;
        }

        public FLUXSalesReportMessageType MapAdmissionPageToSalesReport(List<(int id, Guid referenceId)> pages, ReportPurposeCodes purpose)
        {
            FLUXSalesReportMessageType report = CreateSalesReportMessage(pages, purpose, FluxSalesTypes.TOD);
            return report;
        }

        public FLUXSalesReportMessageType MapTransportPageToSalesReport(List<(int id, Guid referenceId)> pages, ReportPurposeCodes purpose)
        {
            FLUXSalesReportMessageType report = CreateSalesReportMessage(pages, purpose, FluxSalesTypes.TRD);
            return report;
        }

        public FLUXSalesReportMessageType MapFirstSalePageToSalesReport(int id, ReportPurposeCodes purpose, Guid referenceId)
        {
            FLUXSalesReportMessageType report = MapFirstSalePageToSalesReport(new List<(int id, Guid referenceId)> { (id, referenceId) }, purpose);
            return report;
        }

        public FLUXSalesReportMessageType MapAdmissionPageToSalesReport(int id, ReportPurposeCodes purpose, Guid referenceId)
        {
            FLUXSalesReportMessageType report = MapAdmissionPageToSalesReport(new List<(int id, Guid referenceId)> { (id, referenceId) }, purpose);
            return report;
        }

        public FLUXSalesReportMessageType MapTransportPageToSalesReport(int id, ReportPurposeCodes purpose, Guid referenceId)
        {
            FLUXSalesReportMessageType report = MapTransportPageToSalesReport(new List<(int id, Guid referenceId)> { (id, referenceId) }, purpose);
            return report;
        }

        private FLUXSalesReportMessageType CreateSalesReportMessage(List<(int id, Guid referenceId)> pages, ReportPurposeCodes purpose, string salesType)
        {
            if (purpose != ReportPurposeCodes.Original && purpose != ReportPurposeCodes.Replace && purpose != ReportPurposeCodes.Delete)
            {
                throw new ArgumentException("Invalid purpose for Sales domain: " + purpose.ToString());
            }

            FLUXSalesReportMessageType report = new()
            {
                FLUXReportDocument = CreateFluxReportDocument(pages, purpose)
            };

            if (purpose != ReportPurposeCodes.Delete)
            {
                report.SalesReport = CreateSalesReport(pages, salesType);
            }

            return report;
        }

        private static FLUXReportDocumentType CreateFluxReportDocument(List<(int id, Guid referenceId)> pages, ReportPurposeCodes purpose)
        {
            FLUXReportDocumentType document = new()
            {
                CreationDateTime = DateTime.Now,
                PurposeCode = CodeType.CreatePurpose(purpose),
                OwnerFLUXParty = new FLUXPartyType
                {
                    ID = new IDType[] { IDType.CreateParty(BULGARIA_CODE) },
                    Name = new TextType[] { TextType.CreateText(BULGARIA_NAME_EN) }
                }
            };

            if (pages.Count > 1)
            {
                document.ID = new IDType[] { IDType.CreateFromGuid(Guid.NewGuid()) };
            }
            else
            {
                switch (purpose)
                {
                    case ReportPurposeCodes.Original:
                        document.ID = new IDType[] { IDType.CreateFromGuid(pages[0].referenceId) };
                        break;
                    case ReportPurposeCodes.Replace:
                        document.ID = new IDType[] { IDType.CreateFromGuid(Guid.NewGuid()) };
                        document.ReferencedID = IDType.CreateFromGuid(pages[0].referenceId);
                        break;
                    case ReportPurposeCodes.Delete:
                        document.ID = new IDType[] { IDType.CreateFromGuid(Guid.NewGuid()) };
                        document.ReferencedID = IDType.CreateFromGuid(pages[0].referenceId);
                        break;
                }
            }

            return document;
        }

        private SalesReportType[] CreateSalesReport(List<(int id, Guid referenceId)> pages, string salesType)
        {            
            SalesReportType report = new()
            {
                ItemTypeCode = CodeType.CreateCode(ListIDTypes.FLUX_SALES_TYPE, salesType),
                IncludedSalesDocument = pages
                    .Select(x => x.id)
                    .Select(pageId => BaseSalesPageData.GetPageData(pageId, Db, salesType))
                    .Select(page => page.CreateSalesDocument())
                    .ToArray()
            };

            return new SalesReportType[] { report };
        }
    }
}
