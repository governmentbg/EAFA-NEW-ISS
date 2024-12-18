using System.Runtime.Serialization;
using System;

namespace IARA.Common.Exceptions
{
    public class CannotCancelDecreeWithPenalPointsException : ArgumentException
    {
        public CannotCancelDecreeWithPenalPointsException()
        {
        }

        public CannotCancelDecreeWithPenalPointsException(string message) : base(message)
        {
        }

        public CannotCancelDecreeWithPenalPointsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public CannotCancelDecreeWithPenalPointsException(string message, string paramName) : base(message, paramName)
        {
        }

        public CannotCancelDecreeWithPenalPointsException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected CannotCancelDecreeWithPenalPointsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
