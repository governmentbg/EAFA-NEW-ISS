namespace IARA.Infrastructure.FluxIntegration
{
    internal static class FluxEndpoints
    {
        public const string FLUX_VESSEL_REPORT_DOCUMENT = "/api/VesselDomain/SubRepDoc";
        public const string FLUX_VESSEL_QUERY = "/api/VesselDomain/QueryDoc";

        public const string FLUX_SALES_REPORT_MESSAGE = "/api/SalesDomain/SubSalesDoc";
        public const string FLUX_SALES_QUERY = "/api/SalesDomain/SalesQueryDoc";
        public const string FLUX_SALES_RESPONSE = "/api/SalesDomain/SubSalesRespDoc";

        public const string FLUX_FA_REPORT = "/api/FADomain/SubFADoc";
        public const string FLUX_FA_QUERY_ACK = "/api/FADomain/SubFARespDoc";

        public const string FLUX_FLAP_REQUEST = "/api/FLAPDomain/FLAPRequestDoc";

        public const string FLUX_ACDR_REPORT_MESSAGE = "/api/ACDRDomain/SubACDRRepDoc";
    }
}
