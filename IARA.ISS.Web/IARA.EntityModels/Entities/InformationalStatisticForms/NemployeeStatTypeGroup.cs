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
    /// Групи статистическа информация за заети лица (Продължителност на работен ден, Възрасст, Образование...)
    /// </summary>
    [Table("NEmployeeStatTypeGroups", Schema = "RInfStat")]
    [Index(nameof(Code), nameof(ValidTo), Name = "UK_RInfStat_NEmployeeStatTypeGroups", IsUnique = true)]
    public partial class NemployeeStatTypeGroup
    {
        public NemployeeStatTypeGroup()
        {
            MapStatFormTypesEmployeeStatTypeGroups = new HashSet<MapStatFormTypesEmployeeStatTypeGroup>();
            NemployeeStatTypes = new HashSet<NemployeeStatType>();
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

        [InverseProperty(nameof(MapStatFormTypesEmployeeStatTypeGroup.EmployeeStatTypeGroup))]
        public virtual ICollection<MapStatFormTypesEmployeeStatTypeGroup> MapStatFormTypesEmployeeStatTypeGroups { get; set; }
        [InverseProperty(nameof(NemployeeStatType.Group))]
        public virtual ICollection<NemployeeStatType> NemployeeStatTypes { get; set; }
    }
}