using System;

namespace IARA.Common.Exceptions
{
    public class UserNotFoundException : RecordNotFoundException
    {
        public UserNotFoundException(string message) : base(message)
        {
        }

        public UserNotFoundException(string message, string paramName) : base(message, paramName)
        {
        }

        public UserNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public UserNotFoundException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }
    }
}
