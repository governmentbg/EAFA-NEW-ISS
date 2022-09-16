using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:FLUXVesselResponseMessage:5")]
    [XmlRoot("FLUXVesselResponseMessage", Namespace = "urn:un:unece:uncefact:data:standard:FLUXVesselResponseMessage:5", IsNullable = false)]
    public partial class FLUXVesselResponseMessageType
    {
        public FLUXResponseDocumentType FLUXResponseDocument { get; set; }


        [XmlElement("EventVesselEvent")]
        public VesselEventType[] EventVesselEvent { get; set; }
    }
}
