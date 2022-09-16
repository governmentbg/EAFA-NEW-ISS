using System.Collections.Generic;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.RegixAbstractions.Models
{
    public class RegixLegalContext
    {
        public RegixLegalDataDTO Legal { get; set; }
        public List<AddressRegistrationDTO> Addresses { get; set; }
    }
}
