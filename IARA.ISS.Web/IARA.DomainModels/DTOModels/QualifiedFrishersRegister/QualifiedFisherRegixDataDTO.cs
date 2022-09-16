using System.Collections.Generic;
using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.DTOModels.QualifiedFrishersRegister
{
    public class QualifiedFisherRegixDataDTO : BaseRegixChecksDTO
    {
        public RegixPersonDataDTO SubmittedByRegixData { get; set; }

        public List<AddressRegistrationDTO> SubmittedByAddresses { get; set; }

        public SubmittedByRolesEnum SubmittedByRole { get; set; }

        public RegixPersonDataDTO SubmittedForRegixData { get; set; }

        public List<AddressRegistrationDTO> SubmittedForAddresses { get; set; }

        //IApplicationRegister
        public int? Id { get; set; }
    }
}
