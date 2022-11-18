using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.DTOModels.RecreationalFishing.Associations
{
    public class FishingAssociationRegixDataDTO : FishingAssociationBaseRegixDataDTO
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public RegixPersonDataDTO SubmittedBy { get; set; }

        [MinLength(1, ErrorMessageResourceName = "msgListMinLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<AddressRegistrationDTO> SubmittedByAddresses { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public RegixLegalDataDTO SubmittedFor { get; set; }

        [MinLength(1, ErrorMessageResourceName = "msgListMinLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<AddressRegistrationDTO> SubmittedForAddresses { get; set; }

        [MinLength(1, ErrorMessageResourceName = "msgListMinLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<FishingAssociationPersonDTO> Persons { get; set; }
    }
}
