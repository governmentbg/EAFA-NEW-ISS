using System.Runtime.Serialization;
using System;

namespace IARA.Common.Exceptions
{
    public class SendToFLUXInspectionDomainException : InvalidOperationException
    {
        public SendToFLUXInspectionDomainException()
        {
        }

        public SendToFLUXInspectionDomainException(string message) : base(message)
        {
        }

        public SendToFLUXInspectionDomainException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SendToFLUXInspectionDomainException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
