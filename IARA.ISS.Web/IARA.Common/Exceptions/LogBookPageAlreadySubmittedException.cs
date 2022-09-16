using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class LogBookPageAlreadySubmittedException : ArgumentException
    {
        public LogBookPageAlreadySubmittedException()
        {
        }

        public LogBookPageAlreadySubmittedException(string message) : base(message)
        {
        }

        public LogBookPageAlreadySubmittedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public LogBookPageAlreadySubmittedException(string message, string paramName) : base(message, paramName)
        {
        }

        public LogBookPageAlreadySubmittedException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected LogBookPageAlreadySubmittedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
