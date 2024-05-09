using System.Runtime.Serialization;
using System;

namespace IARA.Common.Exceptions
{
    public class AuanNumAlreadyExistsException : ArgumentException
    {
        public AuanNumAlreadyExistsException()
        {
        }

        public AuanNumAlreadyExistsException(string message) : base(message)
        {
        }

        public AuanNumAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public AuanNumAlreadyExistsException(string message, string paramName) : base(message, paramName)
        {
        }

        public AuanNumAlreadyExistsException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected AuanNumAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
