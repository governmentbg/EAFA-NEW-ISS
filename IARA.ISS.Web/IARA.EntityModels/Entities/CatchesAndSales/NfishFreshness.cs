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
    /// Състояние на улов
    /// </summary>
    [Table("NFishFreshness", Schema = "CatchSales")]
    [Index(nameof(MdrFishFreshnessId), Name = "IXFK_NFishFreshness_MDR_Fish_Freshness")]
    [Index(nameof(Code), nameof(ValidTo), Name = "UK_CatchSales_NCatchFishStates", IsUnique = true)]
    public partial class NfishFreshness
    {
        public NfishFreshness()
        {
            LogBookPageProducts = new HashSet<LogBookPageProduct>();
            OriginDeclarationFishes = new HashSet<OriginDeclarationFish>();
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
        [Column("MDR_Fish_Freshness_ID")]
        public int? MdrFishFreshnessId { get; set; }

        [ForeignKey(nameof(MdrFishFreshnessId))]
        [InverseProperty("NfishFreshnesses")]
        public virtual MdrFishFreshness MdrFishFreshness { get; set; }
        [InverseProperty(nameof(LogBookPageProduct.ProductFreshness))]
        public virtual ICollection<LogBookPageProduct> LogBookPageProducts { get; set; }
        [InverseProperty(nameof(OriginDeclarationFish.CatchFishFreshness))]
        public virtual ICollection<OriginDeclarationFish> OriginDeclarationFishes { get; set; }
    }
}