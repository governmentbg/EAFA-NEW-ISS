using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class InvalidPertmiNumberException : ArgumentException
    {
        public InvalidPertmiNumberException()
        {
        }

        public InvalidPertmiNumberException(string message) : base(message)
        {
        }

        public InvalidPertmiNumberException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public InvalidPertmiNumberException(string message, string paramName) : base(message, paramName)
        {
        }

        public InvalidPertmiNumberException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected InvalidPertmiNumberException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
