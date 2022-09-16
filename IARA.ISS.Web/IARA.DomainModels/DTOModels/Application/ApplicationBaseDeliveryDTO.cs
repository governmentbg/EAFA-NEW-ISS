using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.DTOModels.Application
{
    public class ApplicationBaseDeliveryDTO
    {
        public int Id { get; set; }

        public int DeliveryTypeId { get; set; }

        public AddressRegistrationDTO DeliveryAddress { get; set; }

        public int? DeliveryTeritorryUnitId { get; set; }

        public string DeliveryEmailAddress { get; set; }
    }
}
