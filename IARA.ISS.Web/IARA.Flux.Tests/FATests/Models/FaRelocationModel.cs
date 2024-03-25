using System;
using IARA.Flux.Models;

namespace IARA.Flux.Tests.FATests.Models
{
    public class FaRelocationModel
    {
        public DateTime Occurrence { get; set; }

        public FLUXLocationType[] Locations { get; set; }

        public FACatchType[] Catches { get; set; }

        public VesselTransportMeansType RelatedVesselTransportMeans { get; set; }

        public VesselStorageCharacteristicType SourceVesselStorageCharacteristics { get; set; }

        public VesselStorageCharacteristicType DestinationVesselStorageCharacteristics { get; set; }

        public FLAPDocumentType[] FLAPDocuments { get; set; }
    }
}
