using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class CannotDeleteLicenseWithLogBooksException : InvalidOperationException
    {
        public CannotDeleteLicenseWithLogBooksException(int permitLicenseId)
        {
            this.PermitLicenseId = permitLicenseId;
        }

        public CannotDeleteLicenseWithLogBooksException(string message) : base(message)
        {
        }

        public CannotDeleteLicenseWithLogBooksException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CannotDeleteLicenseWithLogBooksException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public int PermitLicenseId { get; private set; }
    }
}
