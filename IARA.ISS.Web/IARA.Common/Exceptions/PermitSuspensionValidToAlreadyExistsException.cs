using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class PermitSuspensionValidToAlreadyExistsException : ArgumentException
    {
        public PermitSuspensionValidToAlreadyExistsException()
        {
        }

        public PermitSuspensionValidToAlreadyExistsException(string message) : base(message)
        {
        }

        public PermitSuspensionValidToAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public PermitSuspensionValidToAlreadyExistsException(string message, string paramName) : base(message, paramName)
        {
        }

        public PermitSuspensionValidToAlreadyExistsException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected PermitSuspensionValidToAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
