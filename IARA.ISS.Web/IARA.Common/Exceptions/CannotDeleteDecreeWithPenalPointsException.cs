using System;

namespace IARA.Common.Exceptions
{
    public class CannotDeleteDecreeWithPenalPointsException : InvalidOperationException
    {
        public CannotDeleteDecreeWithPenalPointsException(int decreeId)
        {
            this.PenalDecreeId = decreeId;
        }

        public CannotDeleteDecreeWithPenalPointsException(string message) : base(message)
        {
        }

        public CannotDeleteDecreeWithPenalPointsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public int PenalDecreeId { get; private set; }
    }
}
