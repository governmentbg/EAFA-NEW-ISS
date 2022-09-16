using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.DTOModels.Buyers
{
    public class PremisesDTO
    {
        public string Name { get; set; }
        public BuyerPremiseUsageDocumentDTO Document { get; set; }
        public string Landlord { get; set; }
        public string EgnEik { get; set; }
        public AddressRegistrationDTO Address { get; set; }
    }
}
