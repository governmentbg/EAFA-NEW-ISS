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
    /// Прикачени файлове за корабен регистър
    /// </summary>
    [Table("ShipRegisterFiles", Schema = "RShips")]
    [Index(nameof(FileTypeId), Name = "IXFK_ShipRegisterFiles_NFileTypes")]
    [Index(nameof(RecordId), Name = "IXFK_ShipRegister_Files_ShipRegister")]
    [Index(nameof(FileId), nameof(RecordId), nameof(FileTypeId), Name = "UK_RS_ShipRegister_Files", IsUnique = true)]
    public partial class ShipRegisterFile
    {         public ShipRegisterFile()
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
        [InverseProperty("ShipRegisterFiles")]
        public virtual File File { get; set; }
        [ForeignKey(nameof(FileTypeId))]
        [InverseProperty(nameof(NfileType.ShipRegisterFiles))]
        public virtual NfileType FileType { get; set; }
        [ForeignKey(nameof(RecordId))]
        [InverseProperty(nameof(ShipRegister.ShipRegisterFiles))]
        public virtual ShipRegister Record { get; set; }
    }
}
