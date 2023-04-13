using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class ActiveFishingTicketExistsException : ArgumentException
    {
        public ActiveFishingTicketExistsException()
        {
        }

        public ActiveFishingTicketExistsException(string message) : base(message)
        {
        }

        public ActiveFishingTicketExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ActiveFishingTicketExistsException(string message, string paramName) : base(message, paramName)
        {
        }

        public ActiveFishingTicketExistsException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected ActiveFishingTicketExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
