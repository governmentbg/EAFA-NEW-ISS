using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class EmailExistsException : ArgumentException
    {
        public EmailExistsException()
        {
        }

        public EmailExistsException(string message) : base(message)
        {
        }

        public EmailExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public EmailExistsException(string message, string paramName) : base(message, paramName)
        {
        }

        public EmailExistsException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected EmailExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
