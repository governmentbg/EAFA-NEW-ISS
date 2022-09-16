using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.DTOModels.ShipsRegister
{
    public class ShipOwnerRegixDataDTO
    {
        public int? Id { get; set; }

        public string Names { get; set; }

        public string EgnLncEik { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsOwnerPerson { get; set; }

        [RequiredIf(nameof(IsOwnerPerson), "msgRequired", typeof(ErrorResources), true)]
        public RegixPersonDataDTO RegixPersonData { get; set; }

        [RequiredIf(nameof(IsOwnerPerson), "msgRequired", typeof(ErrorResources), false)]
        public RegixLegalDataDTO RegixLegalData { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [MinLength(1, ErrorMessageResourceName = "msgListMinLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<AddressRegistrationDTO> AddressRegistrations { get; set; }

        public bool HasValidationErrors { get; set; }

        public bool HasRegixDataDiscrepancy { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsActive { get; set; }
    }
}
