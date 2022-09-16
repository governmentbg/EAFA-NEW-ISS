using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class InvalidPasswordException : ArgumentException
    {
        public InvalidPasswordException()
        {
        }

        public InvalidPasswordException(string message)
            : base(message)
        {
        }

        public InvalidPasswordException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public InvalidPasswordException(string message, string paramName)
            : base(message, paramName)
        {
        }

        public InvalidPasswordException(string message, string paramName, Exception innerException)
            : base(message, paramName, innerException)
        {
        }

        protected InvalidPasswordException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
