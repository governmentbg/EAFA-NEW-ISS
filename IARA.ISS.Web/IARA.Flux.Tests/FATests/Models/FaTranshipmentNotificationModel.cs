using System;
using IARA.Flux.Models;

namespace IARA.Flux.Tests.FATests.Models
{
    public class FaTranshipmentNotificationModel
    {
        public DateTime Occurrence { get; set; }

        public FLUXLocationType[] Locations { get; set; }

        public FACatchType[] Catches { get; set; }

        public VesselTransportMeansType RelatedVesselTransportMeans { get; set; }

        public VesselStorageCharacteristicType DestinationVesselStorageCharacteristics { get; set; }

        public FLUXCharacteristicType[] Characteristics { get; set; }

        public FLAPDocumentType[] FLAPDocuments { get; set; }
    }
}
