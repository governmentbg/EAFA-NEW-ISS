using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.DTOModels.User
{
    public class ChangeUserLegalDTO : UserLegalDTO
    {
        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public RegixLegalDataDTO Legal { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        [MinLength(1, ErrorMessageResourceName = "msgListMinLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<AddressRegistrationDTO> Addresses { get; set; }

        public int? TerritoryUnitId { get; set; }

        public int AssociationId { get; set; }

        //for frontend
        public bool? HasMissingProperties { get; set; }
    }
}
