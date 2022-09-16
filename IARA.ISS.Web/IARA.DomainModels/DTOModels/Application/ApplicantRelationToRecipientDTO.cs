using System.ComponentModel.DataAnnotations;
using IARA.Common.CustomValidators;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.DTOModels.Application
{
    public class ApplicantRelationToRecipientDTO
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public SubmittedByRolesEnum? Role { get; set; }

        [RequiredIf(nameof(Role), "msgRequired", typeof(ErrorResources), SubmittedByRolesEnum.PersonalRepresentative, SubmittedByRolesEnum.LegalRepresentative)]
        public LetterOfAttorneyDTO LetterOfAttorney { get; set; }
    }
}
