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
    /// Видове заявления за административни услуги
    /// </summary>
    [Table("NApplicationTypes", Schema = "Appl")]
    [Index(nameof(GroupId), Name = "IXFK_NApplicationTypes_NApplicationTypeGroups")]
    [Index(nameof(PageCode), Name = "IXFK_NApplicationTypes_NPageCodes")]
    [Index(nameof(Code), nameof(ValidTo), Name = "UK_Appl_NApplicationTypes", IsUnique = true)]
    public partial class NapplicationType
    {
        public NapplicationType()
        {
            Applications = new HashSet<Application>();
            MapApplicationTypeDeliveryTypes = new HashSet<MapApplicationTypeDeliveryType>();
            MapApplicationTypeSubmittedByRoles = new HashSet<MapApplicationTypeSubmittedByRole>();
            NapplicationTypeHierTypes = new HashSet<NapplicationTypeHierType>();
            NapplicationTypeTariffs = new HashSet<NapplicationTypeTariff>();
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
        /// Код на форма за обработка на заявления
        /// </summary>
        [StringLength(500)]
        public string PageCode { get; set; }
        /// <summary>
        /// Име
        /// </summary>
        [Required]
        [StringLength(500)]
        public string Name { get; set; }
        /// <summary>
        /// Флаг дали услугата изисква заплащане
        /// </summary>
        public bool IsPaid { get; set; }
        /// <summary>
        /// Флаг дали е Електронна Административна Услуга
        /// </summary>
        [Required]
        [Column("IsEAS")]
        public bool? IsEas { get; set; }
        /// <summary>
        /// Група на заявлението
        /// </summary>
        [Column("GroupID")]
        public int? GroupId { get; set; }
        /// <summary>
        /// Пореден номер
        /// </summary>
        public short? OrderNum { get; set; }
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

        [ForeignKey(nameof(GroupId))]
        [InverseProperty(nameof(NapplicationTypeGroup.NapplicationTypes))]
        public virtual NapplicationTypeGroup Group { get; set; }
        [ForeignKey(nameof(PageCode))]
        [InverseProperty(nameof(NpageCode.NapplicationTypes))]
        public virtual NpageCode PageCodeNavigation { get; set; }
        [InverseProperty(nameof(Application.ApplicationType))]
        public virtual ICollection<Application> Applications { get; set; }
        [InverseProperty(nameof(MapApplicationTypeDeliveryType.ApplicationType))]
        public virtual ICollection<MapApplicationTypeDeliveryType> MapApplicationTypeDeliveryTypes { get; set; }
        [InverseProperty(nameof(MapApplicationTypeSubmittedByRole.ApplicationType))]
        public virtual ICollection<MapApplicationTypeSubmittedByRole> MapApplicationTypeSubmittedByRoles { get; set; }
        [InverseProperty(nameof(NapplicationTypeHierType.ApplicationType))]
        public virtual ICollection<NapplicationTypeHierType> NapplicationTypeHierTypes { get; set; }
        [InverseProperty(nameof(NapplicationTypeTariff.ApplicationType))]
        public virtual ICollection<NapplicationTypeTariff> NapplicationTypeTariffs { get; set; }
    }
}