using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class InvalidPermitLicenseNumberException : ArgumentException
    {
        public InvalidPermitLicenseNumberException()
        {
        }

        public InvalidPermitLicenseNumberException(string message)
            : base(message)
        {
        }

        public InvalidPermitLicenseNumberException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public InvalidPermitLicenseNumberException(string message, string paramName)
            : base(message, paramName)
        {
        }

        public InvalidPermitLicenseNumberException(string message, string paramName, Exception innerException)
            : base(message, paramName, innerException)
        {
        }

        protected InvalidPermitLicenseNumberException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
