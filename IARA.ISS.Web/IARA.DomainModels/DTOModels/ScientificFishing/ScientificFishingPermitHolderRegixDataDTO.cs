using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.DTOModels.ScientificFishing
{
    public class ScientificFishingPermitHolderRegixDataDTO
    {
        public int? Id { get; set; }

        public int? OwnerId { get; set; }

        public string Name { get; set; }

        public string Egn { get; set; }

        public bool HasValidationErrors { get; set; }

        public bool HasRegixDataDiscrepancy { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public RegixPersonDataDTO RegixPersonData { get; set; }

        public List<AddressRegistrationDTO> AddressRegistrations { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsActive { get; set; }
    }
}
