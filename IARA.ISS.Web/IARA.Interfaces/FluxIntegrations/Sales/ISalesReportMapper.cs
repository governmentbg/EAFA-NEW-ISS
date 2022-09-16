using System;
using System.Collections.Generic;
using IARA.Flux.Models;
using IARA.FluxModels.Enums;

namespace IARA.Interfaces.Flux
{
    public interface ISalesReportMapper
    {
        FLUXSalesReportMessageType MapFirstSalePageToSalesReport(List<(int id, Guid referenceId)> pages, ReportPurposeCodes purpose);

        FLUXSalesReportMessageType MapAdmissionPageToSalesReport(List<(int id, Guid referenceId)> pages, ReportPurposeCodes purpose);

        FLUXSalesReportMessageType MapTransportPageToSalesReport(List<(int id, Guid referenceId)> pages, ReportPurposeCodes purpose);

        FLUXSalesReportMessageType MapFirstSalePageToSalesReport(int id, ReportPurposeCodes purpose, Guid referenceId);

        FLUXSalesReportMessageType MapAdmissionPageToSalesReport(int id, ReportPurposeCodes purpose, Guid referenceId);

        FLUXSalesReportMessageType MapTransportPageToSalesReport(int id, ReportPurposeCodes purpose, Guid referenceId);
    }
}
