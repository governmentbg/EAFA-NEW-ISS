using System.ComponentModel.DataAnnotations;
using IARA.Common.Enums;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.CatchesAndSales
{
    public class CatchRecordFishDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? FishId { get; set; }

        public string FishName { get; set; } // Only for UI table

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public decimal? QuantityKg { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? CatchTypeId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? CatchSizeId { get; set; }

        public int? TurbotCount { get; set; }

        public int? TurbotSizeGroupId { get; set; }

        public SturgeonGendersEnum? SturgeonGender { get; set; }

        public decimal? SturgeonWeightKg { get; set; }

        public decimal? SturgeonSize { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsContinentalCatch { get; set; }

        [StringLength(50, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string ThirdCountryCatchZone { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? CatchQuadrantId { get; set; }

        public string CatchQuadrant { get; set; } // only for UI in table

        public string CatchZone { get; set; } // only for UI in table

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsActive { get; set; }
    }
}
