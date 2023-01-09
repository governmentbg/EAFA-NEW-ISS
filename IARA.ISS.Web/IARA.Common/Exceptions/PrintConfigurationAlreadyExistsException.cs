using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class PrintConfigurationAlreadyExistsException : ArgumentException
    {
        public PrintConfigurationAlreadyExistsException()
        {
        }

        public PrintConfigurationAlreadyExistsException(int? applicationTypeId, int? territoryUnitId)
        {
            ApplicationTypeId = applicationTypeId;
            TerritoryUnitId = territoryUnitId;
        }

        public PrintConfigurationAlreadyExistsException(string message) : base(message)
        {
        }

        public PrintConfigurationAlreadyExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public PrintConfigurationAlreadyExistsException(string message, string paramName) : base(message, paramName)
        {
        }

        public PrintConfigurationAlreadyExistsException(string message, string paramName, Exception innerException) : base(message, paramName, innerException)
        {
        }

        protected PrintConfigurationAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }

        public int? ApplicationTypeId { get; private set; }

        public int? TerritoryUnitId { get; private set;}
    }
}
