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
    /// Сезонност на далян
    /// </summary>
    [Table("NPoundNetSeasonType", Schema = "RDal")]
    [Index(nameof(Name), nameof(ValidTo), Name = "UK_NPoundNetSeasonType", IsUnique = true)]
    public partial class NpoundNetSeasonType
    {
        public NpoundNetSeasonType()
        {
            PoundNetRegisters = new HashSet<PoundNetRegister>();
        }

        /// <summary>
        /// Уникален идентификатор
        /// </summary>
        [Key]
        [Column("ID")]
        public int Id { get; set; }
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
        /// Потребител последно актуализирал записа
        /// </summary>
        [StringLength(500)]
        public string UpdatedBy { get; set; }
        /// <summary>
        /// Дата и час на създаване на записа
        /// </summary>
        public DateTime CreatedOn { get; set; }
        /// <summary>
        /// Дата и час на последна актуализация на записа
        /// </summary>
        public DateTime? UpdatedOn { get; set; }

        [InverseProperty(nameof(PoundNetRegister.SeasonType))]
        public virtual ICollection<PoundNetRegister> PoundNetRegisters { get; set; }
    }
}