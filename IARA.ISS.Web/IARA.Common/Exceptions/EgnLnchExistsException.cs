using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class EgnLnchExistsException : ArgumentException
    {
        public EgnLnchExistsException()
        {
        }

        public EgnLnchExistsException(string message) : base(message)
        {
        }

        public EgnLnchExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public EgnLnchExistsException(string message, string paramName) : base(message, paramName)
        {
        }

        public EgnLnchExistsException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected EgnLnchExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
