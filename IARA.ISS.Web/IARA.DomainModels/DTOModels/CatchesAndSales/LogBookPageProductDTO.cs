using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Enums;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.CatchesAndSales
{
    public class LogBookPageProductDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? FishId { get; set; }

        public int? OriginDeclarationFishId { get; set; }

        public int? OriginProductId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public LogBookTypesEnum? LogBookType { get; set; }

        [RequiredIf(
            nameof(LogBookType),
            "msgRequired",
            typeof(ErrorResources), LogBookTypesEnum.Transportation, LogBookTypesEnum.Admission, LogBookTypesEnum.FirstSale)]
        [StringLength(500, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string CatchLocation { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? ProductPresentationId { get; set; }

        [RequiredIf(
            nameof(LogBookType),
            "msgRequired",
            typeof(ErrorResources), LogBookTypesEnum.Transportation, LogBookTypesEnum.Admission, LogBookTypesEnum.FirstSale)]
        public int? ProductFreshnessId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? ProductPurposeId { get; set; }

        [RequiredIf(
            nameof(LogBookType),
            "msgRequired",
            typeof(ErrorResources), LogBookTypesEnum.Transportation, LogBookTypesEnum.Admission, LogBookTypesEnum.FirstSale)]
        public decimal? MinSize { get; set; }

        [RequiredIf(nameof(LogBookType), "msgRequired", typeof(ErrorResources), LogBookTypesEnum.Aquaculture)]
        public decimal? AverageUnitWeightKg { get; set; }

        [RequiredIf(
            nameof(LogBookType),
            "msgRequired",
            typeof(ErrorResources), LogBookTypesEnum.Transportation, LogBookTypesEnum.Admission, LogBookTypesEnum.FirstSale)]
        public int? FishSizeCategoryId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public decimal? QuantityKg { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public decimal? UnitPrice { get; set; }

        public string TotalPrice { get; set; } // for UI

        public int? TurbotSizeGroupId { get; set; }

        public int? UnitCount { get; set; }

        /// <summary>
        /// For UI only
        /// </summary>
        public bool HasMissingProperties { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsActive { get; set; }
    }
}
