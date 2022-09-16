using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class NoPermitRegisterForPermitLicenseException : ArgumentException
    {
        public NoPermitRegisterForPermitLicenseException()
        {
        }

        public NoPermitRegisterForPermitLicenseException(string message) : base(message)
        {
        }

        public NoPermitRegisterForPermitLicenseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public NoPermitRegisterForPermitLicenseException(string message, string paramName) : base(message, paramName)
        {
        }

        public NoPermitRegisterForPermitLicenseException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected NoPermitRegisterForPermitLicenseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
