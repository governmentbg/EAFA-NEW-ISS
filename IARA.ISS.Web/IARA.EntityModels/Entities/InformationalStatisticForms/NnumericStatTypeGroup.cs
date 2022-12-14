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
    /// Групи числова статистическа информация (Приходи, Разходи, Стойност на Информация за лица,...)
    /// </summary>
    [Table("NNumericStatTypeGroups", Schema = "RInfStat")]
    [Index(nameof(Code), nameof(ValidTo), Name = "UK_RInfStat_NNumericStatTypeGroups", IsUnique = true)]
    public partial class NnumericStatTypeGroup
    {
        public NnumericStatTypeGroup()
        {
            MapStatFormTypesNumericStatTypeGroups = new HashSet<MapStatFormTypesNumericStatTypeGroup>();
            NnumericStatTypes = new HashSet<NnumericStatType>();
        }

        /// <summary>
        /// Уникален идентификатор
        /// </summary>
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        /// <summary>
        /// Код на купувач
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
        /// Пореден номер
        /// </summary>
        public short? OrderNum { get; set; }
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

        [InverseProperty(nameof(MapStatFormTypesNumericStatTypeGroup.NumericStatTypeGroup))]
        public virtual ICollection<MapStatFormTypesNumericStatTypeGroup> MapStatFormTypesNumericStatTypeGroups { get; set; }
        [InverseProperty(nameof(NnumericStatType.Group))]
        public virtual ICollection<NnumericStatType> NnumericStatTypes { get; set; }
    }
}