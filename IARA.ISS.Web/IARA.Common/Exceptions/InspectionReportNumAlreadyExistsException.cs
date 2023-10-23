using System.Runtime.Serialization;
using System;

namespace IARA.Common.Exceptions
{
    public class InspectionReportNumAlreadyExistsException : ArgumentException
    {
        public InspectionReportNumAlreadyExistsException()
        {
        }

        public InspectionReportNumAlreadyExistsException(string message) : base(message)
        {
        }

        public InspectionReportNumAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public InspectionReportNumAlreadyExistsException(string message, string paramName) : base(message, paramName)
        {
        }

        public InspectionReportNumAlreadyExistsException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected InspectionReportNumAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
