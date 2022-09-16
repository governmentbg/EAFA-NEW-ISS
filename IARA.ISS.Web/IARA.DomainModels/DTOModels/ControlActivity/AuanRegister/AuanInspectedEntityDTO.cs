using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.ControlActivity.Inspections;
using IARA.DomainModels.Nomenclatures;

namespace IARA.DomainModels.DTOModels.ControlActivity.AuanRegister
{
    public class AuanInspectedEntityDTO : NomenclatureDTO
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsUnregisteredPerson { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsPerson { get; set; }

        [RequiredIf(nameof(IsPerson), "msgRequired", typeof(ErrorResources), true)]
        public RegixPersonDataDTO Person { get; set; }

        [RequiredIf(nameof(IsPerson), "msgRequired", typeof(ErrorResources), false)]
        public RegixLegalDataDTO Legal { get; set; }

        [RequiredIf(nameof(IsUnregisteredPerson), "msgRequired", typeof(ErrorResources), true)]
        public UnregisteredPersonDTO UnregisteredPerson { get; set; }

        [StringLength(100, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string PersonWorkPlace { get; set; }

        [StringLength(100, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string PersonWorkPosition { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [MinLength(1, ErrorMessageResourceName = "msgListMinLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<AddressRegistrationDTO> Addresses { get; set; }
    }
}
