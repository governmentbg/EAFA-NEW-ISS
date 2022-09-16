using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class PermitDoesNotExistException : ArgumentException
    {
        public PermitDoesNotExistException()
        {
        }

        public PermitDoesNotExistException(string message) : base(message)
        {
        }

        public PermitDoesNotExistException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public PermitDoesNotExistException(string message, string paramName) : base(message, paramName)
        {
        }

        public PermitDoesNotExistException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected PermitDoesNotExistException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
