using System;
using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.RecreationalFishing
{
    public class RecreationalFishingTelkDTO
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsIndefinite { get; set; }

        [StringLength(50, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Num { get; set; }

        [RequiredIf(nameof(IsIndefinite), "msgRequired", typeof(ErrorResources), false)]
        public DateTime? ValidTo { get; set; }
    }
}
