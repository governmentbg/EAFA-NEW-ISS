using System.Runtime.Serialization;
using System;

namespace IARA.Common.Exceptions
{
    public class InspectionTypeInvalidException : ArgumentException
    {
        public InspectionTypeInvalidException()
        {
        }

        public InspectionTypeInvalidException(string message) : base(message)
        {
        }

        public InspectionTypeInvalidException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public InspectionTypeInvalidException(string message, string paramName) : base(message, paramName)
        {
        }

        public InspectionTypeInvalidException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected InspectionTypeInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
