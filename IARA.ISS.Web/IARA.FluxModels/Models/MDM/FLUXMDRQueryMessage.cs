using System;
using System.Xml.Serialization;


namespace IARA.Flux.Models
{
    [Serializable]
    [XmlType(Namespace = "urn:un:unece:uncefact:data:standard:FLUXMDRQueryMessage:5")]
    [XmlRoot("FLUXMDRQueryMessage", Namespace = "urn:un:unece:uncefact:data:standard:FLUXMDRQueryMessage:5", IsNullable = false)]
    public partial class FLUXMDRQueryMessageType
    {
        public MDRQueryType MDRQuery { get; set; }
    }

}
