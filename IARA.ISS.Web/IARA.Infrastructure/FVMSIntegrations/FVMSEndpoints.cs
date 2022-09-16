namespace IARA.Infrastructure.FVMSIntegrations
{
    public class FVMSEndpoints
    {
        public const string NISS_PERMIT = "api/PLGDomain/SubNISSPerm";
        public const string MULTIPLE_NISS_PERMITS = "api/PLGDomain/SubNISSPerms";

        public const string TELEMETRY_QUERY = "api/TelemDomain/TelemQuery";
        public const string MULTIPLE_TELEMETRY_QUERIES = "api/TelemDomain/TelemQueries";
        public const string ALL_VESSELS_TELEMETRY_QUERY = "api/TelemDomain/TelemAllFVQuery";

        public const string GEO_ZONES_REPORT = "api/GZDomain/SubGZData";
    }
}
