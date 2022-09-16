using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.StatisticalForms.Reworks
{
    public class StatisticalFormReworkProductDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? ProductTypeId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsNewProductType { get; set; }

        [RequiredIf(nameof(IsNewProductType), "msgRequired", typeof(ErrorResources), true)]
        [StringLength(500, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string ProductTypeName { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public decimal? Tons { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsActive { get; set; }

    }
}
