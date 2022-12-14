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
    /// Видове инспекции и проверки
    /// </summary>
    [Table("NInspectionCheckTypes", Schema = "RInsp")]
    [Index(nameof(InspectionTypeId), Name = "IXFK_NInspectionCheckTypes_NInspectionTypes")]
    [Index(nameof(Code), nameof(InspectionTypeId), nameof(ValidTo), Name = "UK_RInsp_NInspectionCheckTypes", IsUnique = true)]
    public partial class NinspectionCheckType
    {
        public NinspectionCheckType()
        {
            InspectionChecks = new HashSet<InspectionCheck>();
        }

        /// <summary>
        /// Уникален идентификатор
        /// </summary>
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        /// <summary>
        /// Тип инспекция
        /// </summary>
        [Column("InspectionTypeID")]
        public int InspectionTypeId { get; set; }
        /// <summary>
        /// Код 
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Code { get; set; }
        /// <summary>
        /// Име
        /// </summary>
        [Required]
        [StringLength(500)]
        public string Name { get; set; }
        /// <summary>
        /// Тип стойности (bool / triple)
        /// </summary>
        [Required]
        [StringLength(50)]
        public string CheckType { get; set; }
        /// <summary>
        /// Флаг дали е задължително да се попълни
        /// </summary>
        public bool IsMandatory { get; set; }
        /// <summary>
        /// Флаг дали срещу тази отметка да има и textbox за допълнителни детайли
        /// </summary>
        public bool HasDescription { get; set; }
        /// <summary>
        /// Етикет с описание какво се попълва в полето Description
        /// </summary>
        [StringLength(500)]
        public string DescriptionLabel { get; set; }
        /// <summary>
        /// Начална дата на валидност на записа
        /// </summary>
        public DateTime ValidFrom { get; set; }
        /// <summary>
        /// Крайна дата на валидност на записа
        /// </summary>
        public DateTime ValidTo { get; set; }
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

        [ForeignKey(nameof(InspectionTypeId))]
        [InverseProperty(nameof(NinspectionType.NinspectionCheckTypes))]
        public virtual NinspectionType InspectionType { get; set; }
        [InverseProperty(nameof(InspectionCheck.CheckType))]
        public virtual ICollection<InspectionCheck> InspectionChecks { get; set; }
    }
}