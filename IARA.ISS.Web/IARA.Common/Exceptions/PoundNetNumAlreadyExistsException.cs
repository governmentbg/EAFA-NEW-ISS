using System.Runtime.Serialization;
using System;

namespace IARA.Common.Exceptions
{
    public class PoundNetNumAlreadyExistsException : ArgumentException
    {
        public PoundNetNumAlreadyExistsException()
        {
        }

        public PoundNetNumAlreadyExistsException(string message) : base(message)
        {
        }

        public PoundNetNumAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public PoundNetNumAlreadyExistsException(string message, string paramName) : base(message, paramName)
        {
        }

        public PoundNetNumAlreadyExistsException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected PoundNetNumAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
