using System;
using System.Collections.Generic;

namespace IARA.DomainModels.DTOModels.SystemLog
{
    public class BaseSystemLogDTO
    {
        public string Application { get; set; }

        public string Module { get; set; }

        public string Action { get; set; }

        public string Username { get; set; }

        public string IPAddress { get; set; }

        public string BrowserInfo { get; set; }

        public DateTime LogDate { get; set; }

        public string EventUID { get; set; }

        public string TableId { get; set; }

        public List<SystemLogDTO> SystemLogs { get; set; }
    }
}
