using System.Collections.Generic;
using IARA.DomainModels.DTOModels.Files;

namespace IARA.DomainModels.DTOModels.Common
{
    public class PersonFullDataDTO
    {
        public RegixPersonDataDTO Person { get; set; }

        public List<AddressRegistrationDTO> Addresses { get; set; }

        public FileInfoDTO Photo { get; set; }
    }
}
