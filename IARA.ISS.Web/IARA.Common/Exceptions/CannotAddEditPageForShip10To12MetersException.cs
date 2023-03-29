using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class CannotAddEditPageForShip10To12MetersException : InvalidOperationException
    {
        public CannotAddEditPageForShip10To12MetersException()
        {
        }

        public CannotAddEditPageForShip10To12MetersException(string message) : base(message)
        {
        }

        public CannotAddEditPageForShip10To12MetersException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CannotAddEditPageForShip10To12MetersException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
