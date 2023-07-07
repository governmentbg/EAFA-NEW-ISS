using System;

namespace IARA.RegixAbstractions.Exceptions
{
    public class RegixOperationException : Exception
    {
        public RegixOperationException(string message)
            : base(message)
        { }

        public RegixOperationException(string message, Exception innerEx)
            : base(message, innerEx) { }
    }
}
