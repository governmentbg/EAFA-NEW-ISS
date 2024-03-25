using System;
using IARA.Flux.Models;

namespace IARA.Flux.Tests.FATests.Models
{
    public class FaTranshipmentModel
    {
        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public FLUXLocationType[] Locations { get; set; }

        public FACatchType[] Catches { get; set; }

        public VesselTransportMeansType RelatedVesselTransportMeans { get; set; }

        public FLAPDocumentType[] FLAPDocuments { get; set; }

        public FLUXCharacteristicType[] Characteristics { get; set; }
    }
}
