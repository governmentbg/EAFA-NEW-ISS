using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class PermitLicenseSuspensionValidToAlreadyExistsException : ArgumentException
    {
        public PermitLicenseSuspensionValidToAlreadyExistsException()
        {
        }

        public PermitLicenseSuspensionValidToAlreadyExistsException(string message) : base(message)
        {
        }

        public PermitLicenseSuspensionValidToAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public PermitLicenseSuspensionValidToAlreadyExistsException(string message, string paramName) : base(message, paramName)
        {
        }

        public PermitLicenseSuspensionValidToAlreadyExistsException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected PermitLicenseSuspensionValidToAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
