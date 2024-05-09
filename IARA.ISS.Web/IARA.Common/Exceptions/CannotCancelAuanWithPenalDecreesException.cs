using System.Runtime.Serialization;
using System;

namespace IARA.Common.Exceptions
{
    public class CannotCancelAuanWithPenalDecreesException : ArgumentException
    {
        public CannotCancelAuanWithPenalDecreesException()
        {
        }

        public CannotCancelAuanWithPenalDecreesException(string message) : base(message)
        {
        }

        public CannotCancelAuanWithPenalDecreesException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public CannotCancelAuanWithPenalDecreesException(string message, string paramName) : base(message, paramName)
        {
        }

        public CannotCancelAuanWithPenalDecreesException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected CannotCancelAuanWithPenalDecreesException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
