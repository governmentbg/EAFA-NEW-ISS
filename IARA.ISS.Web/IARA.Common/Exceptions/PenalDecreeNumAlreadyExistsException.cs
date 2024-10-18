using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class PenalDecreeNumAlreadyExistsException : ArgumentException
    {
        public PenalDecreeNumAlreadyExistsException()
        {
        }

        public PenalDecreeNumAlreadyExistsException(string message) : base(message)
        {
        }

        public PenalDecreeNumAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public PenalDecreeNumAlreadyExistsException(string message, string paramName) : base(message, paramName)
        {
        }

        public PenalDecreeNumAlreadyExistsException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected PenalDecreeNumAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
