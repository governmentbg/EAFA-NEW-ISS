using System;
using IARA.Flux.Models;

namespace IARA.Flux.Tests.FATests.Models
{
    public class FaAreaExitModel
    {
        public DateTime Occurrence { get; set; }

        public string FisheryTypeCode { get; set; }

        public string SpeciesTargetCode { get; set; }

        public FLUXLocationType[] Locations { get; set; }

        public FACatchType[] Catches { get; set; }
    }
}
