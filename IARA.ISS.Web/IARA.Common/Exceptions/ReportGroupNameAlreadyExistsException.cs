using System.Runtime.Serialization;
using System;

namespace IARA.Common.Exceptions
{
    public class ReportGroupNameAlreadyExistsException : ArgumentException
    {
        public ReportGroupNameAlreadyExistsException()
        {
        }

        public ReportGroupNameAlreadyExistsException(string message) : base(message)
        {
        }

        public ReportGroupNameAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ReportGroupNameAlreadyExistsException(string message, string paramName) : base(message, paramName)
        {
        }

        public ReportGroupNameAlreadyExistsException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected ReportGroupNameAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
