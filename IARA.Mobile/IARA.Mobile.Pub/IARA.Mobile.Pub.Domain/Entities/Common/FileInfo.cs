using System;
using SQLite;

namespace IARA.Mobile.Pub.Domain.Entities.Common
{
    public class FileInfo
    {
        [PrimaryKey]
        public int? Id { get; set; }
        public int ReferenceId { get; set; }
        public int FileTypeId { get; set; }
        public long Size { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
        public string Description { get; set; }
        public DateTime UploadedOn { get; set; }
        public bool Deleted { get; set; }
    }
}
