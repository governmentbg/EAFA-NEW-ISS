using System;
using System.Collections.Generic;
using IARA.DomainModels.DTOModels.Files;

namespace IARA.DomainModels.DTOModels.NewsPublic
{
    public class NewsDetailsDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }
        public DateTime? PublishStart { get; set; }
        public List<FileInfoDTO> Files { get; set; }
    }
}
