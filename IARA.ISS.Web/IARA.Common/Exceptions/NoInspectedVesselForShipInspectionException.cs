using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class NoInspectedVesselForShipInspectionException : ArgumentException
    {
        public NoInspectedVesselForShipInspectionException()
        {
        }

        public NoInspectedVesselForShipInspectionException(string message) : base(message)
        {
        }

        public NoInspectedVesselForShipInspectionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public NoInspectedVesselForShipInspectionException(string message, string paramName) : base(message, paramName)
        {
        }

        public NoInspectedVesselForShipInspectionException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected NoInspectedVesselForShipInspectionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
