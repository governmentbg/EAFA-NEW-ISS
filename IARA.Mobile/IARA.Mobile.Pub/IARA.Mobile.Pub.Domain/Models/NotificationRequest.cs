using System.Collections.Generic;

namespace IARA.Mobile.Pub.Domain.Models
{
    public class NotificationRequest
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Group { get; set; }
        public IDictionary<string, object> Data { get; set; }
    }
}
