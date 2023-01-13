using System.Collections.Generic;

namespace IARA.DomainModels.DTOModels.Common
{
    public class LegalFullDataDTO
    {
        public RegixLegalDataDTO Legal { get; set; }

        public List<AddressRegistrationDTO> Addresses { get; set; }
    }
}
