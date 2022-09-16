using System;
using System.ComponentModel.DataAnnotations;
using IARA.Common.Resources;
using Microsoft.AspNetCore.Http;

namespace IARA.DomainModels.DTOModels.Files
{
    public class FileInfoDTO
    {
        public FileInfoDTO()
        { }

        public FileInfoDTO(IFormFile file, int fileTypeId)
        {
            this.Name = file?.FileName;
            this.ContentType = file?.ContentType;
            this.Size = file != null ? file.Length : default;
            this.Deleted = false;
            this.FileTypeId = fileTypeId;
            this.File = file;
            this.StoreOriginal = false;
        }

        public int? Id { get; set; }

        public IFormFile File { get; set; }

        public int FileTypeId { get; set; }

        public long Size { get; set; }

        [StringLength(500, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Name { get; set; }

        [StringLength(100, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string ContentType { get; set; }

        [StringLength(4000, ErrorMessageResourceName = "msgMaxLength", ErrorMessageResourceType = typeof(ErrorResources))]
        public string Description { get; set; }

        public DateTime UploadedOn { get; set; }
        public bool Deleted { get; set; } = false;
        public bool StoreOriginal { get; set; } = false;
    }
}
