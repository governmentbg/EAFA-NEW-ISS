using System;
using IARA.Mobile.Domain.Interfaces;
using SQLite;

namespace IARA.Mobile.Pub.Domain.Entities.ScientificFishing
{
    public class SFOuting : IOfflineEntity
    {
        [PrimaryKey]
        public int Id { get; set; }
        public int PermitId { get; set; }
        public DateTime DateOfOuting { get; set; }
        public string WaterArea { get; set; }

        public bool IsLocalOnly { get; set; }
        public bool HasBeenUpdatedLocally { get; set; }
        public bool HasBeenDeletedLocally { get; set; }
    }
}
