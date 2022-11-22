using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class EDeliverySendFailedException : InvalidOperationException
    {
        public EDeliverySendFailedException()
        {
        }

        public EDeliverySendFailedException(string message) : base(message)
        {
        }

        public EDeliverySendFailedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EDeliverySendFailedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
