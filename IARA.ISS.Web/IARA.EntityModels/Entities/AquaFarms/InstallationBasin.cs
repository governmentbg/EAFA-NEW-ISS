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
    /// Басейни към съоръжение на аквакултурно стопанство
    /// </summary>
    [Table("InstallationBasins", Schema = "RAquaSt")]
    [Index(nameof(BasinMaterialTypeId), Name = "IXFK_InstallationBasins_NInstallBasinMaterialTypes")]
    [Index(nameof(BasinPurposeTypeId), Name = "IXFK_InstallationBasins_NInstallBasinPurposeTypes")]
    public partial class InstallationBasin
    {
        public InstallationBasin()
        {
            AquacultureInstallationBasins = new HashSet<AquacultureInstallationBasin>();
            AquacultureInstallationRecirculatorySystems = new HashSet<AquacultureInstallationRecirculatorySystem>();
            this.IsActive = true;
        }


        /// <summary>
        /// Уникален идентификатор
        /// </summary>
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        /// <summary>
        /// Вид според предназначението
        /// </summary>
        [Column("BasinPurposeTypeID")]
        public int BasinPurposeTypeId { get; set; }
        /// <summary>
        /// Вид според материала
        /// </summary>
        [Column("BasinMaterialTypeID")]
        public int BasinMaterialTypeId { get; set; }
        /// <summary>
        /// Площ (кв. м.)
        /// </summary>
        public decimal Area { get; set; }
        /// <summary>
        /// Обем (куб. м.)
        /// </summary>
        public decimal Volume { get; set; }
        /// <summary>
        /// Брой
        /// </summary>
        public int Count { get; set; }
        [StringLength(500)]
        public string Comments { get; set; }
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

        [ForeignKey(nameof(BasinMaterialTypeId))]
        [InverseProperty(nameof(NinstallationBasinMaterialType.InstallationBasins))]
        public virtual NinstallationBasinMaterialType BasinMaterialType { get; set; }
        [ForeignKey(nameof(BasinPurposeTypeId))]
        [InverseProperty(nameof(NinstallationBasinPurposeType.InstallationBasins))]
        public virtual NinstallationBasinPurposeType BasinPurposeType { get; set; }
        [InverseProperty(nameof(AquacultureInstallationBasin.InstallationBasin))]
        public virtual ICollection<AquacultureInstallationBasin> AquacultureInstallationBasins { get; set; }
        [InverseProperty(nameof(AquacultureInstallationRecirculatorySystem.InstallationBasin))]
        public virtual ICollection<AquacultureInstallationRecirculatorySystem> AquacultureInstallationRecirculatorySystems { get; set; }
    }
}