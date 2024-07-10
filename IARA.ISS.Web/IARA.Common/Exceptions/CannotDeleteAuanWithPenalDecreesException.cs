using System;

namespace IARA.Common.Exceptions
{
    public class CannotDeleteAuanWithPenalDecreesException : InvalidOperationException
    {
        public CannotDeleteAuanWithPenalDecreesException(int auanId)
        {
            this.AuanId = auanId;
        }

        public CannotDeleteAuanWithPenalDecreesException(string message) : base(message)
        {
        }

        public CannotDeleteAuanWithPenalDecreesException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public int AuanId { get; private set; }
    }
}
