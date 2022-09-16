using System;

namespace IARA.Logging.Abstractions.Models
{
    public class RequestData
    {
        public string IPAddress { get; set; }
        public string Endpoint { get; set; }
        public DateTime TimeOfRequest { get; set; }
        public string BrowserInfo { get; set; }
        public string Username { get; set; }
    }
}
