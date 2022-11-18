using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class ShipEventExistsOnSameDateException : ArgumentException
    {
        public ShipEventExistsOnSameDateException()
        {
        }

        public ShipEventExistsOnSameDateException(string message) : base(message)
        {
        }

        public ShipEventExistsOnSameDateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ShipEventExistsOnSameDateException(string message, string paramName) : base(message, paramName)
        {
        }

        public ShipEventExistsOnSameDateException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected ShipEventExistsOnSameDateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
