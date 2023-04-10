using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class CannotAddEditPageForShipOver12MetersException : InvalidOperationException
    {
        public CannotAddEditPageForShipOver12MetersException()
        {
        }

        public CannotAddEditPageForShipOver12MetersException(string message) : base(message)
        {
        }

        public CannotAddEditPageForShipOver12MetersException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CannotAddEditPageForShipOver12MetersException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
