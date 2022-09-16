using System;


namespace IARA.DomainModels.DTOModels.ErrorLog
{
    public class ErrorLogDTO
    {
        public DateTime LogDate { get; set; }
        public string Username { get; set; }
        public string Severity { get; set; }

        public string Client { get; set; }

        public string Class { get; set; }

        public string Method { get; set; }
        public string ExceptionSource { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }

    }
}
