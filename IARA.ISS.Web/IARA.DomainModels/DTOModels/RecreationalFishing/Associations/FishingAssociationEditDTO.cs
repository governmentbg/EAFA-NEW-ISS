using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;

namespace IARA.DomainModels.DTOModels.RecreationalFishing.Associations
{
    public class FishingAssociationEditDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? ApplicationId { get; set; }

        public bool IsOnlineApplication { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public RegixLegalDataDTO SubmittedFor { get; set; }

        [MinLength(1, ErrorMessageResourceName = "msgListMinLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<AddressRegistrationDTO> SubmittedForAddresses { get; set; }

        [MinLength(1, ErrorMessageResourceName = "msgListMinLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<FishingAssociationPersonDTO> Persons { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public int? TerritoryUnitId { get; set; }

        public List<FileInfoDTO> Files { get; set; }
    }
}
