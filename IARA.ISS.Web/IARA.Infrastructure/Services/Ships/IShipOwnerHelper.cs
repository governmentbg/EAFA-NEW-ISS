using System.Collections.Generic;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.Infrastructure.Services
{
    internal interface IShipOwnerHelper
    {
        public bool? IsOwnerPerson { get; set; }
        public int? PersonId { get; set; }
        public int? LegalId { get; set; }
        public RegixPersonDataDTO RegixPersonData { get; set; }
        public RegixLegalDataDTO RegixLegalData { get; set; }
        public List<AddressRegistrationDTO> AddressRegistrations { get; set; }

    }
}
