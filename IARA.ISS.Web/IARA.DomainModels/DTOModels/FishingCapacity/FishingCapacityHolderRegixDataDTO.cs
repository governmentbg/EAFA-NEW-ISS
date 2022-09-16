using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.DTOModels.FishingCapacity
{
    public class FishingCapacityHolderRegixDataDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsHolderPerson { get; set; }

        [RequiredIf(nameof(IsHolderPerson), "msgRequired", typeof(ErrorResources), true)]
        public RegixPersonDataDTO Person { get; set; }

        [RequiredIf(nameof(IsHolderPerson), "msgRequired", typeof(ErrorResources), false)]
        public RegixLegalDataDTO Legal { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [MinLength(1, ErrorMessageResourceName = "msgListMinLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<AddressRegistrationDTO> Addresses { get; set; }

        public string Name { get; set; } // for UI only

        public string EgnEik { get; set; } // for UI only

        public bool HasRegixDataDiscrepancy { get; set; } // for UI only

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsActive { get; set; }
    }
}
