using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class FishingAssociationAlreadyExistsException : ArgumentException
    {
        public FishingAssociationAlreadyExistsException()
        {
        }

        public FishingAssociationAlreadyExistsException(string message) : base(message)
        {
        }

        public FishingAssociationAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public FishingAssociationAlreadyExistsException(string message, string paramName) : base(message, paramName)
        {
        }

        public FishingAssociationAlreadyExistsException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected FishingAssociationAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
