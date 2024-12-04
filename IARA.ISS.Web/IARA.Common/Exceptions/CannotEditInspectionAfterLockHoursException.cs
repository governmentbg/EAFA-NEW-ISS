using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class CannotEditInspectionAfterLockHoursException : InvalidOperationException
    {
        public CannotEditInspectionAfterLockHoursException()
        {
        }

        public CannotEditInspectionAfterLockHoursException(string message) : base(message)
        {
        }

        public CannotEditInspectionAfterLockHoursException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CannotEditInspectionAfterLockHoursException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
