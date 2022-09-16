using System;

namespace IARA.DomainModels.DTOModels.NewsManagment
{
    public class NewsManagementDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime? PublishStart { get; set; }
        public DateTime? PublishEnd { get; set; }
        public string CreatedBy { get; set; }
        public bool IsPublished { get; set; }
        public bool HasNotificationsSent { get; set; }
        public bool IsActive { get; set; }
    }
}
