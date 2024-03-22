using System;
using IARA.Flux.Models;

namespace IARA.Flux.Tests.FATests.Models
{
    public class FaDiscardModel
    {
        public DateTime Occurrence { get; set; }

        public string ReasonCode { get; set; }

        public FLUXLocationType[] Locations { get; set; }

        public FACatchType[] Catches { get; set; }

        public FLUXCharacteristicType[] Characteristics { get; set; }
    }
}
