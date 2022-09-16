using IARA.Mobile.Domain.Interfaces;
using SQLite;

namespace IARA.Mobile.Pub.Domain.Entities.ScientificFishing
{
    public class SFCatch : IOfflineEntity
    {
        [PrimaryKey]
        public int Id { get; set; }
        public int OutingId { get; set; }
        public int FishTypeId { get; set; }
        public int CatchUnder100 { get; set; }
        public int Catch100To500 { get; set; }
        public int Catch500To1000 { get; set; }
        public int CatchOver1000 { get; set; }
        public int TotalKeptCount { get; set; }
        public int TotalCatch { get; set; }

        public bool IsLocalOnly { get; set; }
        public bool HasBeenUpdatedLocally { get; set; }
        public bool HasBeenDeletedLocally { get; set; }
    }
}
