using System;

namespace IARA.Common.Exceptions
{
    public class CannotDeleteInspectionWithAuansException : InvalidOperationException
    {
        public CannotDeleteInspectionWithAuansException(int inspectionId)
        {
            this.InspectionId = inspectionId;
        }

        public CannotDeleteInspectionWithAuansException(string message) : base(message)
        {
        }

        public CannotDeleteInspectionWithAuansException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public int InspectionId { get; private set; }
    }
}
