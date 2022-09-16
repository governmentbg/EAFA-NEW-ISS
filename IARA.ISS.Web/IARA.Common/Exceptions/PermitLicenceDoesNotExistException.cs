using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class PermitLicenceDoesNotExistException : ArgumentException
    {
        public PermitLicenceDoesNotExistException()
        {
        }

        public PermitLicenceDoesNotExistException(string message) : base(message)
        {
        }

        public PermitLicenceDoesNotExistException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public PermitLicenceDoesNotExistException(string message, string paramName) : base(message, paramName)
        {
        }

        public PermitLicenceDoesNotExistException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected PermitLicenceDoesNotExistException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
