using System.Runtime.Serialization;
using System;

namespace IARA.Common.Exceptions
{
    public class ReportCodeAlreadyExistsException : ArgumentException
    {
        public ReportCodeAlreadyExistsException()
        {
        }

        public ReportCodeAlreadyExistsException(string message) : base(message)
        {
        }

        public ReportCodeAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ReportCodeAlreadyExistsException(string message, string paramName) : base(message, paramName)
        {
        }

        public ReportCodeAlreadyExistsException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected ReportCodeAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
