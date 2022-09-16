using System;

namespace IARA.Common.Exceptions
{
    public class RecordNotFoundException : ArgumentException
    {
        public RecordNotFoundException(string message)
            : base(message)
        {
        }

        public RecordNotFoundException(string message, string paramName)
            : base(message, paramName)
        {
        }

        public RecordNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public RecordNotFoundException(string message, string paramName, Exception innerException)
            : base(message, paramName, innerException)
        {
        }
    }
}
