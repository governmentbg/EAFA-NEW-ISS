using IARA.Mobile.Domain.Interfaces;
using SQLite;

namespace IARA.Mobile.Insp.Domain.Entities.Inspections
{
    public class PoundNetFishingGearMark : IEntity
    {
        [PrimaryKey]
        public int Id { get; set; }
        public int FishingGearId { get; set; }
        public string Number { get; set; }
        public int StatusId { get; set; }
    }
}
