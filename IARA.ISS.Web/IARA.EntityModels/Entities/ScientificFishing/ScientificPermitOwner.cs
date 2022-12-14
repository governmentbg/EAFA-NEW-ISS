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
    /// Титуляри на разрешителни за научен риболов
    /// </summary>
    [Table("ScientificPermitOwners", Schema = "RNauR")]
    [Index(nameof(OwnerId), Name = "IXFK_ScientificPermitOwners_Persons_Hist")]
    [Index(nameof(ScientificPermitId), Name = "IXFK_ScientificPermitOwners_ScientificPermitRegister")]
    [Index(nameof(ScientificPermitId), nameof(OwnerId), Name = "UK_ScientificPermitOwners", IsUnique = true)]
    public partial class ScientificPermitOwner
    {         public ScientificPermitOwner()
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
        /// Резрешително за научен риболов
        /// </summary>
        [Column("ScientificPermitID")]
        public int ScientificPermitId { get; set; }
        /// <summary>
        /// Титуляр
        /// </summary>
        [Column("OwnerID")]
        public int OwnerId { get; set; }
        /// <summary>
        /// Длъжност в организацията
        /// </summary>
        [StringLength(500)]
        public string RequestedByOrganizationPosition { get; set; }
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

        [ForeignKey(nameof(OwnerId))]
        [InverseProperty(nameof(Person.ScientificPermitOwners))]
        public virtual Person Owner { get; set; }
        [ForeignKey(nameof(ScientificPermitId))]
        [InverseProperty(nameof(ScientificPermitRegister.ScientificPermitOwners))]
        public virtual ScientificPermitRegister ScientificPermit { get; set; }
    }
}
