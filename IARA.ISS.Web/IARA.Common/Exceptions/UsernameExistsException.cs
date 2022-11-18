using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class UsernameExistsException : ArgumentException
    {
        public UsernameExistsException()
        {
        }
        public UsernameExistsException(string message) : base(message)
        {
        }

        public UsernameExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public UsernameExistsException(string message, string paramName) : base(message, paramName)
        {
        }

        public UsernameExistsException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected UsernameExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
