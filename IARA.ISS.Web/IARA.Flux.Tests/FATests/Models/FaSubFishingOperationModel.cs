using System;
using IARA.Flux.Models;

namespace IARA.Flux.Tests.FATests.Models
{
    public class FaSubFishingOperationModel
    {
        public string TypeCode { get; set; }

        public DateTime Occurrence { get; set; }

        public FLUXLocationType Location { get; set; }

        public FishingGearType FishingGear { get; set; }

        public GearProblemType[] GearProblems { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public FLUXCharacteristicType[] Characteristics { get; set; }
    }
}
