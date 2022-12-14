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
    /// Иззети риболовни уреди към наказателно постановление
    /// </summary>
    [Table("PenalDecreeSeizedFishingGear", Schema = "RInsp")]
    [Index(nameof(ConfiscationActionId), Name = "IXFK_PenalDecreeSeizedFishingGear_NConfiscationActions")]
    [Index(nameof(FishingGearId), Name = "IXFK_PenalDecreeSeizedFishingGear_NFishingGears")]
    [Index(nameof(TerritoryUnitId), Name = "IXFK_PenalDecreeSeizedFishingGear_NTerritoryUnits")]
    [Index(nameof(PenalDecreeId), Name = "IXFK_PenalDecreeSeizedFishingGear_PenalDecreesRegister")]
    public partial class PenalDecreeSeizedFishingGear
    {         public PenalDecreeSeizedFishingGear()
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
        /// Наказателно постановление
        /// </summary>
        [Column("PenalDecreeID")]
        public int PenalDecreeId { get; set; }
        /// <summary>
        /// Риболовен уред
        /// </summary>
        [Column("FishingGearID")]
        public int FishingGearId { get; set; }
        /// <summary>
        /// Брой
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// Предприето действие
        /// </summary>
        [Column("ConfiscationActionID")]
        public int ConfiscationActionId { get; set; }
        /// <summary>
        /// ТЗ за съхранение
        /// </summary>
        [Column("TerritoryUnitID")]
        public int? TerritoryUnitId { get; set; }
        /// <summary>
        /// Бележки
        /// </summary>
        [StringLength(2000)]
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

        [ForeignKey(nameof(ConfiscationActionId))]
        [InverseProperty(nameof(NconfiscationAction.PenalDecreeSeizedFishingGears))]
        public virtual NconfiscationAction ConfiscationAction { get; set; }
        [ForeignKey(nameof(FishingGearId))]
        [InverseProperty(nameof(NfishingGear.PenalDecreeSeizedFishingGears))]
        public virtual NfishingGear FishingGear { get; set; }
        [ForeignKey(nameof(PenalDecreeId))]
        [InverseProperty(nameof(PenalDecreesRegister.PenalDecreeSeizedFishingGears))]
        public virtual PenalDecreesRegister PenalDecree { get; set; }
        [ForeignKey(nameof(TerritoryUnitId))]
        [InverseProperty(nameof(NterritoryUnit.PenalDecreeSeizedFishingGears))]
        public virtual NterritoryUnit TerritoryUnit { get; set; }
    }
}
