using System;
using System.Threading.Tasks;

namespace IARA.Mobile.Application.Interfaces.Utilities
{
    public interface IExceptionHandler
    {
        /// <summary>
        /// Sends the given exception to the server
        /// </summary>
        /// <param name="exception">The exception</param>
        Task HandleException(Exception exception);

        /// <summary>
        /// Logs message to the server
        /// </summary>
        /// <param name="message">Message to log</param>
        Task DebugLog(string message);
    }
}
