using System;
using System.Collections.Generic;
using System.Text;
using IARA.DomainModels.DTOModels.Common;
using IARA.DomainModels.DTOModels.Files;

namespace IARA.DomainModels.DTOModels.FishingTickets
{
    public class FishingTicketPersonDataDTO
    {
        public RegixPersonDataDTO Person { get; set; }
        public FileInfoDTO Photo { get; set; }
        public List<AddressRegistrationDTO> UserAddresses { get; set; }
    }
}
