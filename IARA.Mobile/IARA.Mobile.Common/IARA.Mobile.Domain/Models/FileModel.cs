using System;
using IARA.Mobile.Domain.Interfaces;

namespace IARA.Mobile.Domain.Models
{
    public class FileModel : IDtoBaseResult
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string FullPath { get; set; }
        public long Size { get; set; }
        public string ContentType { get; set; }
        public DateTime UploadedOn { get; set; }
        public int FileTypeId { get; set; }
        public bool Deleted { get; set; }
        public bool StoreOriginal { get; set; }
    }
}
