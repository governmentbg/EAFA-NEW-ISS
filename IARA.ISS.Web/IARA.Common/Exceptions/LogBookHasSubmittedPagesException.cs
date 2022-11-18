using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class LogBookHasSubmittedPagesException : InvalidOperationException
    {
        public LogBookHasSubmittedPagesException()
        {
        }

        public LogBookHasSubmittedPagesException(string message) : base(message)
        {
        }

        public LogBookHasSubmittedPagesException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected LogBookHasSubmittedPagesException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
