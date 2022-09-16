using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class QualifiedFisherDoesNotExistException : ArgumentException
    {
        public QualifiedFisherDoesNotExistException()
        {
        }

        public QualifiedFisherDoesNotExistException(string message) : base(message)
        {
        }

        public QualifiedFisherDoesNotExistException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public QualifiedFisherDoesNotExistException(string message, string paramName) : base(message, paramName)
        {
        }

        public QualifiedFisherDoesNotExistException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected QualifiedFisherDoesNotExistException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
