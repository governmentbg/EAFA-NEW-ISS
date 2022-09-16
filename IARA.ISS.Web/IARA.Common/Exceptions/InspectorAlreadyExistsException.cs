using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class InspectorAlreadyExistsException : ArgumentException
    {
        public InspectorAlreadyExistsException()
        {
        }

        public InspectorAlreadyExistsException(string message) : base(message)
        {
        }

        public InspectorAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public InspectorAlreadyExistsException(string message, string paramName) : base(message, paramName)
        {
        }

        public InspectorAlreadyExistsException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected InspectorAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
