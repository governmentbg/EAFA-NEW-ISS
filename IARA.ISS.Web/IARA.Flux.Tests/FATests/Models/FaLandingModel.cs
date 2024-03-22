using System;
using IARA.Flux.Models;

namespace IARA.Flux.Tests.FATests.Models
{
    public class FaLandingModel
    {
        public DateTime DateFrom { get; set; }

        public DateTime DateTo { get; set; }

        public FLUXLocationType[] Locations { get; set; }

        public FACatchType[] Catches { get; set; }

        public FLUXCharacteristicType[] Characteristics { get; set; }
    }
}
