using System.Collections.Generic;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;

namespace IARA.DomainModels.DTOModels.RecreationalFishing
{
    public class RecreationalFishingTicketHolderDTO
    {
        public RegixPersonDataDTO Person { get; set; }
        public List<AddressRegistrationDTO> Addresses { get; set; }
        public FileInfoDTO Photo { get; set; }
    }
}
