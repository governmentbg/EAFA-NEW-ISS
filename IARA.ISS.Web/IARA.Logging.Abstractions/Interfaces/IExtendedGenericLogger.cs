using Microsoft.Extensions.Logging;

namespace IARA.Logging.Abstractions.Interfaces
{
    public interface IExtendedLogger<T> : ILogger<T>, IExtendedLogger
    {
    }
}
