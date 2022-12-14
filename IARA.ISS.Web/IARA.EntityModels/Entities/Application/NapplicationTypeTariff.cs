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
    /// Такси за заплащане на видове заявления
    /// </summary>
    [Table("NApplicationTypeTariff", Schema = "Appl")]
    [Index(nameof(ApplicationTypeId), Name = "IXFK_NApplicationТypeTariff_NApplicationTypes")]
    [Index(nameof(TariffId), Name = "IXFK_NApplicationТypeTariff_NTariff")]
    [Index(nameof(TariffId), nameof(ApplicationTypeId), nameof(ValidTo), Name = "UK_Appl_NApplicationТypeTariff", IsUnique = true)]
    public partial class NapplicationTypeTariff
    {
        /// <summary>
        /// Уникален идентификатор
        /// </summary>
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        /// <summary>
        /// Цена от тарифа
        /// </summary>
        [Column("TariffID")]
        public int TariffId { get; set; }
        /// <summary>
        /// Заявление
        /// </summary>
        [Column("ApplicationTypeID")]
        public int ApplicationTypeId { get; set; }
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

        [ForeignKey(nameof(ApplicationTypeId))]
        [InverseProperty(nameof(NapplicationType.NapplicationTypeTariffs))]
        public virtual NapplicationType ApplicationType { get; set; }
        [ForeignKey(nameof(TariffId))]
        [InverseProperty(nameof(Ntariff.NapplicationTypeTariffs))]
        public virtual Ntariff Tariff { get; set; }
    }
}