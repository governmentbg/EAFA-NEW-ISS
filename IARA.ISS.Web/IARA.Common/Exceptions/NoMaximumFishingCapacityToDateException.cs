using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class NoMaximumFishingCapacityToDateException : ArgumentException
    {
        public NoMaximumFishingCapacityToDateException()
        {
        }

        public NoMaximumFishingCapacityToDateException(string message) : base(message)
        {
        }

        public NoMaximumFishingCapacityToDateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public NoMaximumFishingCapacityToDateException(string message, string paramName) : base(message, paramName)
        {
        }

        public NoMaximumFishingCapacityToDateException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected NoMaximumFishingCapacityToDateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
