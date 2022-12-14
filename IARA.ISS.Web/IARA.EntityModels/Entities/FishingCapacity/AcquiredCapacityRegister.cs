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
    /// Осигурен риболовен капацитет
    /// </summary>
    [Table("AcquiredCapacityRegister", Schema = "RCap")]
    public partial class AcquiredCapacityRegister
    {
        public AcquiredCapacityRegister()
        {
            AcquiredCapacityCertificates = new HashSet<AcquiredCapacityCertificate>();
            CapacityChangeHistories = new HashSet<CapacityChangeHistory>();
            this.IsActive = true;
        }


        /// <summary>
        /// Уникален идентификатор
        /// </summary>
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        /// <summary>
        /// Начин на осигуряване на свободния капацитет (Ranking - чрез класиране в процедура по чл. 37; FreeCapLicence - чрез удостоверение за свободен капацитет).
        /// </summary>
        [Required]
        [StringLength(50)]
        public string AcquiredType { get; set; }
        /// <summary>
        /// Осигурен бруто тонаж чрез класиране
        /// </summary>
        public decimal? GrossTonnage { get; set; }
        /// <summary>
        /// Осигурена мощност (kW) чрез класиране
        /// </summary>
        public decimal? EnginePower { get; set; }
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

        [InverseProperty(nameof(AcquiredCapacityCertificate.AcquiredCapacity))]
        public virtual ICollection<AcquiredCapacityCertificate> AcquiredCapacityCertificates { get; set; }
        [InverseProperty(nameof(CapacityChangeHistory.AcquiredFishingCapacity))]
        public virtual ICollection<CapacityChangeHistory> CapacityChangeHistories { get; set; }
    }
}