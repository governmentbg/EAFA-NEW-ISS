using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class CannotAddEditAdmissionPageException : InvalidOperationException
    {
        public CannotAddEditAdmissionPageException()
        {
        }

        public CannotAddEditAdmissionPageException(string message) : base(message)
        {
        }

        public CannotAddEditAdmissionPageException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CannotAddEditAdmissionPageException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
