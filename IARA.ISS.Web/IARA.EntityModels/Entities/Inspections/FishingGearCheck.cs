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
    /// Проверка на средства за улов (Първоначално маркиране / Повторно маркиране / Инспекция)
    /// </summary>
    [Table("FishingGearChecks", Schema = "RInsp")]
    [Index(nameof(CheckReasonId), Name = "IXFK_FishingGearChecks_NFishingGearCheckReasons")]
    [Index(nameof(RecheckReasonId), Name = "IXFK_FishingGearChecks_NFishingGearRecheckReasons")]
    [Index(nameof(PoundNetId), Name = "IXFK_FishingGearChecks_PoundNetRegister")]
    [Index(nameof(ShipId), Name = "IXFK_FishingGearChecks_ShipRegister")]
    [Index(nameof(UnregisteredShipId), Name = "IXFK_FishingGearChecks_UnregisteredVessels")]
    public partial class FishingGearCheck
    {         public FishingGearCheck()
        {
            this.IsActive = true;
        }


        /// <summary>
        /// Инспекция
        /// </summary>
        [Key]
        [Column("InspectionID")]
        public int InspectionId { get; set; }
        /// <summary>
        /// Вид проверявани уреди (Ship=Уреди на кораб / PoundNet=Уреди на далян)
        /// </summary>
        [StringLength(20)]
        public string FishingToolType { get; set; }
        [Column("ShipID")]
        public int? ShipId { get; set; }
        [Column("UnregisteredShipID")]
        public int? UnregisteredShipId { get; set; }
        [Column("PoundNetID")]
        public int? PoundNetId { get; set; }
        /// <summary>
        /// Причини за маркиране на уреди
        /// </summary>
        [Column("CheckReasonID")]
        public int? CheckReasonId { get; set; }
        /// <summary>
        /// Причина за повторно маркиране на уредите
        /// </summary>
        [Column("RecheckReasonID")]
        public int? RecheckReasonId { get; set; }
        /// <summary>
        /// Причина за повторно маркиране на уредите - Друго в свободен текст
        /// </summary>
        [StringLength(500)]
        public string RecheckDescription { get; set; }
        /// <summary>
        /// Флаг дали записът е активен или изтрит
        /// </summary>
        [Required]
        public bool IsActive { get; set; }
        /// <summary>
        /// Дата и час на създаване на записа
        /// </summary>
        public DateTime CreatedOn { get; set; }
        /// <summary>
        /// Потребител създал записа
        /// </summary>
        [Required]
        [StringLength(500)]
        public string CreatedBy { get; set; }
        /// <summary>
        /// Потребител последно актуализирал записа
        /// </summary>
        [StringLength(500)]
        public string UpdatedBy { get; set; }
        /// <summary>
        /// Дата и час на последна актуализация на записа
        /// </summary>
        public DateTime? UpdatedOn { get; set; }

        [ForeignKey(nameof(CheckReasonId))]
        [InverseProperty(nameof(NfishingGearCheckReason.FishingGearChecks))]
        public virtual NfishingGearCheckReason CheckReason { get; set; }
        [ForeignKey(nameof(InspectionId))]
        [InverseProperty(nameof(InspectionRegister.FishingGearCheck))]
        public virtual InspectionRegister Inspection { get; set; }
        [ForeignKey(nameof(PoundNetId))]
        [InverseProperty(nameof(PoundNetRegister.FishingGearChecks))]
        public virtual PoundNetRegister PoundNet { get; set; }
        [ForeignKey(nameof(RecheckReasonId))]
        [InverseProperty(nameof(NfishingGearRecheckReason.FishingGearChecks))]
        public virtual NfishingGearRecheckReason RecheckReason { get; set; }
        [ForeignKey(nameof(ShipId))]
        [InverseProperty(nameof(ShipRegister.FishingGearChecks))]
        public virtual ShipRegister Ship { get; set; }
        [ForeignKey(nameof(UnregisteredShipId))]
        [InverseProperty(nameof(UnregisteredVessel.FishingGearChecks))]
        public virtual UnregisteredVessel UnregisteredShip { get; set; }
    }
}
