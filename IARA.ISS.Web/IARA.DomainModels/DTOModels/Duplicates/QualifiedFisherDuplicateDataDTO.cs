using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.DTOModels.Duplicates
{
    public class QualifiedFisherDuplicateDataDTO
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsOnline { get; set; }

        [RequiredIf(nameof(IsOnline), "msgRequired", typeof(ErrorResources), false)]
        public int? QualifiedFisherId { get; set; }

        [RequiredIf(nameof(IsOnline), "msgRequired", typeof(ErrorResources), true)]
        public RegixPersonDataDTO QualifiedFisher { get; set; }
    }
}
