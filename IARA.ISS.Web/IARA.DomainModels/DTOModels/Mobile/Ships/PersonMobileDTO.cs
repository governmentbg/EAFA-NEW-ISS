using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.DTOModels.Mobile.Ships
{
    public class PersonMobileDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public EgnLncDTO EgnLnc { get; set; }
        public ShipPersonnelAddressMobileDTO Address { get; set; }
    }
}
