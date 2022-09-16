using IARA.Mobile.Application.DTObjects.Common;
using IARA.Mobile.Domain.Interfaces;

namespace IARA.Mobile.Insp.Application.DTObjects.Inspections
{
    public class UnregisteredPersonDto : IActive
    {
        public int? Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public bool? HasBulgarianAddressRegistration { get; set; }
        public EgnLncDto EgnLnc { get; set; }
        public string Eik { get; set; }
        public bool IsLegal { get; set; }
        public int? CitizenshipId { get; set; }
        public string Comment { get; set; }
        public bool IsActive { get; set; }
    }
}
