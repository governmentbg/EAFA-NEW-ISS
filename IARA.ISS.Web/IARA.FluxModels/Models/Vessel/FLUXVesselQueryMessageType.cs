using System;
using System.Xml.Serialization;

namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:FLUXVesselQueryMessage:5")]
    [XmlRoot("FLUXVesselQueryMessage", Namespace = "urn:un:unece:uncefact:data:standard:FLUXVesselQueryMessage:5", IsNullable = false)]
    public partial class FLUXVesselQueryMessageType
    {
        public VesselQueryType VesselQuery { get; set; }
    }
}
