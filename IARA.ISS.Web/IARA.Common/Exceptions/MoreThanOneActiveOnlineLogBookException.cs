using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class MoreThanOneActiveOnlineLogBookException : InvalidOperationException
    {
        public MoreThanOneActiveOnlineLogBookException()
        {
        }

        public MoreThanOneActiveOnlineLogBookException(string message) : base(message)
        {
        }

        public MoreThanOneActiveOnlineLogBookException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected MoreThanOneActiveOnlineLogBookException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
