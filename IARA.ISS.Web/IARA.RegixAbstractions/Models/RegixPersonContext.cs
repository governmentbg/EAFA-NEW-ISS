using System.Collections.Generic;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.RegixAbstractions.Models
{
    public class RegixPersonContext
    {
        public RegixPersonDataDTO Person { get; set; }
        public List<AddressRegistrationDTO> Addresses { get; set; }
    }
}
