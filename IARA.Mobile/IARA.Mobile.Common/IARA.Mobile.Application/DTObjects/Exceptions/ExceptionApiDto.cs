using IARA.Mobile.Domain.Enums;
using System;

namespace IARA.Mobile.Application.DTObjects.Exceptions
{
    public class ExceptionApiDto
    {
        public LogLevel Level { get; set; }
        public string Message { get; set; }
        public string CallerFileName { get; set; }
        public string CallerName { get; set; }
        public string StackTrace { get; set; }
        public string ExceptionSource { get; set; }
        public string Client { get; set; }

        public DateTime? LogDate { get; set; }
        public string Username { get; set; }
    }
}
