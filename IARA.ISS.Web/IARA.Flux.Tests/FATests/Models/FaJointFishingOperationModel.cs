using System;
using IARA.Flux.Models;

namespace IARA.Flux.Tests.FATests.Models
{
    public class FaJointFishingOperationModel
    {
        public DateTime Occurrence { get; set; }

        public string VesselRelatedActivityCode { get; set; }

        public string FisheryTypeCode { get; set; }

        public string SpeciesTargetCode { get; set; }

        public FLUXLocationType[] Locations { get; set; }

        public FACatchType[] Catches { get; set; }

        public VesselTransportMeansType[] RelatedVesselTransportMeans { get; set; }

        public FishingGearType FishingGear { get; set; }

        public GearProblemType[] GearProblems { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public FLUXCharacteristicType[] Characteristics { get; set; }
    }
}
