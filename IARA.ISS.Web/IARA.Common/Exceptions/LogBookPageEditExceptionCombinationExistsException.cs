using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class LogBookPageEditExceptionCombinationExistsException : ArgumentException
    {
        public LogBookPageEditExceptionCombinationExistsException()
        {
        }

        public LogBookPageEditExceptionCombinationExistsException(string message) : base(message)
        {
        }

        public LogBookPageEditExceptionCombinationExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public LogBookPageEditExceptionCombinationExistsException(string message, string paramName) : base(message, paramName)
        {
        }

        public LogBookPageEditExceptionCombinationExistsException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected LogBookPageEditExceptionCombinationExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
