using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class LogBookPageAlreadySubmittedOtherLogBookException : ArgumentException
    {
        public LogBookPageAlreadySubmittedOtherLogBookException()
        {
        }

        public LogBookPageAlreadySubmittedOtherLogBookException(string message) : base(message)
        {
        }

        public LogBookPageAlreadySubmittedOtherLogBookException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public LogBookPageAlreadySubmittedOtherLogBookException(string message, string paramName) : base(message, paramName)
        {
        }

        public LogBookPageAlreadySubmittedOtherLogBookException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected LogBookPageAlreadySubmittedOtherLogBookException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
