using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class SendToFLUXSalesDomainException : InvalidOperationException
    {
        public SendToFLUXSalesDomainException()
        {
        }

        public SendToFLUXSalesDomainException(string message) : base(message)
        {
        }

        public SendToFLUXSalesDomainException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SendToFLUXSalesDomainException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
