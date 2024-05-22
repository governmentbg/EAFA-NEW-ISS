using System;

namespace IARA.Common.Exceptions
{
    public class ReportSqlException : Exception
    {
        public ReportSqlException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}
