using IARA.Common.Enums;
using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.DTOModels.Application
{
    public class ChangeOfCircumstancesDTO
    {
        public int Id { get; set; }

        public int TypeId { get; set; }

        public ChangeOfCircumstancesDataTypeEnum DataType { get; set; }

        public RegixPersonDataDTO Person { get; set; }

        public RegixLegalDataDTO Legal { get; set; }

        public AddressRegistrationDTO Address { get; set; }

        public int? ShipId { get; set; }

        public int? AquacultureFacilityId { get; set; }

        public int? PermitId { get; set; }

        public int? PermitLicenceId { get; set; }

        public int? BuyerId { get; set; }

        public string Description { get; set; }
    }
}
