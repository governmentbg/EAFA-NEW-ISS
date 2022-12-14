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
    /// Видове съоръжения към аквакултурно стопанство
    /// </summary>
    [Table("NAquacultureInstallationTypes", Schema = "RAquaSt")]
    [Index(nameof(ValidTo), nameof(Code), Name = "UK_RAquaSt_NAquacultureInstallationTypes", IsUnique = true)]
    public partial class NaquacultureInstallationType
    {
        public NaquacultureInstallationType()
        {
            AquacultureFacilityInstallations = new HashSet<AquacultureFacilityInstallation>();
            AquacultureFormFullSystemInstallations = new HashSet<AquacultureFormFullSystemInstallation>();
            AquacultureFormNonFullSystemInstallations = new HashSet<AquacultureFormNonFullSystemInstallation>();
            AquacutlureFormBroodstocks = new HashSet<AquacutlureFormBroodstock>();
            AquacutlureFormStockingMaterials = new HashSet<AquacutlureFormStockingMaterial>();
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

        [InverseProperty(nameof(AquacultureFacilityInstallation.InstallationType))]
        public virtual ICollection<AquacultureFacilityInstallation> AquacultureFacilityInstallations { get; set; }
        [InverseProperty(nameof(AquacultureFormFullSystemInstallation.InstallationType))]
        public virtual ICollection<AquacultureFormFullSystemInstallation> AquacultureFormFullSystemInstallations { get; set; }
        [InverseProperty(nameof(AquacultureFormNonFullSystemInstallation.InstallationType))]
        public virtual ICollection<AquacultureFormNonFullSystemInstallation> AquacultureFormNonFullSystemInstallations { get; set; }
        [InverseProperty(nameof(AquacutlureFormBroodstock.InstallationType))]
        public virtual ICollection<AquacutlureFormBroodstock> AquacutlureFormBroodstocks { get; set; }
        [InverseProperty(nameof(AquacutlureFormStockingMaterial.InstallationType))]
        public virtual ICollection<AquacutlureFormStockingMaterial> AquacutlureFormStockingMaterials { get; set; }
    }
}