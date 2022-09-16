using System.Collections.Generic;
using System.Linq;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.Infrastructure.Services
{
    internal class CapacityHoldersRegixData
    {
        public Dictionary<int, RegixPersonDataDTO> Persons { get; set; }
        public Dictionary<int, RegixLegalDataDTO> Legals { get; set; }
        public ILookup<int, AddressRegistrationDTO> PersonAddresses { get; set; }
        public ILookup<int, AddressRegistrationDTO> LegalAddresses { get; set; }
    }
}
