using System;

namespace IARA.Common.Exceptions
{
    public class LogBookNotFoundException : RecordNotFoundException
    {
        public LogBookNotFoundException(string message) : base(message)
        {
        }

        public LogBookNotFoundException(string message, string paramName) : base(message, paramName)
        {
        }

        public LogBookNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public LogBookNotFoundException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }
    }
}
