using System.Runtime.Serialization;
using System;

namespace IARA.Common.Exceptions
{
    public class InspectorInChargeNotRegisteredException : ArgumentException
    {
        public InspectorInChargeNotRegisteredException()
        {
        }

        public InspectorInChargeNotRegisteredException(string message) : base(message)
        {
        }

        public InspectorInChargeNotRegisteredException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public InspectorInChargeNotRegisteredException(string message, string paramName) : base(message, paramName)
        {
        }

        public InspectorInChargeNotRegisteredException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected InspectorInChargeNotRegisteredException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
