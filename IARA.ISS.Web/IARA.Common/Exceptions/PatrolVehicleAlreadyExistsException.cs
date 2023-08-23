using System.Runtime.Serialization;
using System;

namespace IARA.Common.Exceptions
{
    public class PatrolVehicleAlreadyExistsException : ArgumentException
    {
        public PatrolVehicleAlreadyExistsException()
        {
        }

        public PatrolVehicleAlreadyExistsException(string message) : base(message)
        {
        }

        public PatrolVehicleAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public PatrolVehicleAlreadyExistsException(string message, string paramName) : base(message, paramName)
        {
        }

        public PatrolVehicleAlreadyExistsException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected PatrolVehicleAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
