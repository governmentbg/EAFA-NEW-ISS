// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace IARA.EntityModels.Entities
{
    /// <summary>
    /// Координати на далян
    /// </summary>
    [Table("PoundNetCoordinates", Schema = "RDal")]
    [Index(nameof(PoundNetId), Name = "IXFK_PoundNetCoordinates_PoundNetRegister")]
    [Index(nameof(PoundNetId), nameof(PointNum), Name = "UK_PoundNetCoordinates", IsUnique = true)]
    public partial class PoundNetCoordinate
    {         public PoundNetCoordinate()
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
        /// Далян
        /// </summary>
        [Column("PoundNetID")]
        public int PoundNetId { get; set; }
        /// <summary>
        /// Пореден номер на точката
        /// </summary>
        public short PointNum { get; set; }
        /// <summary>
        /// Координати на точката (WGS84)
        /// </summary>
        [Required]
        [Column(TypeName = "geometry(Point)")]
        public Point Coordinates { get; set; }
        /// <summary>
        /// Флаг дали е свързваща точка
        /// </summary>
        public bool IsConnectPoint { get; set; }
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

        [ForeignKey(nameof(PoundNetId))]
        [InverseProperty(nameof(PoundNetRegister.PoundNetCoordinates))]
        public virtual PoundNetRegister PoundNet { get; set; }
    }
}
