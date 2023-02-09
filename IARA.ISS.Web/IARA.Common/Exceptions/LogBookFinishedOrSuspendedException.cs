using System;
using System.Runtime.Serialization;

namespace IARA.Common.Exceptions
{
    public class LogBookFinishedOrSuspendedException : ArgumentException
    {
        public LogBookFinishedOrSuspendedException(int logBookId)
        {
            LogBookId = logBookId;
        }

        public int LogBookId { get; private set; }
    }
}
