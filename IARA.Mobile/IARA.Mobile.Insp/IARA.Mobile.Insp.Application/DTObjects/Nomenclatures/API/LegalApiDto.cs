using IARA.Mobile.Insp.Application.Interfaces.Dtos;

namespace IARA.Mobile.Insp.Application.DTObjects.Nomenclatures.API
{
    public class LegalApiDto : IAddressDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Eik { get; set; }
        public PersonnelAddressApiDto Address { get; set; }
    }
}
