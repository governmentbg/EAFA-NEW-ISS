using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class PageNumberNotInLogbookException : ArgumentException
    {
        public PageNumberNotInLogbookException()
        {
        }

        public PageNumberNotInLogbookException(string message) : base(message)
        {
        }

        public PageNumberNotInLogbookException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public PageNumberNotInLogbookException(string message, string paramName) : base(message, paramName)
        {
        }

        public PageNumberNotInLogbookException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected PageNumberNotInLogbookException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
