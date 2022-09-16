using System.Collections.Generic;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.DTOModels.Reports
{
    public class PersonReportInfoDTO
    {
        public int Id { get; set; }

        public string Comments { get; set; }

        public RegixPersonDataDTO RegixPersonData { get; set; }

        public List<AddressRegistrationDTO> AddressRegistrations { get; set; }
    }
}
