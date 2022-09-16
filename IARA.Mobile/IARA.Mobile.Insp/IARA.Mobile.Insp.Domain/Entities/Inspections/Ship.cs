using IARA.Mobile.Domain.Interfaces;
using SQLite;

namespace IARA.Mobile.Insp.Domain.Entities.Inspections
{
    public class Ship : IEntity
    {
        [PrimaryKey]
        public int Id { get; set; }
        public int Uid { get; set; }
        public int? AssociationId { get; set; }
        public string Name { get; set; }
        public string CFR { get; set; }
        public string ExtMarkings { get; set; }
        public int FlagId { get; set; }
        public string UVI { get; set; }
        public string CallSign { get; set; }
        public int? ShipTypeId { get; set; }
        public string MMSI { get; set; }
        public int? FleetTypeId { get; set; }

        public string NormalizedShipName { get; set; }
        public string NormalizedCFR { get; set; }
        public string NormalizedExtMarkings { get; set; }
    }
}
