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
    /// Водни организми към разрешително за стопански риболов
    /// </summary>
    [Table("PermitRegisterFishes", Schema = "RStRib")]
    [Index(nameof(FishId), Name = "IXFK_PermitRegisterFishes_NFishes")]
    [Index(nameof(PermitRegisterId), Name = "IXFK_PermitRegisterFishes_PermitRegister")]
    [Index(nameof(FishId), nameof(PermitRegisterId), Name = "UK_RStR_PermitRegisterFishes", IsUnique = true)]
    public partial class PermitRegisterFish
    {         public PermitRegisterFish()
        {
            this.IsActive = true;
        }


        /// <summary>
        /// Уникален идентификатор
        /// </summary>
        [Key]
        [Column("ID")]
        public int Id { get; set; }
        [Column("PermitRegisterID")]
        public int PermitRegisterId { get; set; }
        [Column("FishID")]
        public int FishId { get; set; }
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

        [ForeignKey(nameof(FishId))]
        [InverseProperty(nameof(Nfish.PermitRegisterFishes))]
        public virtual Nfish Fish { get; set; }
        [ForeignKey(nameof(PermitRegisterId))]
        [InverseProperty("PermitRegisterFishes")]
        public virtual PermitRegister PermitRegister { get; set; }
    }
}
