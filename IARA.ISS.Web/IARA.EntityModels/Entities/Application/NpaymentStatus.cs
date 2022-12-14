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
    /// Статуси на плащания
    /// </summary>
    [Table("NPaymentStatuses", Schema = "Appl")]
    [Index(nameof(Code), nameof(ValidTo), Name = "UK_Appl_NPaymentStatuses", IsUnique = true)]
    public partial class NpaymentStatus
    {
        public NpaymentStatus()
        {
            ApplicationChangeHistories = new HashSet<ApplicationChangeHistory>();
            ApplicationPayments = new HashSet<ApplicationPayment>();
            Applications = new HashSet<Application>();
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
        public DateTime ValidFrom { get; set; }
        /// <summary>
        /// Крайна дата на валидност на записа
        /// </summary>
        public DateTime ValidTo { get; set; }
        /// <summary>
        /// Пореден номер
        /// </summary>
        public short? OrderNum { get; set; }
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

        [InverseProperty(nameof(ApplicationChangeHistory.PaymentStatus))]
        public virtual ICollection<ApplicationChangeHistory> ApplicationChangeHistories { get; set; }
        [InverseProperty(nameof(ApplicationPayment.PaymentStatus))]
        public virtual ICollection<ApplicationPayment> ApplicationPayments { get; set; }
        [InverseProperty(nameof(Application.PaymentStatus))]
        public virtual ICollection<Application> Applications { get; set; }
    }
}