using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace IARA.Common.Exceptions
{
    public class InvalidLogBookPagesRangeException : ArgumentException
    {
        public InvalidLogBookPagesRangeException()
        {
        }

        public InvalidLogBookPagesRangeException(string message) : base(message)
        {
        }

        public InvalidLogBookPagesRangeException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public InvalidLogBookPagesRangeException(string message, string paramName) : base(message, paramName)
        {
        }

        public InvalidLogBookPagesRangeException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected InvalidLogBookPagesRangeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
