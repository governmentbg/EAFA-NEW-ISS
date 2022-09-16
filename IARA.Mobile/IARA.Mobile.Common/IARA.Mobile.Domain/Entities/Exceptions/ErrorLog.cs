using IARA.Mobile.Domain.Enums;
using SQLite;
using System;

namespace IARA.Mobile.Domain.Entities.Exceptions
{
    public class ErrorLog
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public LogLevel Level { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public string ExceptionSource { get; set; }
        public DateTime LogDate { get; set; }
    }
}
