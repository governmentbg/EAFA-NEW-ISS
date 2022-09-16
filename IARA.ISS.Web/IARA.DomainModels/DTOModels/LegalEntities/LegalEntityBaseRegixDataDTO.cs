using System.Collections.Generic;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.DTOModels.LegalEntities
{
    public class LegalEntityBaseRegixDataDTO : BaseRegixChecksDTO
    {
        public int? Id { get; set; }

        public RegixPersonDataDTO Requester { get; set; }

        public List<AddressRegistrationDTO> RequesterAddresses { get; set; }

        public RegixLegalDataDTO Legal { get; set; }

        public List<AddressRegistrationDTO> Addresses { get; set; }
    }
}
