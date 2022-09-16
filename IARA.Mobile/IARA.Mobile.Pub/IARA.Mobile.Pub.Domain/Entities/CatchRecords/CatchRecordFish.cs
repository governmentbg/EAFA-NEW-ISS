using IARA.Mobile.Domain.Interfaces;
using SQLite;

namespace IARA.Mobile.Pub.Domain.Entities.CatchRecords
{
    public class CatchRecordFish : IOfflineEntity
    {
        [PrimaryKey]
        public int Id { get; set; }
        public int CatchRecordId { get; set; }
        public int FishTypeId { get; set; }
        public int Count { get; set; }
        public double Quantity { get; set; }

        public bool IsLocalOnly { get; set; }
        public bool HasBeenUpdatedLocally { get; set; }
        public bool HasBeenDeletedLocally { get; set; }
    }
}
