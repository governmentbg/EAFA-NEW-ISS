using System;

namespace IARA.Mobile.Pub.Application.DTObjects.News
{
    public class NewsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public bool HasImage { get; set; }
        public DateTime PublishStart { get; set; }
    }
}
