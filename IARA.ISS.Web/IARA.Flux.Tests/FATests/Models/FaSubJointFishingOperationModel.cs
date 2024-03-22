using IARA.Flux.Models;

namespace IARA.Flux.Tests.FATests.Models
{
    public class FaSubJointFishingOperationModel
    {
        public VesselTransportMeansType RelatedVesselTransportMeans { get; set; }

        public FACatchType[] Catches { get; set; }

        public FLUXCharacteristicType[] Characteristics { get; set; }
    }
}
