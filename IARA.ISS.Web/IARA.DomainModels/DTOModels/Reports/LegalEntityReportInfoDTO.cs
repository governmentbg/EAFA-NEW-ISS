using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.DTOModels.Reports
{
    public class LegalEntityReportInfoDTO
    {
        public int Id { get; set; }

        public string LegalName { get; set; }

        public string Eik { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string PostalCode { get; set; }

        public AddressRegistrationDTO CorrespondenceAddress { get; set; }

        public AddressRegistrationDTO CourtRegistrationAddress { get; set; }
    }
}
