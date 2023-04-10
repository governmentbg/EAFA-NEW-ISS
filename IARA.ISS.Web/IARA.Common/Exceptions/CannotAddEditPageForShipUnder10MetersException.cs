using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class CannotAddEditPageForShipUnder10MetersException : InvalidOperationException
    {
        public CannotAddEditPageForShipUnder10MetersException()
        {
        }

        public CannotAddEditPageForShipUnder10MetersException(string message) : base(message)
        {
        }

        public CannotAddEditPageForShipUnder10MetersException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CannotAddEditPageForShipUnder10MetersException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
