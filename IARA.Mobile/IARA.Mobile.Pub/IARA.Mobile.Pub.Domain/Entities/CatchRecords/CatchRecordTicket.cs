using System;
using IARA.Mobile.Domain.Interfaces;
using SQLite;

namespace IARA.Mobile.Pub.Domain.Entities.CatchRecords
{
    public class CatchRecordTicket : IOfflineEntity
    {
        [PrimaryKey]
        public int Id { get; set; }
        public string TypeName { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        public string PersonFullName { get; set; }
        public string StatusName { get; set; }

        public bool IsLocalOnly { get; set; }
        public bool HasBeenUpdatedLocally { get; set; }
        public bool HasBeenDeletedLocally { get; set; }
    }
}
