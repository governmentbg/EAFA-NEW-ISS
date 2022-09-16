using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:FLUXVesselPositionMessage:4")]
    [XmlRoot("FLUXVesselPositionMessage", Namespace = "urn:un:unece:uncefact:data:standard:FLUXVesselPositionMessage:4", IsNullable = false)]
    public partial class FLUXVesselPositionMessageType
    {
        public FLUXReportDocumentType FLUXReportDocument { get; set; }

        public VesselTransportMeansType VesselTransportMeans { get; set; }
    }
}
