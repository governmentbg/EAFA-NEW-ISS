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
    /// Видове файлове за прикачване
    /// </summary>
    [Table("NPageCodes", Schema = "Admin")]
    public partial class NpageCode
    {
        public NpageCode()
        {
            NapplicationTypes = new HashSet<NapplicationType>();
            NchangeOfCircumstancesTypes = new HashSet<NchangeOfCircumstancesType>();
            NmobileVersions = new HashSet<NmobileVersion>();
            NrequiredFileTypes = new HashSet<NrequiredFileType>();
        }

        /// <summary>
        /// Код
        /// </summary>
        [Key]
        [StringLength(500)]
        public string Code { get; set; }
        /// <summary>
        /// Име
        /// </summary>
        [Required]
        [StringLength(500)]
        public string Name { get; set; }
        /// <summary>
        /// Описание
        /// </summary>
        [StringLength(4000)]
        public string Description { get; set; }
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

        [InverseProperty(nameof(NapplicationType.PageCodeNavigation))]
        public virtual ICollection<NapplicationType> NapplicationTypes { get; set; }
        [InverseProperty(nameof(NchangeOfCircumstancesType.PageCodeNavigation))]
        public virtual ICollection<NchangeOfCircumstancesType> NchangeOfCircumstancesTypes { get; set; }
        [InverseProperty(nameof(NmobileVersion.PageCodeNavigation))]
        public virtual ICollection<NmobileVersion> NmobileVersions { get; set; }
        [InverseProperty(nameof(NrequiredFileType.PageCodeNavigation))]
        public virtual ICollection<NrequiredFileType> NrequiredFileTypes { get; set; }
    }
}