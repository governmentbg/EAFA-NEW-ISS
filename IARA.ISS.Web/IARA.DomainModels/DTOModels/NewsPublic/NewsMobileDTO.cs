using System;
using System.Collections.Generic;
using System.Text;

namespace IARA.DomainModels.DTOModels.NewsPublic
{
    public class NewsMobileDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public bool HasImage { get; set; }
        public DateTime? PublishStart { get; set; }
        public DateTime? PublishEnd { get; set; }
    }
}
