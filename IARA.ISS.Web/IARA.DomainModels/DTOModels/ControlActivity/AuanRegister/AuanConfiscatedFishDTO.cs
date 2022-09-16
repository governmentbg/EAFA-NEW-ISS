using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.ControlActivity.AuanRegister
{
    public class AuanConfiscatedFishDTO
    {
        public int? Id { get; set; }

        public int? FishTypeId { get; set; }

        public decimal? Weight { get; set; }

        public int? Count { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? ConfiscationActionId { get; set; }

        public int? TerritoryUnitId { get; set; }

        public int? TurbotSizeGroupId { get; set; }

        public int? ApplianceId { get; set; }

        [StringLength(2000, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Comments { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsActive { get; set; }
    }
}
