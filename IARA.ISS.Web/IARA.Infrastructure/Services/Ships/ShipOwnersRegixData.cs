using System.Collections.Generic;
using System.Linq;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.Infrastructure.Services
{
    internal class ShipOwnersRegixData
    {
        public Dictionary<int, RegixPersonDataDTO> PersonRegixData { get; set; }
        public Dictionary<int, RegixLegalDataDTO> LegalRegixData { get; set; }
        public ILookup<int, AddressRegistrationDTO> PersonAddressData { get; set; }
        public ILookup<int, AddressRegistrationDTO> LegalAddressData { get; set; }
    }
}
