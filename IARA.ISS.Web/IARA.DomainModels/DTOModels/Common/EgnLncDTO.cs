using System.ComponentModel.DataAnnotations;
using IARA.Common.Enums;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.Common
{
    public class EgnLncDTO
    {
        [StringLength(15, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public string EgnLnc { get; set; }

        public IdentifierTypeEnum IdentifierType { get; set; }

        public override string ToString()
        {
            return $"{IdentifierType}|{EgnLnc}";
        }
    }
}
