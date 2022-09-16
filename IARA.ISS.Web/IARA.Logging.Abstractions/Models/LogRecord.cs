using System;
using Microsoft.Extensions.Logging;

namespace IARA.Logging.Abstractions.Models
{
    public class LogRecord
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


        public override string ToString()
        {
            return $"UserName: {Username}{Environment.NewLine}"
                 + $"Message: {Message}{Environment.NewLine}"
                 + $"ExceptionSource: {ExceptionSource}{Environment.NewLine}"
                 + $"StackTrace: {StackTrace}{Environment.NewLine}"
                 + $"LogDate: {LogDate}"
                 + $"Client: {Client}";
        }
    }
}
