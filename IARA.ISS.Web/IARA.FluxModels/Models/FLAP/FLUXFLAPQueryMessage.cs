using System;
using System.Xml.Serialization;


namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:FLUXFLAPQueryMessage:1")]
    [XmlRoot("FLUXFLAPQueryMessage", Namespace = "urn:un:unece:uncefact:data:standard:FLUXFLAPQueryMessage:1", IsNullable = false)]
    public partial class FLUXFLAPQueryMessageType
    {
        public FLAPQueryType FLAPQuery { get; set; }
    }

}
