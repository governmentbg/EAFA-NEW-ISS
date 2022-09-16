using IARA.Common.CustomValidators;
using IARA.Common.Enums;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.DTOModels.Application
{
    public class ApplicationSubmittedForDTO : ApplicationSubmittedForRegixDataDTO
    {
        [RequiredIf(nameof(SubmittedByRole), "msgRequired", typeof(ErrorResources), SubmittedByRolesEnum.PersonalRepresentative, SubmittedByRolesEnum.LegalRepresentative)]
        public LetterOfAttorneyDTO SubmittedByLetterOfAttorney { get; set; }
    }
}
