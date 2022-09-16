namespace IARA.Infrastructure.Services.Reports
{
    internal static class SQLReportUtils
    {
        public static bool ValidateDatasourceQuery(string datasourceQuery)
        {
            if (string.IsNullOrEmpty(datasourceQuery))
            {
                return false;
            }

            bool isValid = true;

            string query = datasourceQuery.Trim().ToLower();

            if (query.Substring(0, 6) != "select")
            {
                isValid = false;
            }

            if (query.Contains("delete ") || query.Contains(" delete"))
            {
                isValid = false;
            }

            if (query.Contains("exec ") || query.Contains(" exec"))
            {
                isValid = false;
            }

            if (query.Contains("insert ") || query.Contains(" insert"))
            {
                isValid = false;
            }

            if (query.Contains("update ") || query.Contains(" update"))
            {
                isValid = false;
            }

            if (query.Contains("alter ") || query.Contains(" alter"))
            {
                isValid = false;
            }

            if (query.Contains("create ") || query.Contains(" create"))
            {
                isValid = false;
            }

            if (query.Contains("drop ") || query.Contains(" drop"))
            {
                isValid = false;
            }

            if (query.Contains("truncate table ") || query.Contains(" truncate table"))
            {
                isValid = false;
            }

            if (query.Contains("into ") || query.Contains(" into"))
            {
                isValid = false;
            }

            return isValid;
        }
    }
}
