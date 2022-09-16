using System;

namespace IARA.Mobile.Pub.Application.DTObjects.News
{
    public class NewsFiltersDto
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public int[] DistrictsIds { get; set; }
        public string FreeTextSearch { get; set; }
    }
}
