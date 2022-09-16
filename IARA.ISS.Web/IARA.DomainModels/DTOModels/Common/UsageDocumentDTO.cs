using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.Common
{
    public class UsageDocumentDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? DocumentTypeId { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [StringLength(50, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string DocumentNum { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public bool? IsDocumentIndefinite { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public DateTime? DocumentValidFrom { get; set; }

        [RequiredIf(nameof(IsDocumentIndefinite), "msgRequired", typeof(ErrorResources), false)]
        public DateTime? DocumentValidTo { get; set; }

        public bool? IsLessorPerson { get; set; }

        [RequiredIf(nameof(IsLessorPerson), "msgRequired", typeof(ErrorResources), true)]
        public RegixPersonDataDTO LessorPerson { get; set; }

        [RequiredIf(nameof(IsLessorPerson), "msgRequired", typeof(ErrorResources), false)]
        public RegixLegalDataDTO LessorLegal { get; set; }

        [MinLength(1, ErrorMessageResourceName = "msgListMinLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<AddressRegistrationDTO> LessorAddresses { get; set; }

        public string Comments { get; set; }

        public bool? IsActive { get; set; }
    }
}
