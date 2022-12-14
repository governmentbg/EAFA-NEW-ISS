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
    /// Марки за риболовен уред
    /// </summary>
    [Table("FishingGearMarks", Schema = "RStRib")]
    [Index(nameof(FishingGearId), Name = "IXFK_FishingGearMarks_FishingGearRegister")]
    [Index(nameof(InspectionId), Name = "IXFK_FishingGearMarks_InspectionRegister")]
    [Index(nameof(MarkStatusId), Name = "IXFK_FishingGearMarks_NFishingGearMarkStatuses")]
    [Index(nameof(PermitLicenseId), Name = "IXFK_FishingGearMarks_PermitLicensesRegister")]
    [Index(nameof(MarkNum), nameof(InspectionId), nameof(PermitLicenseId), Name = "UK_RStR_FishingGearMarks", IsUnique = true)]
    public partial class FishingGearMark
    {         public FishingGearMark()
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
        /// Риболовен уред
        /// </summary>
        [Column("FishingGearID")]
        public int FishingGearId { get; set; }
        /// <summary>
        /// Стаус на маркa за риболовен уред
        /// </summary>
        [Column("MarkStatusID")]
        public int MarkStatusId { get; set; }
        /// <summary>
        /// Удостоверение, към което е свързан уредът
        /// </summary>
        [Column("PermitLicenseID")]
        public int? PermitLicenseId { get; set; }
        /// <summary>
        /// Номер на марка
        /// </summary>
        [Required]
        [StringLength(4000)]
        public string MarkNum { get; set; }
        /// <summary>
        /// Номер на инспекция ако марките са към уред за инспекция, null ако е регистриран уред (това го ползваме за да гарантираме уникалност по MarkNum за регистрирани уреди)
        /// </summary>
        [Column("InspectionID")]
        public int? InspectionId { get; set; }
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

        [ForeignKey(nameof(FishingGearId))]
        [InverseProperty(nameof(FishingGearRegister.FishingGearMarks))]
        public virtual FishingGearRegister FishingGear { get; set; }
        [ForeignKey(nameof(InspectionId))]
        [InverseProperty(nameof(InspectionRegister.FishingGearMarks))]
        public virtual InspectionRegister Inspection { get; set; }
        [ForeignKey(nameof(MarkStatusId))]
        [InverseProperty(nameof(NfishingGearMarkStatus.FishingGearMarks))]
        public virtual NfishingGearMarkStatus MarkStatus { get; set; }
        [ForeignKey(nameof(PermitLicenseId))]
        [InverseProperty(nameof(PermitLicensesRegister.FishingGearMarks))]
        public virtual PermitLicensesRegister PermitLicense { get; set; }
    }
}
