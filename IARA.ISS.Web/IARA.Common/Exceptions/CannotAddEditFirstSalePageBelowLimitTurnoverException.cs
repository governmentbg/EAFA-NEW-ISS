using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class CannotAddEditFirstSalePageBelowLimitTurnoverException : InvalidOperationException
    {
        public CannotAddEditFirstSalePageBelowLimitTurnoverException()
        {
        }

        public CannotAddEditFirstSalePageBelowLimitTurnoverException(string message) : base(message)
        {
        }

        public CannotAddEditFirstSalePageBelowLimitTurnoverException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CannotAddEditFirstSalePageBelowLimitTurnoverException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
