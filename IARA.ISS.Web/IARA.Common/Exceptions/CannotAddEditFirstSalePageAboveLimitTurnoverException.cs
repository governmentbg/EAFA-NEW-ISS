using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class CannotAddEditFirstSalePageAboveLimitTurnoverException : InvalidOperationException
    {
        public CannotAddEditFirstSalePageAboveLimitTurnoverException()
        {
        }

        public CannotAddEditFirstSalePageAboveLimitTurnoverException(string message) : base(message)
        {
        }

        public CannotAddEditFirstSalePageAboveLimitTurnoverException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected CannotAddEditFirstSalePageAboveLimitTurnoverException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
