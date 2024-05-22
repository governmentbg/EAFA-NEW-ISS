using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class MissingPageOldNumberWithoutPermissionException : ArgumentException
    {
        public MissingPageOldNumberWithoutPermissionException()
        {
        }

        public MissingPageOldNumberWithoutPermissionException(string message) : base(message)
        {
        }

        public MissingPageOldNumberWithoutPermissionException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public MissingPageOldNumberWithoutPermissionException(string message, string paramName) : base(message, paramName)
        {
        }

        public MissingPageOldNumberWithoutPermissionException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected MissingPageOldNumberWithoutPermissionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
