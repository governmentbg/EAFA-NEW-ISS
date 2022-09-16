using System;
using System.Collections.Generic;
using IARA.DomainModels.DTOModels.Files;

namespace IARA.DomainModels.DTOModels.NewsPublic
{
    public class NewsDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string MainImage { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? PublishStart { get; set; }
        public DateTime? PublishEnd { get; set; }
        public bool HasNotificationsSent { get; set; }
    }
}
