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
    /// Удостоверения при промяна на риболовен капацитет
    /// </summary>
    [Table("CapacityChangeHistoryCertificates", Schema = "RCap")]
    [Index(nameof(CapacityCertificateId), Name = "IXFK_CapacityChngHistCert_CapacityCertificatesRegister")]
    [Index(nameof(CapacityChangeHistoryId), Name = "IXFK_CapacityChngHistCert_CapacityChangeHistory")]
    [Index(nameof(CapacityChangeHistoryId), nameof(CapacityCertificateId), Name = "UK_RCap_CapacityChangeHistoryCertificates", IsUnique = true)]
    public partial class CapacityChangeHistoryCertificate
    {         public CapacityChangeHistoryCertificate()
        {
            this.IsActive = true;
        }


        /// <summary>
        /// Уникален идентификатор
        /// </summary>
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        public int CapacityChangeHistoryId { get; set; }
        public int CapacityCertificateId { get; set; }
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
        /// Флаг дали записът е активен или изтрит
        /// </summary>
        [Required]
        public bool IsActive { get; set; }
        /// <summary>
        /// Потребител последно актуализирал записа
        /// </summary>
        [StringLength(500)]
        public string UpdatedBy { get; set; }
        /// <summary>
        /// Дата и час на последна актуализация на записа
        /// </summary>
        public DateTime? UpdatedOn { get; set; }

        [ForeignKey(nameof(CapacityCertificateId))]
        [InverseProperty(nameof(CapacityCertificatesRegister.CapacityChangeHistoryCertificates))]
        public virtual CapacityCertificatesRegister CapacityCertificate { get; set; }
        [ForeignKey(nameof(CapacityChangeHistoryId))]
        [InverseProperty("CapacityChangeHistoryCertificates")]
        public virtual CapacityChangeHistory CapacityChangeHistory { get; set; }
    }
}
