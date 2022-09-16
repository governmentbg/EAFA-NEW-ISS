using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using IARA.Logging.Abstractions.Models;
using Microsoft.Extensions.Logging;

namespace IARA.Logging.Abstractions.Interfaces
{
    public interface IExtendedLogger : ILogger
    {
        /// <summary>
        /// Логва възникнала грешка в базата. Ако не успее, логва във файл с име Error_log.txt
        /// </summary>
        /// <param name="ex">Грешка за логване</param>
        int LogException(Exception ex, [CallerFilePath] string callerFileName = null, [CallerMemberName] string callerName = null);

        int LogException(string message, Exception ex, [CallerFilePath] string callerFileName = null, [CallerMemberName] string callerName = null);

        /// <summary>
        /// Логва предупреждение в базата, само ако нивото за логване е настроено за записване на предупреждения
        /// </summary>
        /// <param name="ex">Предупреждение за логване</param>
        int LogWarning(string message, [CallerFilePath] string callerFileName = null, [CallerMemberName] string callerName = null);

        /// <summary>
        /// Логва информативно съобщение в базата, само ако нивото за логване е настроено за записване на предупреждения
        /// </summary>
        /// <param name="ex">Информация за логване</param>
        int LogInfo(string message, [CallerFilePath] string callerFileName = null, [CallerMemberName] string callerName = null);

        /// <summary>
        /// Логва съобщение в базата което може да бъде с различна тежест/severity - грешка, информативно съобщение, предупреждение и други.
        /// </summary>
        /// <param name="record">Запис за логване</param>
        int Log(LogRecord record);

        /// <summary>
        /// Логва списък от съобщения в базата които могат да бъдат с различна тежест/severity - грешки, информативни съобщения, предупредителни съобщения и други.  
        /// </summary>
        /// <param name="records">Записи за логване</param>
        List<int> Log(List<LogRecord> records);
    }
}
