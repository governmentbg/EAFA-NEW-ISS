using System;
using IARA.Mobile.Domain.Interfaces;
using SQLite;

namespace IARA.Mobile.Pub.Domain.Entities.CatchRecords
{
    public class CatchRecord : IOfflineEntity
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string Identifier { get; set; }
        public int TicketId { get; set; }
        public string WaterArea { get; set; }
        public DateTime CatchDate { get; set; }
        public string Description { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public int TotalCount { get; set; }
        public double TotalQuantity { get; set; }

        public bool IsLocalOnly { get; set; }
        public bool HasBeenUpdatedLocally { get; set; }
        public bool HasBeenDeletedLocally { get; set; }
    }
}
