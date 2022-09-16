using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;

namespace IARA.DomainModels.DTOModels.LegalEntities
{
    public class LegalEntityEditDTO
    {
        public int Id { get; set; }

        public int? ApplicationId { get; set; }

        public bool IsOnlineApplication { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public RegixLegalDataDTO Legal { get; set; }

        [MinLength(1, ErrorMessageResourceName = "msgListMinLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<AddressRegistrationDTO> Addresses { get; set; }

        public List<AuthorizedPersonDTO> AuthorizedPeople { get; set; }

        public List<FileInfoDTO> Files { get; set; }
    }
}
