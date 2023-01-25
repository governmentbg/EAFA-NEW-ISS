using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class CannotDeletePermitWithLicensesException : InvalidOperationException
    {
        public CannotDeletePermitWithLicensesException(int permitId)
        {
            this.PermitId = permitId;
        }

        public CannotDeletePermitWithLicensesException(string message) : base(message)
        {
        }

        public CannotDeletePermitWithLicensesException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CannotDeletePermitWithLicensesException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public int PermitId { get; private set; }
    }
}
