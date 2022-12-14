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
    /// Видове причини за прекратяване
    /// </summary>
    [Table("NSuspensionReasons", Schema = "RStRib")]
    [Index(nameof(SuspensionTypeId), Name = "IXFK_NSuspensionReasons_NSuspensionTypes")]
    [Index(nameof(Code), nameof(ValidTo), Name = "UK_RStR_NSuspensionReasons", IsUnique = true)]
    public partial class NsuspensionReason
    {
        public NsuspensionReason()
        {
            PermitLicenseSuspensionChangeHistories = new HashSet<PermitLicenseSuspensionChangeHistory>();
            PermitSuspensionChangeHistories = new HashSet<PermitSuspensionChangeHistory>();
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
        [StringLength(50)]
        public string Code { get; set; }
        /// <summary>
        /// Име
        /// </summary>
        [Required]
        [StringLength(500)]
        public string Name { get; set; }
        [Column("SuspensionTypeID")]
        public int SuspensionTypeId { get; set; }
        /// <summary>
        /// Продължителността на прекратяването (отнася се само за разрешителни)
        /// </summary>
        public short? DurationMonths { get; set; }
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

        [ForeignKey(nameof(SuspensionTypeId))]
        [InverseProperty(nameof(NsuspensionType.NsuspensionReasons))]
        public virtual NsuspensionType SuspensionType { get; set; }
        [InverseProperty(nameof(PermitLicenseSuspensionChangeHistory.Reason))]
        public virtual ICollection<PermitLicenseSuspensionChangeHistory> PermitLicenseSuspensionChangeHistories { get; set; }
        [InverseProperty(nameof(PermitSuspensionChangeHistory.Reason))]
        public virtual ICollection<PermitSuspensionChangeHistory> PermitSuspensionChangeHistories { get; set; }
    }
}