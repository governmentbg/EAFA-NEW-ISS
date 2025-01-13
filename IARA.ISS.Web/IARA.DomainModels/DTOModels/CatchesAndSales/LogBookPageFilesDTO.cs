using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Files;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IARA.DomainModels.DTOModels.CatchesAndSales
{
    public class LogBookPageFilesDTO
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? LogBookPageId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public LogBookTypesEnum? LogBookType { get; set; }

        public List<FileInfoDTO> Files { get; set; }
    }
}
