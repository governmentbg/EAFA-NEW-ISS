using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class InvalidLogBookLicensePagesRangeException : ArgumentException
    {
        public InvalidLogBookLicensePagesRangeException()
        {
        }

        public InvalidLogBookLicensePagesRangeException(string message) : base(message)
        {
        }

        public InvalidLogBookLicensePagesRangeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public InvalidLogBookLicensePagesRangeException(string message, string paramName) : base(message, paramName)
        {
        }

        public InvalidLogBookLicensePagesRangeException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected InvalidLogBookLicensePagesRangeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
