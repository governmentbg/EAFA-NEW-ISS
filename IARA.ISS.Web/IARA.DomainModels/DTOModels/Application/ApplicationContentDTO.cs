using System.Collections.Generic;
using IARA.DomainModels.DTOModels.Files;

namespace IARA.DomainModels.DTOModels.Application
{
    public class ApplicationContentDTO
    {
        public int ApplicationId { get; set; }

        public string DraftContent { get; set; }

        public List<FileInfoDTO> Files { get; set; }
    }
}
