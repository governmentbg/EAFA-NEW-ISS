using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Files;

namespace IARA.DomainModels.DTOModels.ScientificFishing
{
    public class ScientificFishingPermitHolderDTO : ScientificFishingPermitHolderRegixDataDTO
    {
        public int? RequestNumber { get; set; }

        public int? PermitNumber { get; set; }

        [StringLength(500, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public string ScientificPosition { get; set; }

        public FileInfoDTO Photo { get; set; }

        public string PhotoBase64 { get; set; }
    }
}
