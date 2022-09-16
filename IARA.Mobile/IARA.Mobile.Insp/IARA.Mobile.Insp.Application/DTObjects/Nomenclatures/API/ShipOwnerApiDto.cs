using IARA.Mobile.Domain.Interfaces;

namespace IARA.Mobile.Insp.Application.DTObjects.Nomenclatures.API
{
    public class ShipOwnerApiDto : IActive
    {
        public int Id { get; set; }
        public int? PersonId { get; set; }
        public int? LegalId { get; set; }
        public int ShipUid { get; set; }
        public bool IsActive { get; set; }
    }
}
