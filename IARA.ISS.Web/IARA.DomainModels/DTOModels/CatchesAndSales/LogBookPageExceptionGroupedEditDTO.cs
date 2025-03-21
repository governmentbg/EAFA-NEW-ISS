using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Files;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IARA.DomainModels.DTOModels.CatchesAndSales
{
    public class LogBookPageExceptionGroupedEditDTO
    {
        public List<int> LogBookPageExceptionIds { get; set; }

        public List<int> UserIds { get; set; }

        public List<int> LogBookTypeIds { get; set; }

        public int? LogBookId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public DateTime? ExceptionActiveFrom { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public DateTime? ExceptionActiveTo { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public DateTime? EditPageFrom { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public DateTime? EditPageTo { get; set; }

        public List<FileInfoDTO> Files { get; set; }
    }
}
