using IARA.Mobile.Application.DTObjects.Common;
using IARA.Mobile.Insp.Application.Interfaces.Dtos;

namespace IARA.Mobile.Insp.Application.DTObjects.Nomenclatures.API
{
    public class PersonApiDto : IAddressDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public EgnLncDto EgnLnc { get; set; }
        public PersonnelAddressApiDto Address { get; set; }
    }
}
