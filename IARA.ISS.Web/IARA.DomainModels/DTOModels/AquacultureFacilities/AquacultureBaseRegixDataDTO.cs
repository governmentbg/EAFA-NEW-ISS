using System.ComponentModel.DataAnnotations;
using IARA.Common.Enums;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.AquacultureFacilities
{
    public class AquacultureBaseRegixDataDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? ApplicationId { get; set; }

        public PageCodeEnum? PageCode { get; set; }

        public string StatusReason { get; set; }
    }
}
