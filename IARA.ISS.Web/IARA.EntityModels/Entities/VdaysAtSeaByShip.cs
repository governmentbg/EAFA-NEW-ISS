﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace IARA.EntityModels.Entities
{
    [Keyless]
    public partial class VdaysAtSeaByShip
    {
        [Column("ShipUID")]
        public int? ShipUid { get; set; }
        public double? Year { get; set; }
        public double? DaysAtSea { get; set; }
    }
}