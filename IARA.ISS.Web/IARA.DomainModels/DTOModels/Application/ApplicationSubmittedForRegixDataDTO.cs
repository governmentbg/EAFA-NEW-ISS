using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.DTOModels.Application
{
    public class ApplicationSubmittedForRegixDataDTO
    {
        public SubmittedByRolesEnum SubmittedByRole { get; set; }

        [RequiredIf(nameof(SubmittedByRole), "msgRequired", typeof(ErrorResources), SubmittedByRolesEnum.LegalOwner, SubmittedByRolesEnum.LegalRepresentative)]
        public RegixLegalDataDTO Legal { get; set; }

        [RequiredIf(nameof(SubmittedByRole), "msgRequired", typeof(ErrorResources), SubmittedByRolesEnum.PersonalRepresentative)]
        public RegixPersonDataDTO Person { get; set; }

        [MinLength(1, ErrorMessageResourceName = "msgListMinLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<AddressRegistrationDTO> Addresses { get; set; }
    }
}
