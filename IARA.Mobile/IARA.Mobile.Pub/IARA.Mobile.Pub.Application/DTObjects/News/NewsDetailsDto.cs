using System;

namespace IARA.Mobile.Pub.Application.DTObjects.News
{
    public class NewsDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public bool HasImage { get; set; }
        public DateTime PublishStart { get; set; }
    }
}
