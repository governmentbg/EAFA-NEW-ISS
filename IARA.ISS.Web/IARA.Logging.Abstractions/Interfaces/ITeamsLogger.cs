using System.Threading.Tasks;
using IARA.Logging.Abstractions.Models;
using Microsoft.Extensions.Logging;

namespace IARA.Logging.Abstractions.Interfaces
{
    public interface ITeamsLogger
    {
        Task<bool> Log(LogRecord record);
        LogLevel LogLevel { get; }
    }
}
