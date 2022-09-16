using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Resources;

namespace IARA.DomainModels.DTOModels.Common
{
    public class UsageDocumentRegixDataDTO
    {
        public int Id { get; set; }

        public int DocumentTypeId { get; set; }

        public bool? IsLessorPerson { get; set; }

        [RequiredIf(nameof(IsLessorPerson), "msgRequired", typeof(ErrorResources), true)]
        public RegixPersonDataDTO LessorPerson { get; set; }

        [RequiredIf(nameof(IsLessorPerson), "msgRequired", typeof(ErrorResources), false)]
        public RegixLegalDataDTO LessorLegal { get; set; }

        [MinLength(1, ErrorMessageResourceName = "msgListMinLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<AddressRegistrationDTO> LessorAddresses { get; set; }

        public bool IsActive { get; set; }
    }
}
