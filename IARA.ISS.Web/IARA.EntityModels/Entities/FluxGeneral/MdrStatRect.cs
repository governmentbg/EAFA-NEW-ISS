// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IARA.EntityModels.Entities
{
    [Table("MDR_Stat_Rect", Schema = "FLUX_General")]
    public partial class MdrStatRect
    {
        public MdrStatRect()
        {
            NcatchZones = new HashSet<NcatchZone>();
        }

        [Key]
        [Column("ID")]
        public override int Id { get; set; }
        [StringLength(50)]
        public override string Code { get; set; }
        [StringLength(500)]
        public string EnDescription { get; set; }
        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
        [StringLength(500)]
        public string CreatedBy { get; set; }
        [StringLength(500)]
        public string UpdatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        [Column("ICESNAME")]
        [StringLength(5)]
        public string Icesname { get; set; }
        [Column("SOUTH")]
        public float? South { get; set; }
        [Column("WEST")]
        public float? West { get; set; }
        [Column("NORTH")]
        public float? North { get; set; }
        [Column("EAST")]
        public float? East { get; set; }
        [Column("SOURCE")]
        [StringLength(4)]
        public string Source { get; set; }

        [InverseProperty(nameof(NcatchZone.MdrStatRect))]
        public virtual ICollection<NcatchZone> NcatchZones { get; set; }
    }
}