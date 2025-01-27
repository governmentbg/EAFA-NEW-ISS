﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IARA.EntityModels.Entities
{
    /// <summary>
    /// Източници на данни за кръстосана проверка
    /// </summary>
    [Table("NCheckSources", Schema = "Checks")]
    [Index("ValidTo", "Code", Name = "UK_Check_NCheckSources", IsUnique = true)]
    public partial class NcheckSource
    {
        public NcheckSource()
        {
            CrossChecks = new HashSet<CrossCheck>();
        }

        /// <summary>
        /// Уникален идентификатор
        /// </summary>
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        /// <summary>
        /// Код
        /// </summary>
        [Required]
        [StringLength(50)]
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
        [Column(TypeName = "timestamp without time zone")]
        public DateTime ValidFrom { get; set; }
        /// <summary>
        /// Крайна дата на валидност на записа
        /// </summary>
        [Column(TypeName = "timestamp without time zone")]
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
        [Column(TypeName = "timestamp without time zone")]
        public DateTime CreatedOn { get; set; }
        /// <summary>
        /// Потребител последно актуализирал записа
        /// </summary>
        [StringLength(500)]
        public string UpdatedBy { get; set; }
        /// <summary>
        /// Дата и час на последна актуализация на записа
        /// </summary>
        [Column(TypeName = "timestamp without time zone")]
        public DateTime? UpdatedOn { get; set; }

        [InverseProperty("CheckSourceNavigation")]
        public virtual ICollection<CrossCheck> CrossChecks { get; set; }
    }
}