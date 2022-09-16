using System;
using IARA.Mobile.Domain.Interfaces;
using SQLite;

namespace IARA.Mobile.Pub.Domain.Entities.CatchRecords
{
    public class CatchRecordFile : IOfflineEntity
    {
        [Indexed(Name = nameof(CatchRecordId), Order = 1, Unique = true)]
        public int Id { get; set; }

        [Indexed(Name = nameof(CatchRecordId), Order = 1, Unique = true)]
        public int CatchRecordId { get; set; }

        public string Name { get; set; }
        public string FullPath { get; set; }
        public long Size { get; set; }
        public string ContentType { get; set; }
        public DateTime UploadedOn { get; set; }
        public bool Deleted { get; set; }

        public bool IsLocalOnly { get; set; }
        public bool HasBeenUpdatedLocally { get; set; }
        public bool HasBeenDeletedLocally { get; set; }
    }
}
