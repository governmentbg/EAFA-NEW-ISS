using IARA.Mobile.Domain.Interfaces;
using SQLite;

namespace IARA.Mobile.Insp.Domain.Entities.Inspections
{
    public class ShipOwner : IEntity
    {
        [PrimaryKey]
        public int Id { get; set; }
        public int EntityId { get; set; }
        public int Type { get; set; }
        public int PersonType { get; set; }
        public int? PersonId { get; set; }
        public int? LegalId { get; set; }
        public int ShipUid { get; set; }
        public string PersonEGN { get; set; }
        public string LegalEIK { get; set; }
    }
}
