using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class CannotAddEditAquaculturePageException : InvalidOperationException
    {
        public CannotAddEditAquaculturePageException()
        {
        }

        public CannotAddEditAquaculturePageException(string message) : base(message)
        {
        }

        public CannotAddEditAquaculturePageException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CannotAddEditAquaculturePageException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
