using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class InvalidSqlQueryException : ArgumentException
    {
        public InvalidSqlQueryException()
        {
        }

        public InvalidSqlQueryException(string message) : base(message)
        {
        }

        public InvalidSqlQueryException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public InvalidSqlQueryException(string message, string paramName) : base(message, paramName)
        {
        }

        public InvalidSqlQueryException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected InvalidSqlQueryException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
