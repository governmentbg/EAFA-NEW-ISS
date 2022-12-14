// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IARA.EntityModels.Entities
{
    /// <summary>
    /// Прикачени документи към новини
    /// </summary>
    [Table("NewsFiles", Schema = "News")]
    [Index(nameof(FileId), Name = "IXFK_NewsFiles_Files")]
    [Index(nameof(FileTypeId), Name = "IXFK_NewsFiles_NFileTypes")]
    [Index(nameof(RecordId), Name = "IXFK_NewsFiles_News")]
    [Index(nameof(FileId), nameof(RecordId), nameof(FileTypeId), Name = "UK_News_NewsFiles", IsUnique = true)]
    public partial class NewsFile
    {         public NewsFile()
        {
            this.IsActive = true;
        }


        /// <summary>
        /// Уникален идентификатор
        /// </summary>
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        /// <summary>
        /// Тип на файла
        /// </summary>
        [Column("FileTypeID")]
        public int FileTypeId { get; set; }
        [Column("RecordID")]
        public int RecordId { get; set; }
        [Column("FileID")]
        public int FileId { get; set; }
        /// <summary>
        /// Флаг дали записът е активен или изтрит
        /// </summary>
        [Required]
        public bool IsActive { get; set; }
        /// <summary>
        /// Потребител създал записа
        /// </summary>
        [Required]
        [StringLength(500)]
        public string CreatedBy { get; set; }
        /// <summary>
        /// Дата и час на създаване на записа
        /// </summary>
        public DateTime CreatedOn { get; set; }
        /// <summary>
        /// Потребител последно актуализирал записа
        /// </summary>
        [StringLength(500)]
        public string UpdatedBy { get; set; }
        /// <summary>
        /// Дата и час на последна актуализация на записа
        /// </summary>
        public DateTime? UpdatedOn { get; set; }

        [ForeignKey(nameof(FileId))]
        [InverseProperty("NewsFiles")]
        public virtual File File { get; set; }
        [ForeignKey(nameof(FileTypeId))]
        [InverseProperty(nameof(NfileType.NewsFiles))]
        public virtual NfileType FileType { get; set; }
        [ForeignKey(nameof(RecordId))]
        [InverseProperty(nameof(News.NewsFiles))]
        public virtual News Record { get; set; }
    }
}
