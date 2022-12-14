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
    /// Потребителски роли, които могат да изпълняват даден репорт
    /// </summary>
    [Table("ReportRolePermissions", Schema = "Rep")]
    [Index(nameof(ReportId), Name = "IXFK_ReportRolePermissions_Reports")]
    [Index(nameof(RoleId), Name = "IXFK_ReportRolePermissions_Roles")]
    [Index(nameof(ReportId), nameof(RoleId), Name = "UK_Rep_ReportRolesPermissions", IsUnique = true)]
    public partial class ReportRolePermission
    {         public ReportRolePermission()
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
        /// Отчет
        /// </summary>
        [Column("ReportID")]
        public int ReportId { get; set; }
        /// <summary>
        /// Параметър
        /// </summary>
        [Column("RoleID")]
        public int RoleId { get; set; }
        /// <summary>
        /// Флаг дали записът е активен или изтрит (май е излишно тук)
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

        [ForeignKey(nameof(ReportId))]
        [InverseProperty("ReportRolePermissions")]
        public virtual Report Report { get; set; }
        [ForeignKey(nameof(RoleId))]
        [InverseProperty("ReportRolePermissions")]
        public virtual Role Role { get; set; }
    }
}
