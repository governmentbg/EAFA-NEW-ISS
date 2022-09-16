using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class BuyerDoesNotExistsException : ArgumentException
    {
        public BuyerDoesNotExistsException()
        {
        }

        public BuyerDoesNotExistsException(string message) : base(message)
        {
        }

        public BuyerDoesNotExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public BuyerDoesNotExistsException(string message, string paramName) : base(message, paramName)
        {
        }

        public BuyerDoesNotExistsException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected BuyerDoesNotExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
