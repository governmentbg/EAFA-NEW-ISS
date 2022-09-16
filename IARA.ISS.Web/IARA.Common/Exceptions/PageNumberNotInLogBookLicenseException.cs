using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class PageNumberNotInLogBookLicenseException : ArgumentException
    {
        public PageNumberNotInLogBookLicenseException()
        {
        }

        public PageNumberNotInLogBookLicenseException(string message) : base(message)
        {
        }

        public PageNumberNotInLogBookLicenseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public PageNumberNotInLogBookLicenseException(string message, string paramName) : base(message, paramName)
        {
        }

        public PageNumberNotInLogBookLicenseException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected PageNumberNotInLogBookLicenseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
