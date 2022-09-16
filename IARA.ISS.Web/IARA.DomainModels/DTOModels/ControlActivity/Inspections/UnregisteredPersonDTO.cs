using IARA.DomainModels.DTOModels.Common;

namespace IARA.DomainModels.DTOModels.ControlActivity.Inspections
{
    public class UnregisteredPersonDTO
    {
        public int? Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public bool? HasBulgarianAddressRegistration { get; set; }
        public EgnLncDTO EgnLnc { get; set; }
        public string Eik { get; set; }
        public bool IsLegal { get; set; }
        public int? CitizenshipId { get; set; }
        public string Comment { get; set; }
        public bool IsActive { get; set; }
    }
}
