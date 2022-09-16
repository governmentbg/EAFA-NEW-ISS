using System;
using System.Collections.Generic;
using System.Text;

namespace IARA.DomainModels.RequestModels
{
    public class NewsManagmentFilters : BaseRequestModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public bool? IsPublished { get; set; }
    }
}
