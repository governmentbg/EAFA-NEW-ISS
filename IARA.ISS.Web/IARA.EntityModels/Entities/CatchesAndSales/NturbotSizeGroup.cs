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
    /// Размерни групи за калкан
    /// </summary>
    [Table("NTurbotSizeGroups", Schema = "CatchSales")]
    [Index(nameof(Code), nameof(ValidTo), Name = "UK_CatchSales_NTurbotSizeGroups", IsUnique = true)]
    public partial class NturbotSizeGroup
    {
        public NturbotSizeGroup()
        {
            AuanconfiscatedFishes = new HashSet<AuanconfiscatedFish>();
            CatchRecordFishes = new HashSet<CatchRecordFish>();
            LogBookPageProducts = new HashSet<LogBookPageProduct>();
        }

        /// <summary>
        /// Уникален идентификатор
        /// </summary>
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        /// <summary>
        /// код
        /// </summary>
        [Required]
        [StringLength(10)]
        public string Code { get; set; }
        /// <summary>
        /// Име
        /// </summary>
        [Required]
        [StringLength(500)]
        public string Name { get; set; }
        /// <summary>
        /// Начална дата на валидност на записа
        /// </summary>
        public DateTime ValidFrom { get; set; }
        /// <summary>
        /// Крайна дата на валидност на записа
        /// </summary>
        public DateTime ValidTo { get; set; }
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

        [InverseProperty(nameof(AuanconfiscatedFish.TurbotSizeGroup))]
        public virtual ICollection<AuanconfiscatedFish> AuanconfiscatedFishes { get; set; }
        [InverseProperty(nameof(CatchRecordFish.TurbotSizeGroup))]
        public virtual ICollection<CatchRecordFish> CatchRecordFishes { get; set; }
        [InverseProperty(nameof(LogBookPageProduct.TurbotSizeGroup))]
        public virtual ICollection<LogBookPageProduct> LogBookPageProducts { get; set; }
    }
}