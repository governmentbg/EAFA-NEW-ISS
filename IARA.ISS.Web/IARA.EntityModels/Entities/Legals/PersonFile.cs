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
    /// Снимка на физическо лице - връзка към Files
    /// </summary>
    [Table("PersonFiles", Schema = "Legals")]
    [Index(nameof(FileId), Name = "IXFK_PersonFiles_Files")]
    [Index(nameof(FileTypeId), Name = "IXFK_PersonFiles_NFileTypes")]
    [Index(nameof(RecordId), Name = "IXFK_PersonFiles_Persons")]
    [Index(nameof(FileId), nameof(RecordId), nameof(FileTypeId), Name = "UK_Legals_PersonFiles", IsUnique = true)]
    public partial class PersonFile
    {         public PersonFile()
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
        /// Физическо лице
        /// </summary>
        [Column("RecordID")]
        public int RecordId { get; set; }
        /// <summary>
        /// Файл на снимката
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
        [InverseProperty("PersonFiles")]
        public virtual File File { get; set; }
        [ForeignKey(nameof(FileTypeId))]
        [InverseProperty(nameof(NfileType.PersonFiles))]
        public virtual NfileType FileType { get; set; }
        [ForeignKey(nameof(RecordId))]
        [InverseProperty(nameof(Person.PersonFiles))]
        public virtual Person Record { get; set; }
    }
}
