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
    /// Проверени страници от дневник по време на инспекция
    /// </summary>
    [Table("InspectionLogBookPages", Schema = "RInsp")]
    [Index(nameof(AdmissionLogBookPageId), Name = "IXFK_InspectionLogBookPages_AdmissionLogBookPages")]
    [Index(nameof(FirstSaleLogBookPageId), Name = "IXFK_InspectionLogBookPages_FirstSaleLogBookPages")]
    [Index(nameof(InspectionId), Name = "IXFK_InspectionLogBookPages_InspectionRegister")]
    [Index(nameof(LogBookId), Name = "IXFK_InspectionLogBookPages_LogBooks")]
    [Index(nameof(ShipLogBookPageId), Name = "IXFK_InspectionLogBookPages_ShipLogBookPages")]
    [Index(nameof(ShipId), Name = "IXFK_InspectionLogBookPages_ShipRegister")]
    [Index(nameof(TransportationLogBookPageId), Name = "IXFK_InspectionLogBookPages_TransportationLogBookPages")]
    [Index(nameof(UnregisteredShipId), Name = "IXFK_InspectionLogBookPages_UnregisteredVessels")]
    public partial class InspectionLogBookPage
    {
        public InspectionLogBookPage()
        {
            InspectionCatchMeasures = new HashSet<InspectionCatchMeasure>();
            this.IsActive = true;
        }


        /// <summary>
        /// Уникален идентификатор
        /// </summary>
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        /// <summary>
        /// Вид на иснпектирания кораб (Inspected / TransboardSender / TransbroardReceiver )
        /// </summary>
        [Required]
        [StringLength(20)]
        public string InspectedShipType { get; set; }
        /// <summary>
        /// Инспекция
        /// </summary>
        [Column("InspectionID")]
        public int InspectionId { get; set; }
        /// <summary>
        /// Стойност на проверка според CheckType ( Y / N / X / null )
        /// </summary>
        [Required]
        [StringLength(1)]
        public string CheckLogBookMatches { get; set; }
        /// <summary>
        /// Дневник, за който е проверката
        /// </summary>
        [Column("LogBookID")]
        public int? LogBookId { get; set; }
        /// <summary>
        /// Тип на дневник или тип фактура (и в случай на регистриран и на нерегистриран дневник)
        /// </summary>
        [StringLength(20)]
        public string LogBookType { get; set; }
        /// <summary>
        /// Кораб, за който е страницата
        /// </summary>
        [Column("ShipID")]
        public int? ShipId { get; set; }
        /// <summary>
        /// Нерегистриран кораб
        /// </summary>
        [Column("UnregisteredShipID")]
        public int? UnregisteredShipId { get; set; }
        /// <summary>
        /// Декларация за произход, за която е проверката
        /// </summary>
        [Column("ShipLogBookPageID")]
        public int? ShipLogBookPageId { get; set; }
        /// <summary>
        /// Декларация за приемане
        /// </summary>
        [Column("AdmissionLogBookPageID")]
        public int? AdmissionLogBookPageId { get; set; }
        /// <summary>
        /// Декларация за транспорт/превоз
        /// </summary>
        [Column("TransportationLogBookPageID")]
        public int? TransportationLogBookPageId { get; set; }
        /// <summary>
        /// Декларация за първа продажба
        /// </summary>
        [Column("FirstSaleLogBookPageID")]
        public int? FirstSaleLogBookPageId { get; set; }
        /// <summary>
        /// Идентификатор на нов дневник, който не съществува в регистъра и е добавен по време на проверката
        /// </summary>
        [StringLength(50)]
        public string UnregisteredLogBookNum { get; set; }
        /// <summary>
        /// Номер на нерегистрирана страница от дневник
        /// </summary>
        [StringLength(50)]
        public string UnregisteredPageNum { get; set; }
        /// <summary>
        /// Дата на нерегистрирана страница
        /// </summary>
        [Column(TypeName = "date")]
        public DateTime? UnregisteredPageDate { get; set; }
        /// <summary>
        /// Детайли, въведени от инспектор
        /// </summary>
        [StringLength(200)]
        public string Description { get; set; }
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

        [ForeignKey(nameof(AdmissionLogBookPageId))]
        [InverseProperty("InspectionLogBookPages")]
        public virtual AdmissionLogBookPage AdmissionLogBookPage { get; set; }
        [ForeignKey(nameof(FirstSaleLogBookPageId))]
        [InverseProperty("InspectionLogBookPages")]
        public virtual FirstSaleLogBookPage FirstSaleLogBookPage { get; set; }
        [ForeignKey(nameof(InspectionId))]
        [InverseProperty(nameof(InspectionRegister.InspectionLogBookPages))]
        public virtual InspectionRegister Inspection { get; set; }
        [ForeignKey(nameof(LogBookId))]
        [InverseProperty("InspectionLogBookPages")]
        public virtual LogBook LogBook { get; set; }
        [ForeignKey(nameof(ShipId))]
        [InverseProperty(nameof(ShipRegister.InspectionLogBookPages))]
        public virtual ShipRegister Ship { get; set; }
        [ForeignKey(nameof(ShipLogBookPageId))]
        [InverseProperty("InspectionLogBookPages")]
        public virtual ShipLogBookPage ShipLogBookPage { get; set; }
        [ForeignKey(nameof(TransportationLogBookPageId))]
        [InverseProperty("InspectionLogBookPages")]
        public virtual TransportationLogBookPage TransportationLogBookPage { get; set; }
        [ForeignKey(nameof(UnregisteredShipId))]
        [InverseProperty(nameof(UnregisteredVessel.InspectionLogBookPages))]
        public virtual UnregisteredVessel UnregisteredShip { get; set; }
        [InverseProperty(nameof(InspectionCatchMeasure.InspectedLogBookPage))]
        public virtual ICollection<InspectionCatchMeasure> InspectionCatchMeasures { get; set; }
    }
}