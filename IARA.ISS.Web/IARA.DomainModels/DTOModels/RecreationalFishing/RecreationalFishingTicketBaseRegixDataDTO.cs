using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.DTOModels.RecreationalFishing
{
    public class RecreationalFishingTicketBaseRegixDataDTO : BaseRegixChecksDTO
    {
        public int? Id { get; set; }

        [Required(ErrorMessageResourceName = "msgRequired", ErrorMessageResourceType = typeof(ErrorResources))]
        public RegixPersonDataDTO Person { get; set; }

        [MinLength(1, ErrorMessageResourceName = "msgListMinLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public List<AddressRegistrationDTO> PersonAddressRegistrations { get; set; }

        public RegixPersonDataDTO RepresentativePerson { get; set; }

        public List<AddressRegistrationDTO> RepresentativePersonAddressRegistrations { get; set; }

        public RecreationalFishingTelkDTO TelkData { get; set; }
    }
}
