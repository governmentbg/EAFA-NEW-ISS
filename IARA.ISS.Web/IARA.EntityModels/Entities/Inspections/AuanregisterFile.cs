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
    /// Файлове към АУАН
    /// </summary>
    [Table("AUANRegisterFiles", Schema = "RInsp")]
    [Index(nameof(RecordId), Name = "IXFK_AUANRegisterFiles_AUANRegister")]
    [Index(nameof(FileId), Name = "IXFK_AUANRegisterFiles_Files")]
    [Index(nameof(FileTypeId), Name = "IXFK_AUANRegisterFiles_NFileTypes")]
    [Index(nameof(FileTypeId), nameof(RecordId), nameof(FileId), Name = "UK_RInsp_AUANRegisterFiles", IsUnique = true)]
    public partial class AuanregisterFile
    {         public AuanregisterFile()
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
        /// <summary>
        /// АУАН
        /// </summary>
        [Column("RecordID")]
        public int RecordId { get; set; }
        /// <summary>
        /// Файл
        /// </summary>
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
        [InverseProperty("AuanregisterFiles")]
        public virtual File File { get; set; }
        [ForeignKey(nameof(FileTypeId))]
        [InverseProperty(nameof(NfileType.AuanregisterFiles))]
        public virtual NfileType FileType { get; set; }
        [ForeignKey(nameof(RecordId))]
        [InverseProperty(nameof(Auanregister.AuanregisterFiles))]
        public virtual Auanregister Record { get; set; }
    }
}
