using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.ScientificFishing
{
    public class ScientificFishingOutingCatchDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? OutingId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? FishTypeId { get; set; }

        public NomenclatureDTO FishType { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [Range(0, int.MaxValue, ErrorMessageResourceName = "msgPositiveIntegerValue", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? CatchUnder100 { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [Range(0, int.MaxValue, ErrorMessageResourceName = "msgPositiveIntegerValue", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? Catch100To500 { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [Range(0, int.MaxValue, ErrorMessageResourceName = "msgPositiveIntegerValue", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? Catch500To1000 { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [Range(0, int.MaxValue, ErrorMessageResourceName = "msgPositiveIntegerValue", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? CatchOver1000 { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [Range(0, int.MaxValue, ErrorMessageResourceName = "msgPositiveIntegerValue", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? TotalKeptCount { get; set; }

        public int TotalCatch { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsActive { get; set; }
    }
}
