using IARA.Mobile.Domain.Interfaces;
using SQLite;

namespace IARA.Mobile.Insp.Domain.Entities.Inspections
{
    public class ShipOwner : IEntity
    {
        [PrimaryKey]
        public int Id { get; set; }
        public int? PersonId { get; set; }
        public int? LegalId { get; set; }
        public int ShipUid { get; set; }
    }
}
