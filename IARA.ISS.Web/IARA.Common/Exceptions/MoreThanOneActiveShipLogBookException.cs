using System.Runtime.Serialization;
using System;

namespace IARA.Common.Exceptions
{
    public class MoreThanOneActiveShipLogBookException : InvalidOperationException
    {
        public MoreThanOneActiveShipLogBookException()
        {
        }

        public MoreThanOneActiveShipLogBookException(string message) : base(message)
        {
        }

        public MoreThanOneActiveShipLogBookException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MoreThanOneActiveShipLogBookException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
