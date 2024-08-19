using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class InspectionStateInvalidException : ArgumentException
    {
        public InspectionStateInvalidException()
        {
        }

        public InspectionStateInvalidException(string message) : base(message)
        {
        }

        public InspectionStateInvalidException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public InspectionStateInvalidException(string message, string paramName) : base(message, paramName)
        {
        }

        public InspectionStateInvalidException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected InspectionStateInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
